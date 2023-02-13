using System.Threading.Tasks;
using UnityEngine.UIElements;
using OpenAI.Completions;
using OpenAI.Models;
using UnityEditor;
using UnityEngine;
using OpenAI;
using System;

using Modules.UniChat.External.DataObjects;
using Event = UnityEngine.Event;


namespace Modules.UniChat.Editor
{
    public class ChatUI : EditorWindow
    {
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

        [MenuItem("Tools/UniChat")]
        public static void ShowWindow ()
        {
            var window              = GetWindow<ChatUI>();
            window.titleContent     = new GUIContent("UniChat");
            window.minSize          = new Vector2(100, 100);
        }

        void CreateGUI()
        {
            rootVisualElement.Add(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Constants.WindowUxmlPath).Instantiate());
            rootVisualElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(Constants.WindowUssPath));
            conversation            = AssetDatabase.LoadAssetAtPath<ConversationSo>(Constants.ConversationPath);
            inputBoxTextField       = rootVisualElement.Q<TextField>(Constants.InputBoxTextFieldName);
            chatBoxScrollView       = rootVisualElement.Q<ScrollView>(Constants.ChatBoxScrollViewName);

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

        async void OnSendMessage ()
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

            var prompt = conversation.FormatPrompt(message);
            await RequestAiReply(prompt);
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

        async Task RequestAiReply (string messageText)
        {
            if (string.IsNullOrEmpty(conversation.OpenAISettings.Model.ToString()))
            {
                Debug.Log("<color=orange><b>>>> No model selected</b></color>");
                return;
            }

            var api = new OpenAIClient();
            var request = new CompletionRequest
            (
                prompt:             messageText + "\n",
                maxTokens:          conversation.OpenAISettings.MaxTokens,
                temperature:        conversation.OpenAISettings.Temperature,
                presencePenalty:    conversation.OpenAISettings.PresencePenalty,
                frequencyPenalty:   conversation.OpenAISettings.FrequencyPenalty,
                model:              conversation.OpenAISettings.Model
            );

            aiIsTyping = true;
            var message = "";
            AddMessage(conversation.AiName, message, DateTime.Now, conversation.LatestIndex, false);

            await foreach (var token in api.CompletionsEndpoint.StreamCompletionEnumerableAsync(request))
            {
                aiIsTyping = true;
                var tokenString = token.ToString();
                message += tokenString;
                if (string.IsNullOrWhiteSpace(message)) continue;
                // if (message.Contains("|.")) continue;

                AppendExistingMessage(tokenString, conversation.LatestIndex);
            }

            conversation.SetMemories(message);
            SaveChatHistory();
            aiIsTyping = false;
            inputBoxTextField.Focus();
        }

        void AppendExistingMessage (string messageText, int index)
        {
            activeElement.AppendMessage($"{messageText}");
            conversation.AppendMessage(index, $"{messageText}");
        }

        void SaveChatHistory ()
        {
            foreach (var scriptableObject in conversation.GetScriptableObjects())
            {
                EditorUtility.SetDirty(scriptableObject);
                AssetDatabase.SaveAssetIfDirty(scriptableObject);
            }

            EditorUtility.SetDirty(conversation);
            AssetDatabase.SaveAssetIfDirty(conversation);
            AssetDatabase.Refresh();
        }
    }
}