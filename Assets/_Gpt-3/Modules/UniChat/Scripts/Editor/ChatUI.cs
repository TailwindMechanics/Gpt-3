using System.Collections.Generic;
using Event = UnityEngine.Event;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;

using Modules.UniChat.External.DataObjects;


namespace Modules.UniChat.Editor
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
            // Clear and re-populate the chat history
            chatBoxScrollView.Clear();
            foreach (var log in conversation.History)
            {
                AddMessage(log.SenderName, log.Message, log.IsBot, true);
            }

            // Reset input field and scroll to the bottom of the chat box
            ResetInputField();
            chatBoxScrollView.scrollOffset = new Vector2(0, chatBoxScrollView.contentContainer.layout.height);

            // Repaint the window to update the UI
            Repaint();
        }

        void CreateGUI()
        {
            chatBot = AssetDatabase.FindAssets($"t:{typeof(ScriptableObject)}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid)))
                .OfType<IChat>()
                .FirstOrDefault();

            var uxmlAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_Gpt-3/Modules/UniChat/UI/ChatWindow.uxml");
            var ussAsset = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/_Gpt-3/Modules/UniChat/UI/ChatWindow.uss");
            var root = rootVisualElement;

            foreach (var log in conversation.History)
            {
                AddMessage(log.SenderName, log.Message, log.IsBot, true);
            }

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

        async void OnInputFieldKeyDown(KeyDownEvent evt)
        {
            var message = inputBoxTextField.text.Trim();
            AddMessage(conversation.Username, message, false, false);
            ResetInputField();
            SetEditorDirty();

            var prompt = conversation.GetPromptJson(conversation.Username, message);
            await RequestAiReply(prompt);
        }

        void AddChatMessage(string sender, string message)
        {
            var messageContainer = new VisualElement();
            messageContainer.AddToClassList("message-container");

        void AddMessage (string senderName, string messageText, bool isBot, bool reloading, List<float> embedding = null)
        {
            var newMessageVe = new ChatMessageVisualElement(senderName, messageText, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            newMessageVe.ToggleAlignment(senderName == conversation.Username);
            chatBoxScrollView.Add(newMessageVe);

            if (reloading) return;

            var newMessage = new MessageVo(senderName, messageText, isBot);
            if (embedding != null) newMessage.SetEmbedding(embedding);
            conversation.Add(newMessage);
        }

        async Task RequestAiReply(string messageText)
        {
            var result = await conversation.GetAiReply(messageText);
            AddMessage(conversation.BotName, result.response, true, false, result.embedding);
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