using UnityEditor;
using UnityEngine.UIElements;

namespace Modules.UniChat.Editor
{
    public class ChatMessageVisualElement : VisualElement
    {
        readonly VisualElement root;
        readonly Label timestamp;
        readonly Label sender;
        readonly Label message;


        public ChatMessageVisualElement (string senderText, string messageText, string timestampText)
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Constants.MessageUxmlPath);
            Add(visualTree.Instantiate());

            root        = this.Q<VisualElement>(Constants.RootContainerName);
            sender      = this.Q<Label>(Constants.SenderLabelName);
            message     = this.Q<Label>(Constants.MessageLabelName);
            timestamp   = this.Q<Label>(Constants.TimestampLabelName);

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