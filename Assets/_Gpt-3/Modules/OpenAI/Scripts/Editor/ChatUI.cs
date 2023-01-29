using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using OpenAI_API;
using System;

using Modules.OpenAI.External.DataObjects;


namespace Modules.OpenAI.Editor
{
    public class ChatUI : EditorWindow
    {
        const string aiName                 = "Uni";
        const string openAiCredsPath        = "Assets/_Gpt-3/GitIgnored/Credentials/OpenAI/TM_openApiCredentials.asset";
        const string conversationPath       = "Assets/_Gpt-3/Modules/OpenAI/Data/Chat/Main_conversation.asset";
        const string uxmlPath               = "Assets/_Gpt-3/Modules/OpenAI/UI/OpenAI_uxml.uxml";
        const string ussPath                = "Assets/_Gpt-3/Modules/OpenAI/UI/OpenAI_uss.uss";
        const string inputBoxTextFieldName  = "inputBox_textField";
        const string chatBoxScrollViewName  = "chatBox_scrollView";

        OpenApiCredentialsSo openApiCredentialsSo;
        ChatMessageVisualElement activeElement;
        ScrollView chatBoxScrollView;
        TextField inputBoxTextField;
        ConversationSo conversation;
        bool aiIsTyping;


        // todo
        // Implement up/down keys for repopulating previous messages
        // Make text copy/paste-able
        // gpt has a window of 8000 characters
        // Allow text input while ai is typing, just don't allow sending
            // Or maybe sending a message mid-reply shuts it up
        // Enable canceling of ai reply
        // Maybe add a scroll to latest button
        // Maybe add some kind of "AI is thinking" indicator (window title?)
        // Name the AI
        // Would be cool to have /commands
        // Right click and send anything in Unity to the AI to analyse

        [MenuItem("Tools/" + aiName)]
        public static void ShowWindow ()
        {
            var window              = GetWindow<ChatUI>();
            window.titleContent     = new GUIContent(aiName);
            window.minSize          = new Vector2(100, 100);
        }

