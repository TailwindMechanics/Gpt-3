using UnityEngine.UIElements;
using UnityEditor;


namespace Modules.OpenAI.Editor
{
    public class ChatMessageVisualElement : VisualElement
    {
        const string uxmlPath = "Assets/_Gpt-3/Modules/OpenAI/UI/ChatMessage_uxml.uxml";

        const string rootContainerName      = "rootContainer_visualElement";
        const string timestampLabelName     = "timestamp_label";
        const string messageLabelName       = "message_label";
        const string senderLabelName        = "sender_label";

        readonly VisualElement root;
        readonly Label sender;
        readonly Label message;
        readonly Label timestamp;


        public ChatMessageVisualElement (string senderText, string messageText, string timestampText)
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            Add(visualTree.Instantiate());

            root        = this.Q<VisualElement>(rootContainerName);
            sender      = this.Q<Label>(senderLabelName);
            message     = this.Q<Label>(messageLabelName);
            timestamp   = this.Q<Label>(timestampLabelName);

            sender.text     = senderText;
            message.text    = messageText;
            timestamp.text  = timestampText;

            if (string.IsNullOrWhiteSpace(senderText)) sender.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            if (string.IsNullOrWhiteSpace(timestampText)) timestamp.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        public void ToggleAlignment (bool isLeft)
        {
            root.style.marginLeft   = isLeft ? 6 : 60;
            root.style.marginRight  = !isLeft ? 6 : 60;
            sender.style.alignSelf  = new StyleEnum<Align>(isLeft ? Align.FlexStart : Align.FlexEnd);
        }

        public void AppendMessage(string appendage)
            => message.text += appendage;
    }
}