using Modules.OpenAI.External.DataObjects;
using Label = UnityEngine.UIElements.Label;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;


namespace Modules.OpenAI.Editor
{
    public class ChatUI : EditorWindow
    {
        const string uxmlPath       = "Assets/_Gpt-3/Modules/OpenAI/UI/OpenAI_visualTree.uxml";
        const string ussPath        = "Assets/_Gpt-3/Modules/OpenAI/UI/OpenAI_uss.uss";
        const string historyPath    = "Assets/_Gpt-3/Modules/OpenAI/Data/ChatHistory/Main_ChatHistory.asset";

        const string inputBoxTextFieldName  = "inputBox_textField";
        const string chatBoxScrollViewName  = "chatBox_scrollView";

        ScrollView chatBoxScrollView;
        TextField inputBoxTextField;
        string previousSender;
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
            var history = AssetDatabase.LoadAssetAtPath<ChatHistorySo>(historyPath);

            root = rootVisualElement;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            root.Add(visualTree.Instantiate());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
            root.styleSheets.Add(styleSheet);

            inputBoxTextField = root.Q<TextField>(inputBoxTextFieldName);
            chatBoxScrollView = root.Q<ScrollView>(chatBoxScrollViewName);
            chatBoxScrollView.contentContainer.RegisterCallback<GeometryChangedEvent>(ScrollToLatest);

            history.History.ForEach(AddMessage);

            inputBoxTextField.Focus();
            inputBoxTextField.RegisterCallback<KeyDownEvent>(_ =>
            {
                if (!Event.current.Equals(Event.KeyboardEvent("Return"))) return;
                if (string.IsNullOrWhiteSpace(inputBoxTextField.text)) return;

                var newMessage = new ChatMessageVo(history.CurrentUser, inputBoxTextField.text);
                AddMessage(newMessage);
                SaveHistory(history, newMessage);
                inputBoxTextField.SetValueWithoutNotify("");
                inputBoxTextField.Focus();
            });
        }

        void AddMessage (ChatMessageVo messageVo)
        {
            var continuedMessage = previousSender == messageVo.SenderName;
            previousSender = messageVo.SenderName;

            var senderName = continuedMessage ? "" : $"   {messageVo.SenderName}\n";

            var newLabel = new Label
            (
                $"<b>{senderName}</b>"
                + $"       {messageVo.Message}"
            );

            chatBoxScrollView.Add(newLabel);
        }

        void ScrollToLatest (GeometryChangedEvent data)
            => chatBoxScrollView.scrollOffset = new Vector2(0, data.newRect.height);

        void SaveHistory (ChatHistorySo history, ChatMessageVo newMessage)
        {
            history.Add(newMessage);
            EditorUtility.SetDirty(history);
            AssetDatabase.SaveAssetIfDirty(history);
            AssetDatabase.Refresh();
        }

        // todo
        // Learn how to add a pre-made visual element
        // with a title and body and uss already applied
        // So all that's needed is to pass in the name of who sent the reply
        // Learn how to capture keyboard input
        // So return button sends reply, then remove send button
        // Make the reply box grow in size as the reply length increases
        // Learn how to setup same functionality at runtime
        // so it is clear how and how to abstract the chat system
    }
}