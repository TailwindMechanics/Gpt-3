#if UNITY_EDITOR

using UnityEngine.UIElements;
using System.Linq;
using UnityEditor;
using UnityEngine;

using Modules.UniChat.External.DataObjects;


namespace Modules.UniChat.Editor
{
    public class ChatUI : EditorWindow
    {
        ScrollView chatScrollView;
        TextField inputField;
        IChat chatBot;


        [MenuItem("Window/Chat Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<ChatUI>();
            window.titleContent = new GUIContent("Chat Window");
            window.minSize = new Vector2(300, 400);
        }

        void OnEnable()
        {
            chatBot = AssetDatabase.FindAssets($"t:{typeof(ScriptableObject)}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid)))
                .OfType<IChat>()
                .FirstOrDefault();

            var uxmlAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_Gpt-3/Modules/UniChat/UI/ChatWindow.uxml");
            var ussAsset = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/_Gpt-3/Modules/UniChat/UI/ChatWindow.uss");
            var root = rootVisualElement;

            uxmlAsset.CloneTree(root);
            root.styleSheets.Add(ussAsset);

            inputField = root.Q<TextField>("inputBox_textField");
            chatScrollView = root.Q<ScrollView>("chatBox_scrollView");

            inputField.RegisterCallback<KeyDownEvent>(OnInputFieldKeyDown);

            ScrollToBottomOnScrollViewChange();
        }

        async void OnInputFieldKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode != KeyCode.Return) return;

            var message = inputField.value.Trim();
            inputField.value = "";
            AddChatMessage("You", message);
            var response = await chatBot.GetResponse(message);
            var responseText = response.Trim();
            AddChatMessage("ChatBot", responseText);
        }

        void AddChatMessage(string sender, string message)
        {
            var messageContainer = new VisualElement();
            messageContainer.AddToClassList("message-container");

            var senderLabel = new Label(sender + ":");
            senderLabel.AddToClassList("sender-label");

            var messageLabel = new Label(message);
            messageLabel.AddToClassList("message-label");

            messageContainer.Add(senderLabel);
            messageContainer.Add(messageLabel);

            chatScrollView.contentContainer.Add(messageContainer);
        }

        void ScrollToBottomOnScrollViewChange()
        {
            chatScrollView.contentContainer.RegisterCallback<GeometryChangedEvent>(data =>
            {
                chatScrollView.scrollOffset = new Vector2(0, data.newRect.height);
            });
        }
    }
}

#endif