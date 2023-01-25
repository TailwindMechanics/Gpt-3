using System;
using System.Threading.Tasks;
using Label = UnityEngine.UIElements.Label;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;


namespace Modules.OpenAI.Editor
{
    public class ChatUI : EditorWindow
    {
        const string uxmlPath   = "Assets/_Gpt-3/Modules/OpenAI/UI/OpenAI_visualTree.uxml";
        const string ussPath    = "Assets/_Gpt-3/Modules/OpenAI/UI/OpenAI_uss.uss";

        const string inputBoxTextFieldName  = "inputBox_textField";
        const string chatBoxScrollViewName  = "chatBox_scrollView";

        ScrollView chatBoxScrollView;
        TextField inputBoxTextField;
        VisualElement root;

        // todo
        // Learn how to add a pre-made visual element
            // with a title and body and uss already applied
            // So all that's needed is to pass in the name of who sent the reply
        // Learn how to capture keyboard input
            // So return button sends reply, then remove send button
        // Make the reply box grow in size as the reply length increases
        // Learn how to setup same functionality at runtime
            // so it is clear how and how to abstract the chat system

        [MenuItem("Testing/Show Window")]
        public static void ShowWindow ()
        {
            var window = GetWindow<ChatUI>();
            window.titleContent = new GUIContent("Chat");
            window.minSize = new Vector2(100, 100);
        }

        void CreateGUI()
        {
            root = rootVisualElement;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            root.Add(visualTree.Instantiate());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
            root.styleSheets.Add(styleSheet);

            inputBoxTextField   = root.Q<TextField>(inputBoxTextFieldName);
            chatBoxScrollView   = root.Q<ScrollView>(chatBoxScrollViewName);

            inputBoxTextField.RegisterCallback<KeyDownEvent>(keyEvent =>
            {
                if (Event.current.Equals(Event.KeyboardEvent("Return")))
                {
                    chatBoxScrollView.Add(new Label(inputBoxTextField.text));
                    inputBoxTextField.SetValueWithoutNotify("");
                }
            });
        }
    }
}