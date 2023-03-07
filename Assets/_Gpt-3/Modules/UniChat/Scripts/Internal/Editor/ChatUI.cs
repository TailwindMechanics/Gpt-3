#if UNITY_EDITOR

using Event = UnityEngine.Event;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using System;

using Modules.UniChat.Internal.DataObjects;


namespace Modules.UniChat.Internal.Editor
{
    public class ChatUI : EditorWindow
    {
        ScrollView chatBoxScrollView;
        TextField inputBoxTextField;
        ConversationSo conversation;
        bool aiIsTyping;


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
                if (!Event.current.Equals(Event.KeyboardEvent("Return"))) return;
                if (aiIsTyping || string.IsNullOrWhiteSpace(inputBoxTextField.text))
                {
                    inputBoxTextField.Focus();
                    return;
                }

                OnSendMessage();
            });

            chatBoxScrollView.contentContainer.RegisterCallback<GeometryChangedEvent>(data =>
            {
                chatBoxScrollView.scrollOffset = new Vector2(0, data.newRect.height);
            });
        }

        async void OnSendMessage ()
        {
            var message = inputBoxTextField.text.Trim();
            DisplayMessage(conversation.Username, message);
            await RequestChatBotReply(conversation.Username, message);
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

        async Task RequestChatBotReply(string senderName, string messageText)
        {
            var result = await conversation.GetChatBotReply(senderName, messageText);
            DisplayMessage(conversation.BotName, result);
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