        void CreateGUI()
        {
            rootVisualElement.Add(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath).Instantiate());
            rootVisualElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath));
            openApiCredentialsSo    = AssetDatabase.LoadAssetAtPath<OpenApiCredentialsSo>(openAiCredsPath);
            conversation            = AssetDatabase.LoadAssetAtPath<ConversationSo>(conversationPath);
            inputBoxTextField       = rootVisualElement.Q<TextField>(inputBoxTextFieldName);
            chatBoxScrollView       = rootVisualElement.Q<ScrollView>(chatBoxScrollViewName);

            inputBoxTextField.SetValueWithoutNotify("");
            inputBoxTextField.Focus();

            LoadChatHistory();
            ListenForKeyPress();
            ListenForScrollViewChange();
        }

        void ListenForScrollViewChange ()
        {
            chatBoxScrollView.contentContainer.RegisterCallback<GeometryChangedEvent>(data =>
            {
                chatBoxScrollView.scrollOffset = new Vector2(0, data.newRect.height);
            });
        }

        void ListenForKeyPress ()
        {
            inputBoxTextField.RegisterCallback<KeyDownEvent>(_ =>
            {
                if (!Event.current.Equals(Event.KeyboardEvent("Return"))) return;
                if (string.IsNullOrWhiteSpace(inputBoxTextField.text)) return;
                if (aiIsTyping)
                {
                    inputBoxTextField.Focus();
                    return;
                }

                OnSendMessage();
            });
        }

        void LoadChatHistory ()
        {
            for (var i = 0; i < conversation.History.Count; i++)
            {
                AddMessage(conversation.History[i].SenderName, conversation.History[i].Message, conversation.History[i].TimestampDateTime, i, true);
            }
        }

        void OnSendMessage ()
        {
            var message = inputBoxTextField.text.Trim();
            AddMessage(conversation.CurrentUser, message, DateTime.Now, conversation.LatestIndex, false);
            inputBoxTextField.SetValueWithoutNotify("");
            SaveChatHistory();

            if (!conversation.ApiCallsEnabled)
            {
                Debug.Log("<color=orange><b>>>> API Disabled</b></color>");
                return;
            }

            Debug.Log("<color=cyan><b>>>> API Enabled</b></color>");
            RequestAiReply(message);
        }

        void AddMessage (string sender, string messageText, DateTime timestamp, int index, bool reloading)
        {
            var isLocalUser         = sender == conversation.CurrentUser;
            var previous            = index > 0 ? conversation.History[index-1] : null;
            var dayChanged          = previous != null && previous.TimestampDateTime.Date != timestamp.Date;
            var continuedMessage    = previous != null && !dayChanged && previous.SenderName == sender;
            var senderText          = continuedMessage ? "" : $"{sender}";
            var timestampText       = previous == null || dayChanged ? DateTime.Now.ToString("dd/MM/yyyy HH:mm") : "";

            if (continuedMessage == false)
            {
                AddNewUIMessage(senderText, messageText, timestampText, isLocalUser);
            }
            else if (reloading == false)
            {
                if (isLocalUser) messageText = $"\n{messageText}";
                AppendExistingMessage(messageText, index);
            }

            if (!reloading) AddNewMessageHistory(senderText, messageText);
        }

        void AddNewUIMessage (string senderText, string messageText, string timestampText, bool isLocalUser)
        {
            var newMessageVe = new ChatMessageVisualElement(senderText, messageText, timestampText);
            newMessageVe.ToggleAlignment(isLocalUser);
            chatBoxScrollView.Add(newMessageVe);
            activeElement = newMessageVe;
        }

        void AddNewMessageHistory(string senderText, string messageText)
            => conversation.Add(new MessageVo(senderText, messageText));

        void AppendExistingMessage (string messageText, int index)
        {
            activeElement.AppendMessage($"{messageText}");
            conversation.AppendMessage(index, $"{messageText}");
        }

        async Task RequestAiReply (string messageText)
        {
            var api = new OpenAIAPI(new APIAuthentication(openApiCredentialsSo.ApiKey));
            var request = new CompletionRequest
            (
                prompt:             messageText,
                model:              MapModelToEnum(conversation.OpenAISettings.Model),
                max_tokens:         conversation.OpenAISettings.MaxTokens,
                temperature:        conversation.OpenAISettings.Temperature,
                presencePenalty:    conversation.OpenAISettings.PresencePenalty,
                frequencyPenalty:   conversation.OpenAISettings.FrequencyPenalty
            );

            aiIsTyping = true;
            var message = "";
            AddMessage(aiName, message, DateTime.Now, conversation.LatestIndex, false);

            await foreach (var token in api.Completions.StreamCompletionEnumerableAsync(request))
            {
                aiIsTyping = true;
                var tokenString = token.ToString();
                message += tokenString;
                if (string.IsNullOrWhiteSpace(message)) continue;

                AppendExistingMessage(tokenString, conversation.LatestIndex);
            }

            SaveChatHistory();
            aiIsTyping = false;
            inputBoxTextField.Focus();
        }

        void SaveChatHistory ()
        {
            EditorUtility.SetDirty(conversation.GetHistory());
            AssetDatabase.SaveAssetIfDirty(conversation.GetHistory());
            EditorUtility.SetDirty(conversation);
            AssetDatabase.SaveAssetIfDirty(conversation);
            AssetDatabase.Refresh();
        }

        Model MapModelToEnum (OpenAiModel enumModel)
        {
            return enumModel switch
            {
                OpenAiModel.AdaText             => Model.AdaText,
                OpenAiModel.AdaTextEmbedding    => Model.AdaTextEmbedding,
                OpenAiModel.BabbageText         => Model.BabbageText,
                OpenAiModel.CurieText           => Model.CurieText,
                OpenAiModel.CushmanCode         => Model.CushmanCode,
                OpenAiModel.DavinciCode         => Model.DavinciCode,
                OpenAiModel.DavinciText         => Model.DavinciText,
                _ => Model.DavinciCode
            };
        }
    }
}