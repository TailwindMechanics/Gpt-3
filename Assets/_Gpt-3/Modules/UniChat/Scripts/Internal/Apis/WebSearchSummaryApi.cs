﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using OpenAI.Chat;
using System.Linq;
using UnityEngine;
using System.Net;
using AngleSharp;
using System.IO;
using System;
using OpenAI;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.Vo;
using Modules.UniChat.Internal.DataObjects;


namespace Modules.UniChat.Internal.Apis
{
    public class WebSearchSummaryApi : IWebSearchSummaryApi
    {
        readonly WebSearchSettingsVo webSettings;
        readonly OpenAIClient openAiApi;
        readonly HttpClient httpClient;

        public WebSearchSummaryApi(WebSearchSettingsVo newWebSettings)
        {
            try
            {
                webSettings = newWebSettings;
                openAiApi = new OpenAIClient();
                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
                Log("APIs initialized successfully");
            }
            catch (HttpRequestException ex)
            {
                Debug.LogError($"Error initializing APIs: {ex.Message}");
                throw;
            }
        }

        public async Task<string> SearchAndGetSummary(string message, bool logging = false)
        {
            try
            {
                var query = await ConstructSearchQuery(message, logging);

                const string cssSelector = "main, article, #content";
                if (logging) Log($"Sending search query to Google Search API: {query}");
                var searchResults = await Search(query, logging);
                if (logging) Log($"Received search results from Google Search API: {searchResults.Items.Count}");

                var topUrls = searchResults.Items.Select(sr => sr.Url).ToList();
                if (logging) Log($"Attempting to scrape top URLs: {topUrls.Count}");
                var summarizedData = "Unable to generate a summary due to insufficient data.";

                var retryCount = 0;
                foreach (var url in topUrls)
                {
                    retryCount++;
                    if (retryCount > 3) break;

                    if (logging) Log($"Scraping URL (attempt {retryCount}): {url}");
                    var extractedContent = await ScrapeUrlAsync(url, cssSelector, logging);
                    if (string.IsNullOrEmpty(extractedContent.Content))
                    {
                        continue;
                    }

                    if (logging) Log("Scraped top URL successfully");

                    if (logging) Log("Sending extracted content to OpenAI for salient summary");
                    summarizedData = await GetSalientSummaryFromOpenAiAsync(query, new[] { extractedContent }, logging);
                    if (logging) Log($"Received executive summary from OpenAI: {summarizedData}");

                    if (!summarizedData.Contains("Content not relevant"))
                    {
                        break;
                    }
                }

                Log(summarizedData);

                return summarizedData;
            }
            catch (HttpRequestException ex)
            {
                Log($"Error processing query: {ex.Message}");
                throw;
            }
        }

        async Task<string> ConstructSearchQuery(string chatMessage, bool logging)
        {
            try
            {
                if (logging)
                {
                    Log($"Constructing query with AI using chat message: {chatMessage}");
                }

                var chatPrompts = new List<ChatPrompt>
                {
                    new("system", "You are SearchBot. Please construct a Google search query based on the following chat message:"),
                    new("user", chatMessage)
                };

                var chatRequest = new ChatRequest
                (
                    model: webSettings.SummaryModel.Model,
                    messages: chatPrompts,
                    maxTokens: webSettings.SummaryModel.MaxTokens,
                    temperature: webSettings.SummaryModel.Temperature,
                    topP: webSettings.SummaryModel.TopP,
                    presencePenalty: webSettings.SummaryModel.PresencePenalty,
                    frequencyPenalty: webSettings.SummaryModel.FrequencyPenalty
                );

                var result = await openAiApi.ChatEndpoint.GetCompletionAsync(chatRequest);
                var constructedQuery = result.FirstChoice.ToString().Trim();

                if (logging)
                {
                    Log($"Constructed query with AI: {constructedQuery}");
                }

                return constructedQuery;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error constructing query with AI: {ex.Message}");
                throw;
            }
        }

        async Task<GoogleSearchResponse> Search(string query, bool logging)
        {
            try
            {
                if (logging)
                {
                    Log($"Searching: {query}");
                }

                var requestUrl = $"{webSettings.BaseUrl}{webSettings.ApiKey}&cx={webSettings.EngineId}&q={Uri.EscapeDataString(query)}";
                var response = await httpClient.GetAsync(requestUrl);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var searchResponse = JsonConvert.DeserializeObject<GoogleSearchResponse>(responseContent);

                if (logging)
                {
                    Log($"Received Google Search results: {responseContent.Length}");
                }

                return searchResponse;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error searching with Google API: {ex.Message}");
                throw;
            }
        }

