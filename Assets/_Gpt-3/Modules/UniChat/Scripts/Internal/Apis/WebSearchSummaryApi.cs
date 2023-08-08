using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using OpenAI.Chat;
using System.Linq;
using System.Text;
using UnityEngine;
using AngleSharp;
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
        readonly bool logging;


        public WebSearchSummaryApi(WebSearchSettingsVo newWebSettings, bool doLogging)
        {
            try
            {
                logging = doLogging;
                webSettings = newWebSettings;
                openAiApi = new OpenAIClient();
                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
            }
            catch (HttpRequestException ex)
            {
                Log($"Error initializing APIs: {ex.Message}", true);
                throw;
            }
        }

        public async Task<string> SearchAndGetSummary(string message)
        {
            try
            {
                var query = await ConstructSearchQuery(message);
                var searchResults = await Search(query);
                var parsedSearch = ParseSearchResults(searchResults);
                var summary = await GetSummary(query, new UrlContent("google.com", parsedSearch));

                if (!string.IsNullOrEmpty(summary) && !summary.Contains("Content not relevant"))
                {
                    summary = await ShortenLongUrlsInText(summary);
                    Log($"Received summary: {summary}");
                    return summary;
                }

                const int maxResultsToProcess = 5;
                foreach (var item in searchResults.Items.Take(maxResultsToProcess))
                {
                    try
                    {
                        var extractedContent = await ScrapeUrlAsync(item.Url);
                        if (extractedContent == null) continue;

                        if (!string.IsNullOrEmpty(summary) && !summary.Contains("Content not relevant"))
                        {
                            summary = await ShortenLongUrlsInText(summary);
                            Log($"Received summary: {summary}");
                            return summary;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log($"Error processing URL {item.Url}: {ex.Message}", true);
                    }
                }

                return "Unable to generate a summary due to insufficient data.";
            }
            catch (HttpRequestException ex)
            {
                Log($"Error processing query: {ex.Message}", true);
                throw;
            }
        }

        async Task<string> ConstructSearchQuery(string chatMessage)
        {
            try
            {
                Log($"Constructing query with AI using chat message: {chatMessage}");

                var chatPrompts = new List<Message>
                {
                    new(Role.System, "You are SearchBot. Please construct a Google search query based on the following chat message:"),
                    new(Role.User, chatMessage)
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

                Log($"Constructed query with AI: {constructedQuery}");

                return constructedQuery;
            }
            catch (Exception ex)
            {
                Log($"Error constructing query with AI: {ex.Message}", true);
                throw;
            }
        }

        async Task<GoogleSearchResponse> Search(string query)
        {
            try
            {
                Log($"Searching: {query}");

                var requestUrl = $"{webSettings.BaseUrl}{webSettings.ApiKey}&cx={webSettings.EngineId}&q={Uri.EscapeDataString(query)}";
                var response = await httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var searchResponse = JsonConvert.DeserializeObject<GoogleSearchResponse>(responseContent);

                Log($"Received Google Search results: {searchResponse.Items.Count}");
                return searchResponse;
            }
            catch (Exception ex)
            {
                Log($"Error searching with Google API: {ex.Message}", true);
                throw;
            }
        }

        async Task<UrlContent> ScrapeUrlAsync(string url)
        {
            try
            {
                const string cssSelector = "main, article, #content, .content, .post, .article, .entry-content, .post-content";
                var exclusionSelectors = new List<string> { "nav", "footer", ".ad", ".advertisement", ".promo", ".widget", ".comment", ".comments", ".related-posts" };

                using var response = await httpClient.GetAsync(url);
                var byteArray = await response.Content.ReadAsByteArrayAsync();
                var responseContent = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

                var config = Configuration.Default.WithDefaultLoader();
                var document = await BrowsingContext.New(config).OpenAsync(req => req.Content(responseContent));
                var targetedElements = document.QuerySelectorAll(cssSelector);
                Log($"HTML Document: {document.DocumentElement.OuterHtml}");

                exclusionSelectors.ForEach(selector =>
                {
                    document.QuerySelectorAll(selector).ToList().ForEach(element =>
                    {
                        element.Remove();
                    });
                });

                var filteredElements = targetedElements.Where(e => e.Parent != null);
                var content = string.Join("\n\n", filteredElements.Select(e => e.TextContent.Trim()));

                Log($"Scraped: {url}, content: {content}");

                return new UrlContent(url, content);
            }
            catch (HttpRequestException ex)
            {
                Log($"HttpRequestException {url}: {ex.Message}", true);
                return null;
            }
            catch (Exception ex)
            {
                Log($"Exception {url}: {ex.Message}", true);
                return null;
            }
        }

        async Task<string> GetSummary(string query, UrlContent content)
        {
            try
            {
                const int maxContentTokens = 2000;
                var truncatedContent = TruncateToTokens(content.Content, maxContentTokens);

                var system = $"{webSettings.SummaryModel.Direction}\ndatetime: {DateTime.UtcNow:yyyy-MM-dd HH:mm}\n";
                var chatPrompts = new List<Message>
                {
                    new(Role.System, system),
                    new(Role.User, $"query: {query}"),
                    new(Role.User, $"content: {truncatedContent}")
                };

                Log($"Requesting summary for url: {content.Url}, system: {system}");

                var summaryRequest = new ChatRequest
                (
                    model: webSettings.SummaryModel.Model,
                    messages: chatPrompts,
                    maxTokens: webSettings.SummaryModel.MaxTokens,
                    temperature: webSettings.SummaryModel.Temperature,
                    topP: webSettings.SummaryModel.TopP,
                    presencePenalty: webSettings.SummaryModel.PresencePenalty,
                    frequencyPenalty: webSettings.SummaryModel.FrequencyPenalty
                );

                var response = await openAiApi.ChatEndpoint.GetCompletionAsync(summaryRequest);
                var summary = response.FirstChoice.ToString().Trim();
                if (!summary.Contains("Content not relevant")) return summary;

                Log($"Skipped non-relevant content: {truncatedContent.Split(' ').Length} tokens, summary: {summary}, url: {content.Url}");

                return summary;
            }
            catch (Exception ex)
            {
                Log($"Error generating summary: {ex.Message}", true);
                throw;
            }
        }

        string TruncateToTokens(string text, int maxTokens)
        {
            var words = text.Split(' ');
            return words.Length <= maxTokens ? text : string.Join(" ", words.Take(maxTokens));
        }

        string ParseSearchResults(GoogleSearchResponse response)
        {
            try
            {
                const int resultsToParse = 20;
                if (response.Items.Count > resultsToParse)
                {
                    response.Items.RemoveRange(resultsToParse, response.Items.Count - resultsToParse);
                }

                Log($"Parsed {response.Items.Count} search results.");

                var sb = new StringBuilder();
                foreach (var item in response.Items)
                {
                    sb.AppendLine(item.Title);
                    sb.AppendLine(item.Snippet);
                    sb.AppendLine(item.Url);
                    sb.AppendLine();
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Log($"Error parsing search results: {ex.Message}", true);
                throw;
            }
        }

        async Task<string> ShortenLongUrlsInText(string text)
        {
            const int maxLength = 100;
            const string urlPattern = @"https?://(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’])";

            var matches = Regex.Matches(text, urlPattern);
            var outputText = text;

            foreach (Match match in matches)
            {
                if (match.Value.Length <= maxLength) continue;

                var shortUrl = await ShortenUrl(match.Value);
                outputText = outputText.Replace(match.Value, shortUrl);
            }

            return outputText;
        }

        async Task<string> ShortenUrl(string longUrl)
        {
            try
            {
                var request = new { url = longUrl };
                var jsonRequest = JsonConvert.SerializeObject(request);
                var requestContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", webSettings.TinyUrlSettingsVo.ApiKey);

                var response = await httpClient.PostAsync(webSettings.TinyUrlSettingsVo.Endpoint, requestContent);
                response.EnsureSuccessStatusCode();

                var utf8Encoding = Encoding.GetEncoding("UTF-8");
                var contentBytes = await response.Content.ReadAsByteArrayAsync();
                var jsonResponse = utf8Encoding.GetString(contentBytes);

                var jsonResponseObj = JsonConvert.DeserializeObject<TinyUrlResponse>(jsonResponse);
                var shortUrl = jsonResponseObj.Data.TinyUrl;

                Log($"Shortened url: {longUrl} -> {shortUrl}");
                return shortUrl;
            }
            catch (HttpRequestException ex)
            {
                Log($"Failed to shorten url {ex.Message}: {longUrl}", true);
                return null;
            }
        }

        public class TinyUrlResponse
        {
            [JsonProperty("data")]
            public TinyUrlData Data { get; set; }
        }

        public class TinyUrlData
        {
            [JsonProperty("tiny_url")]
            public string TinyUrl { get; set; }
        }


        void Log(string message, bool error = false)
        {
            if (!logging) return;

            var color = error ? "#E08B87" : "#ADD9D9";
            message = message.Replace("<", "&lt;").Replace(">", "&gt;");
            Debug.Log($"<color={color}><b>>>> WebSearchSummaryApi: {message.Replace("\n", "")}</b></color>");
        }
    }
}