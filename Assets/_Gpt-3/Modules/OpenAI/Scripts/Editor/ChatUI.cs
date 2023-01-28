using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;

using Modules.OpenAI.External.DataObjects;
using OpenAI_API;


namespace Modules.OpenAI.Editor
{
    public class ChatUI : EditorWindow
    {
        const string openAiCredsPath    = "Assets/_Gpt-3/GitIgnored/Credentials/OpenAI/TM_openApiCredentials.asset";
        const string demoHistoryPath    = "Assets/_Gpt-3/Modules/OpenAI/Data/ChatHistory/Demo_chatHistory.asset";
        const string aiHistoryPath      = "Assets/_Gpt-3/Modules/OpenAI/Data/ChatHistory/AI_ChatHistory.asset";
        const string uxmlPath           = "Assets/_Gpt-3/Modules/OpenAI/UI/OpenAI_visualTree.uxml";
        const string ussPath            = "Assets/_Gpt-3/Modules/OpenAI/UI/OpenAI_uss.uss";

        const string inputBoxTextFieldName  = "inputBox_textField";
        const string chatBoxScrollViewName  = "chatBox_scrollView";

        OpenApiCredentialsSo openApiCredentialsSo;
        ScrollView chatBoxScrollView;
        TextField inputBoxTextField;
        ChatHistorySo conversation;
        VisualElement root;


        [MenuItem("Testing/Show Window")]
        public static void ShowWindow ()
        {
            var window = GetWindow<ChatUI>();
            window.titleContent = new GUIContent("Chat");
            window.minSize = new Vector2(100, 100);
        }

        void CreateGUI()
        {
            openApiCredentialsSo = AssetDatabase.LoadAssetAtPath<OpenApiCredentialsSo>(openAiCredsPath);
            conversation = AssetDatabase.LoadAssetAtPath<ChatHistorySo>(demoHistoryPath);

            root = rootVisualElement;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            root.Add(visualTree.Instantiate());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
            root.styleSheets.Add(styleSheet);

            inputBoxTextField = root.Q<TextField>(inputBoxTextFieldName);
            chatBoxScrollView = root.Q<ScrollView>(chatBoxScrollViewName);
            chatBoxScrollView.contentContainer.RegisterCallback<GeometryChangedEvent>(data =>
            {
                chatBoxScrollView.scrollOffset = new Vector2(0, data.newRect.height);
            });

            for (var i = 0; i < conversation.History.Count; i++)
            {
                var previous = i > 0 ? conversation.History[i-1] : null;
                AddMessage(conversation.History[i], previous);
            }

            inputBoxTextField.Focus();
            inputBoxTextField.RegisterCallback<KeyDownEvent>(_ =>
            {
                if (!Event.current.Equals(Event.KeyboardEvent("Return"))) return;
                if (string.IsNullOrWhiteSpace(inputBoxTextField.text)) return;

                // Construct new user message, display it, and save it
                var newUserMessage = new ChatMessageVo(conversation.CurrentUser, inputBoxTextField.text);
                AddMessage(newUserMessage, conversation.Latest);
                SaveHistory(newUserMessage);

                // Clear input box
                inputBoxTextField.SetValueWithoutNotify("");
                inputBoxTextField.Focus();

                if (conversation.ApiCallsEnabled)
                {
                    Debug.Log("<color=cyan><b>>>> API Enabled</b></color>");
                    inputBoxTextField.SetEnabled(false);
                    RequestAiReply(newUserMessage, aiReply =>
                    {
                        var newAiMessage = new ChatMessageVo("AI", aiReply);
                        AddMessage(newAiMessage, newUserMessage);
                        SaveHistory(newAiMessage);
                    });
                }
                else
                {
                    Debug.Log("<color=orange><b>>>> API Disabled</b></color>");
                }
            });
        }

        void AddMessage (ChatMessageVo messageVo, ChatMessageVo previous)
        {
            var continuedMessage    = previous != null && previous.SenderName == messageVo.SenderName;
            var dayChanged          = previous == null || previous.TimestampDateTime.Date != messageVo.TimestampDateTime.Date;

            var senderText          = continuedMessage ? "" : $"{messageVo.SenderName}";
            var messageText         = messageVo.Message.Trim();
            var timestampText       = dayChanged ? messageVo.Timestamp : "";

            var newMessage = new ChatMessageVisualElement(senderText, messageText, timestampText);
            chatBoxScrollView.Add(newMessage);
        }

        void SaveHistory (ChatMessageVo newMessage)
        {
            conversation.Add(newMessage);
            EditorUtility.SetDirty(conversation);
            AssetDatabase.SaveAssetIfDirty(conversation);
            AssetDatabase.Refresh();
        }

        async Task RequestAiReply (ChatMessageVo input, Action<string> callback)
        {
            Debug.Log("<color=cyan><b>>>> API Enabled: RequestAiReply</b></color>");
            var api = new OpenAIAPI(openApiCredentialsSo.ApiKey);
            var request = new CompletionRequest
            (
                input.Message,
                Model.DavinciText,
                200,
                0.5,
                presencePenalty: 0.1,
                frequencyPenalty: 0.1
            );

            var message = "";
            await foreach (var token in api.Completions.StreamCompletionEnumerableAsync(request))
            {
                message += token.ToString();
            }

            callback.Invoke(message);
        }
    }
}