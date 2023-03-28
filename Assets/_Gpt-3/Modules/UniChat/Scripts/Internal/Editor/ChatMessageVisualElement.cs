#if UNITY_EDITOR

using UnityEngine.UIElements;
using UnityEditor;
using System;

using Modules.SyntaxHighlighter.Internal.DataObjects;


namespace Modules.UniChat.Internal.Editor
{
    public class ChatMessageVisualElement : VisualElement
    {
        readonly HighlightSettingsSo highlightSettings;
        readonly VisualElement root;
        readonly Label timestamp;
        readonly Label sender;
        readonly Label message;
        const int Indent = 25;


        public ChatMessageVisualElement (string senderText, string messageText, string timestampText)
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Constants.MessageUxmlPath);
            Add(visualTree.Instantiate());
            highlightSettings = AssetDatabase.LoadAssetAtPath<HighlightSettingsSo>(Constants.HighlightSettings);

            root        = this.Q<VisualElement>(Constants.RootContainerName);
            sender      = this.Q<Label>(Constants.SenderLabelName);
            message     = this.Q<Label>(Constants.MessageLabelName);
            timestamp   = this.Q<Label>(Constants.TimestampLabelName);

            sender.text     = senderText;
            ParseMessage(messageText);
            timestamp.text  = timestampText;

            if (string.IsNullOrWhiteSpace(senderText)) sender.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            if (string.IsNullOrWhiteSpace(timestampText)) timestamp.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        public void ToggleAlignment (bool isLeft)
        {
            root.style.marginLeft   = isLeft ? 6 : Indent;
            root.style.marginRight  = !isLeft ? 6 : Indent;
            sender.style.alignSelf  = new StyleEnum<Align>(isLeft ? Align.FlexStart : Align.FlexEnd);
        }

        public void AppendMessage(string appendage)
        {
            message.text += appendage;
            message.text = message.text.Replace("#Response", "").TrimStart();
        }

        void ParseMessage(string messageText)
        {
            var textContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    flexGrow = 1,
                    flexShrink = 1
                }
            };
            message.Add(textContainer);

            const string codeBlockDelimiter = "```";
            if (!messageText.Contains(codeBlockDelimiter))
            {
                var label = new Label(messageText)
                {
                    style =
                    {
                        unityFont = message.style.unityFont,
                        fontSize = message.style.fontSize,
                        color = message.style.color,
                        display = DisplayStyle.Flex,
                        whiteSpace = WhiteSpace.Normal,
                        marginTop = 5,
                        marginBottom = 0
                    }
                };

                textContainer.Add(label);
                return;
            }

            var parts = messageText.Split(new[] { codeBlockDelimiter }, StringSplitOptions.None);
            for (var i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();
                if (i % 2 == 0) continue;

                var codeBlock = new CodeBlockVisualElement(parts[i], highlightSettings.Vo);
                textContainer.Add(codeBlock);
            }
        }
    }
}

#endif