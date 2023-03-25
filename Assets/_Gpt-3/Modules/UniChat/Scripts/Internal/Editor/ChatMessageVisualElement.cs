#if UNITY_EDITOR

using UnityEngine.UIElements;
using UnityEditor;
using System;
using Modules.SyntaxHighlighter.Internal.DataObjects;
using Highlighter = Modules.SyntaxHighlighter.External.Behaviours.Highlighter;


namespace Modules.UniChat.Internal.Editor
{
    public class ChatMessageVisualElement : VisualElement
    {
        readonly HighlightSettingsSo highlightSettings;
        readonly VisualElement root;
        readonly Label timestamp;
        readonly Label sender;
        readonly Label message;


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
            root.style.marginLeft   = isLeft ? 6 : 60;
            root.style.marginRight  = !isLeft ? 6 : 60;
            sender.style.alignSelf  = new StyleEnum<Align>(isLeft ? Align.FlexStart : Align.FlexEnd);
        }

        public void AppendMessage(string appendage)
        {
            message.text += appendage;
            message.text = message.text.Replace("#Response", "").TrimStart();
        }

        void ParseMessage(string messageText)
        {
            const string codeBlockDelimiter = "```";
            var parts = messageText.Split(new[] { codeBlockDelimiter }, StringSplitOptions.None);

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

            for (var i = 0; i < parts.Length; i++)
            {
                if (i % 2 == 0)
                {
                    var label = new Label(parts[i])
                    {
                        style =
                        {
                            unityFont = message.style.unityFont,
                            fontSize = message.style.fontSize,
                            color = message.style.color,
                            display = DisplayStyle.Flex,
                            whiteSpace = WhiteSpace.Normal
                        }
                    };

                    textContainer.Add(label);
                }
                else
                {
                    var codeText = Highlighter.Highlight(parts[i], highlightSettings.Vo)
                        .Replace("csharp", "")
                        .Replace("json", "")
                        .Trim();

                    var codeBlock = new CodeBlockVisualElement(codeText);
                    textContainer.Add(codeBlock);
                }
            }
        }
    }
}

#endif