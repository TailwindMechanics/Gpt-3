using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;

using Modules.OpenAI.External.DataObjects;
using OpenAI_API;


namespace Modules.OpenAI.Editor
{
    public class ChatUI : EditorWindow
    {
        const string openAiCredsPath        = "Assets/_Gpt-3/GitIgnored/Credentials/OpenAI/TM_openApiCredentials.asset";
        const string conversationPath       = "Assets/_Gpt-3/Modules/OpenAI/Data/Chat/Main_conversation.asset";
        const string uxmlPath               = "Assets/_Gpt-3/Modules/OpenAI/UI/OpenAI_visualTree.uxml";
        const string ussPath                = "Assets/_Gpt-3/Modules/OpenAI/UI/OpenAI_uss.uss";
        const string inputBoxTextFieldName  = "inputBox_textField";
        const string chatBoxScrollViewName  = "chatBox_scrollView";

        OpenApiCredentialsSo openApiCredentialsSo;
        ChatMessageVisualElement activeElement;
        ScrollView chatBoxScrollView;
        TextField inputBoxTextField;
        ConversationSo conversation;


        [MenuItem("Testing/Show Window")]
        public static void ShowWindow ()
        {
            var window              = GetWindow<ChatUI>();
            window.titleContent     = new GUIContent("Chat");
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

                OnReturnKeyPress();
            });
        }

        void LoadChatHistory ()
        {
            for (var i = 0; i < conversation.History.Count; i++)
            {
                AddMessage(conversation.History[i].SenderName, conversation.History[i].Message, conversation.History[i].TimestampDateTime, i, true);
            }
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
                AddUIMessage(senderText, messageText, timestampText, isLocalUser);
                if (!reloading)
                {
                    UpdateMessageHistory();
                }
            }
            else if (reloading == false)
            {
                AppendExistingMessage(messageText, index);
            }
        }

        void OnReturnKeyPress ()
        {
            var message = inputBoxTextField.text.Trim();
            AddMessage(conversation.CurrentUser, message, DateTime.Now, conversation.LatestIndex, false);
            ResetInputBox();

            if (!conversation.ApiCallsEnabled)
            {
                Debug.Log("<color=orange><b>>>> API Disabled</b></color>");
                return;
            }

            Debug.Log("<color=cyan><b>>>> API Enabled</b></color>");
            inputBoxTextField.SetEnabled(false);
            RequestAiReply(message, aiReply =>
            {
                AddMessage("AI", aiReply, DateTime.Now, conversation.LatestIndex, false);
            });
        }

        void AddUIMessage (string senderText, string messageText, string timestampText, bool isLocalUser)
        {
            var newMessageVe = new ChatMessageVisualElement(senderText, messageText, timestampText);
            newMessageVe.ToggleAlignment(isLocalUser);
            chatBoxScrollView.Add(newMessageVe);
            activeElement = newMessageVe;
        }

        void UpdateMessageHistory()
        {
            var newMessageVo = new MessageVo(conversation.CurrentUser, inputBoxTextField.text);
            conversation.Add(newMessageVo);
            SaveHistory();
        }

        void AppendExistingMessage (string messageText, int index)
        {
            activeElement.AppendMessage($"\n{messageText}");
            conversation.AppendMessage(index, $"\n{messageText}");
            SaveHistory();
        }

        async Task RequestAiReply (string messageText, Action<string> callback)
        {
            Debug.Log("<color=cyan><b>>>> API Enabled: RequestAiReply</b></color>");
            var api = new OpenAIAPI(openApiCredentialsSo.ApiKey);
            var request = new CompletionRequest
            (
                messageText,
                Model.DavinciText,
                200,
                0.5,
                presencePenalty: 0.1,
                frequencyPenalty: 0.1
            );

            var message = "";
            await foreach (var token in api.Completions.StreamCompletionEnumerableAsync(request))
            {
                message += token.ToString();
            }

            callback.Invoke(message);
        }

        void ResetInputBox ()
        {
            inputBoxTextField.SetValueWithoutNotify("");
            inputBoxTextField.Focus();
        }

        void SaveHistory ()
        {
            EditorUtility.SetDirty(conversation);
            AssetDatabase.SaveAssetIfDirty(conversation);
            AssetDatabase.Refresh();
        }
    }
}