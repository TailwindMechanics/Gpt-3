#if UNITY_EDITOR

using System.Collections.Generic;
using Event = UnityEngine.Event;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

using Modules.UniChat.Internal.DataObjects;


namespace Modules.UniChat.Internal.Editor
{
    public class ChatUI : EditorWindow
    {
        const string SearchBotName = "SearchBot";
        ScrollView chatBoxScrollView;
        TextField inputBoxTextField;
        ConversationSo conversation;
        bool aiIsTyping;

        int messageHistoryIndex;


        [MenuItem("Tailwind/UniChat")]
        public static void ShowWindow ()
        {
            var window              = GetWindow<ChatUI>();
            window.titleContent     = new GUIContent("UniChat");
            window.minSize          = new Vector2(100, 100);
            window.Refresh();
        }

        void Refresh()
        {
            LoadConversation();
            ResetInputField();
            Repaint();
        }

        void LoadConversation ()
        {
            chatBoxScrollView.Clear();
            foreach (var log in conversation.History)
            {
                DisplayMessage(log.SenderName, log.Message);
            }
        }

        void CreateGUI()
        {
            rootVisualElement.Add(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Constants.WindowUxmlPath).Instantiate());
            rootVisualElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(Constants.WindowUssPath));
            conversation            = AssetDatabase.LoadAssetAtPath<ConversationSo>(Constants.ConversationPath);
            inputBoxTextField       = rootVisualElement.Q<TextField>(Constants.InputBoxTextFieldName);
            chatBoxScrollView       = rootVisualElement.Q<ScrollView>(Constants.ChatBoxScrollViewName);

            ResetInputField();
            LoadConversation();

            inputBoxTextField.RegisterCallback<KeyDownEvent>(_ =>
            {
                if (Event.current.keyCode == KeyCode.Return && !aiIsTyping && !string.IsNullOrWhiteSpace(inputBoxTextField.text))
                {
                    OnSendMessage();
                }
                else if (Event.current.keyCode is KeyCode.UpArrow)
                {
                    messageHistoryIndex = (messageHistoryIndex - 1 + conversation.History.Count) % conversation.History.Count;
                    inputBoxTextField.value = conversation.History[messageHistoryIndex].Message;
                    inputBoxTextField.SelectAll();
                }
                if (Event.current.keyCode is KeyCode.DownArrow)
                {
                    messageHistoryIndex = 0;
                }
            });

            chatBoxScrollView.contentContainer.RegisterCallback<GeometryChangedEvent>(data =>
            {
                chatBoxScrollView.scrollOffset = new Vector2(0, data.newRect.height);
            });
        }

        async void ProcessCommand(string senderName, string command, string message)
        {
            if (command.Equals(SearchBotName, StringComparison.OrdinalIgnoreCase)
            && !senderName.Equals(SearchBotName, StringComparison.OrdinalIgnoreCase))
            {
                var searchBotReply = await conversation.GetSearchBotReply(message);
                DisplayMessage(SearchBotName, searchBotReply);

                await RequestBotReply(conversation.BotName, SearchBotName, searchBotReply);
            }
        }

        async void OnSendMessage()
        {
            var message = inputBoxTextField.text.Trim();
            DisplayMessage(conversation.Username, message);

            var botMentions = FindMentionedBots(message);
            foreach (var mention in botMentions)
            {
                ProcessCommand(conversation.Username, mention, message);
            }

            ResetInputField();
            await RequestBotReply(conversation.BotName, conversation.Username, message);
        }

        List<string> FindMentionedBots(string message)
        {
            var words = message.Split(' ');
            return (from t in words where t.StartsWith("@") select t[1..]).ToList();
        }

        void ResetInputField ()
        {
            inputBoxTextField.SetValueWithoutNotify("");
            inputBoxTextField.Focus();
        }

        void DisplayMessage (string senderName, string messageText)
        {
            var newMessageVe = new ChatMessageVisualElement(senderName, messageText, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            newMessageVe.ToggleAlignment(senderName == conversation.Username);
            chatBoxScrollView.Add(newMessageVe);
            SetEditorDirty();
        }

        async Task RequestBotReply(string botName, string senderName, string messageText)
        {
            var result = await conversation.GetChatBotReply(senderName, messageText);
            var botMentions = FindMentionedBots(result);
            foreach (var mention in botMentions)
            {
                ProcessCommand(botName, mention, result);
            }
            DisplayMessage(botName, result);
        }

        void SetEditorDirty ()
        {
            EditorUtility.SetDirty(conversation);
            AssetDatabase.SaveAssetIfDirty(conversation);
            AssetDatabase.Refresh();
        }
    }
}

#endif