#if UNITY_EDITOR

using System.Text.RegularExpressions;
using Event = UnityEngine.Event;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;
using System;
using System.Web;
using Modules.UniChat.Internal.DataObjects;
using UnityEngine.Networking;


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

        async void OnSendMessage()
        {
            var userMessage = inputBoxTextField.text.Trim();
            ResetInputField();

            conversation.AddUserMessage(conversation.Username, userMessage);

            if (userMessage.Contains($"@{SearchBotName}", StringComparison.OrdinalIgnoreCase))
            {
                await ProcessMessagesWithSearchBot(userMessage);
            }
            else
            {
                await ProcessMessagesWithoutSearchBot(userMessage);
            }
        }

        async Task ProcessMessagesWithSearchBot(string userMessage)
        {
            DisplayMessage(conversation.Username, userMessage);
            var searchBotReply = await conversation.GetSearchBotReply(SearchBotName, userMessage);
            DisplayMessage(SearchBotName, searchBotReply);

            var chatBotReply = await conversation.GetChatBotReply(conversation.Username, $"{userMessage}\n{searchBotReply}");
            if (conversation.BotName != null)
            {
                DisplayMessage(conversation.BotName, chatBotReply);
            }
        }

        async Task ProcessMessagesWithoutSearchBot(string userMessage)
        {
            DisplayMessage(conversation.Username, userMessage);
            if (conversation.BotName == null) return;

            var chatBotReply = await conversation.GetChatBotReply(conversation.Username, userMessage);
            DisplayMessage(conversation.BotName, chatBotReply);

            if (chatBotReply.Contains($"@{SearchBotName}", StringComparison.OrdinalIgnoreCase))
            {
                var searchBotReply = await conversation.GetSearchBotReply(SearchBotName, chatBotReply);
                DisplayMessage(SearchBotName, searchBotReply);

                chatBotReply = await conversation.GetChatBotReply(SearchBotName, searchBotReply);
                DisplayMessage(conversation.BotName, chatBotReply);
            }
        }

        void ResetInputField ()
        {
            inputBoxTextField.SetValueWithoutNotify("");
            inputBoxTextField.Focus();
        }

        void DisplayMessage(string senderName, string messageText)
        {
            messageText = Regex.Replace(messageText, $"@{SearchBotName}", _ =>
            {
                var result = $"<color=#40A0FF><u>@{SearchBotName}</u></color>";
                return result;
            }, RegexOptions.IgnoreCase);

            var newMessageVe = new ChatMessageVisualElement(senderName, messageText, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            newMessageVe.ToggleAlignment(senderName == conversation.Username);
            chatBoxScrollView.Add(newMessageVe);
            SetEditorDirty();
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