        async Task<UrlContent> ScrapeUrlAsync(string url, string cssSelector, bool logging)
        {
            try
            {
                var request = WebRequest.Create(url);
                using var response = await request.GetResponseAsync();
                await using var stream = response.GetResponseStream();
                if (stream == null) return null;

                using var reader = new StreamReader(stream);
                var responseContent = await reader.ReadToEndAsync();

                var config = Configuration.Default.WithDefaultLoader();
                var document = await BrowsingContext.New(config).OpenAsync(req => req.Content(responseContent));
                var targetedElements = document.QuerySelectorAll(cssSelector);

                if (logging)
                {
                    Log($"Scraped: {url}");
                }

                var content = string.Join("\n\n", targetedElements.Select(e => e.TextContent.Trim()));
                return new UrlContent(url, content);
            }
            catch (WebException ex)
            {
                Debug.LogError($"WebException {url}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception {url}: {ex.Message}");
                return null;
            }
        }

        async Task<string> GetSalientSummaryFromOpenAiAsync(string query, IEnumerable<UrlContent> extractedContents, bool logging)
        {
            var enumerable = extractedContents as UrlContent[] ?? extractedContents.ToArray();
            if (!enumerable.Any())
            {
                Log("No valid extracted contents to generate a summary.");
                return "Unable to generate a summary due to insufficient data.";
            }

            const int maxContentTokens = 2000;
            var summaries = new List<string>();

            foreach (var content in enumerable)
            {
                var truncatedContent = TruncateToTokens(content.Content, maxContentTokens);

                var chatPrompts = new List<ChatPrompt>
                {
                    new("system", $"{webSettings.SummaryModel.Direction}\nquery: {query}, content: {truncatedContent}")
                };

                var chatRequest = new ChatRequest
                (
                    model: webSettings.SummaryModel.Model,
                    messages: chatPrompts,
                    maxTokens: webSettings.SummaryModel.MaxTokens,
                    temperature: webSettings.SummaryModel.Temperature,
                    topP: webSettings.SummaryModel.TopP,
                    presencePenalty: webSettings.SummaryModel.PresencePenalty,
                    frequencyPenalty: webSettings.SummaryModel.FrequencyPenalty
                );

                var result = await openAiApi.ChatEndpoint.GetCompletionAsync(chatRequest);
                var summary = result.FirstChoice.ToString();

                if (summary.Contains("Content not relevant"))
                {
                    if (logging)
                    {
                        Log($"Skipped non-relevant content: {truncatedContent.Split(' ').Length} tokens, summary: {summary}, url: {content.Url}");
                    }
                    continue;
                }

                summaries.Add(summary);

                if (logging)
                {
                    Log($"Generated salient summary with {truncatedContent.Split(' ').Length} tokens: {summaries.Last()}, url: {content.Url}");
                }
            }

            var finalSummaryPrompt = new List<ChatPrompt>
            {
                new("system", $"Please provide a concise and coherent executive summary of the following information in response to the query: '{query}'\n\n{string.Join("\n\n", summaries)}")
            };

            var finalSummaryRequest = new ChatRequest
            (
                model: webSettings.SummaryModel.Model,
                messages: finalSummaryPrompt,
                maxTokens: webSettings.SummaryModel.MaxTokens,
                temperature: webSettings.SummaryModel.Temperature,
                topP: webSettings.SummaryModel.TopP,
                presencePenalty: webSettings.SummaryModel.PresencePenalty,
                frequencyPenalty: webSettings.SummaryModel.FrequencyPenalty
            );

            var finalResult = await openAiApi.ChatEndpoint.GetCompletionAsync(finalSummaryRequest);
            return finalResult.FirstChoice.ToString();
        }

        string TruncateToTokens(string text, int maxTokens)
        {
            var words = text.Split(' ');
            return words.Length <= maxTokens ? text : string.Join(" ", words.Take(maxTokens));
        }

        void Log(string message)
            => Debug.Log($"<color=#ADD9D9><b>>>> WebSearchSummaryApi: {message.Replace("\n", "")}</b></color>");
    }
}