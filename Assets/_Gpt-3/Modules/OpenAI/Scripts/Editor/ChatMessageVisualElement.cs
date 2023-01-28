using UnityEngine.UIElements;
using UnityEditor;


namespace Modules.OpenAI.Editor
{
    public class ChatMessageVisualElement : VisualElement
    {
        const string uxmlPath = "Assets/_Gpt-3/Modules/OpenAI/UI/ChatMessage.uxml";

        const string senderLabelName        = "sender_label";
        const string messageLabelName       = "message_label";
        const string timestampLabelName     = "timestamp_label";

        Label sender;
        Label message;
        Label timestamp;


        public ChatMessageVisualElement (string senderText, string messageText, string timestampText)
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            Add(visualTree.Instantiate());

            sender      = this.Q<Label>(senderLabelName);
            message     = this.Q<Label>(messageLabelName);
            timestamp   = this.Q<Label>(timestampLabelName);

            sender.text     = senderText;
            message.text    = messageText;
            timestamp.text  = timestampText;
        }
    }
}