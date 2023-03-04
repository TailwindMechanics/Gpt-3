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

            ResetInputField();

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

        async void OnSendMessage ()
        {
            var message = inputBoxTextField.text.Trim();
            AddMessage(conversation.Username, message, false, false);
            ResetInputField();
            SetEditorDirty();

            var prompt = conversation.GetPromptJson(conversation.Username, message);
            await RequestAiReply(prompt);
        }

        void ResetInputField ()
        {
            inputBoxTextField.SetValueWithoutNotify("");
            inputBoxTextField.Focus();
        }

        void AddMessage (string senderName, string messageText, bool isBot, bool reloading)
        {
            var newMessageVe = new ChatMessageVisualElement(senderName, messageText, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            newMessageVe.ToggleAlignment(senderName == conversation.Username);
            chatBoxScrollView.Add(newMessageVe);

            if (reloading) return;

            conversation.Add(new MessageVo(senderName, messageText, isBot));
        }

        async Task RequestAiReply(string messageText)
        {
            var result = await conversation.GetAiReply(messageText);
            AddMessage(conversation.BotName, result.Trim(), true, false);
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