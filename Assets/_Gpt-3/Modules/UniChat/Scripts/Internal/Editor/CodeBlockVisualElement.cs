using System.Text.RegularExpressions;
using UnityEngine.UIElements;
using UnityEditor;

using Highlighter = Modules.SyntaxHighlighter.External.Behaviours.Highlighter;
using Modules.SyntaxHighlighter.Internal.DataObjects;


namespace Modules.UniChat.Internal.Editor
{
	public class CodeBlockVisualElement : VisualElement
	{
		readonly string originalCode;
		readonly string language;


		public CodeBlockVisualElement(string codeText, HighlightSettingsVo highlightSettings)
		{
			foreach (var lang in Highlighter.Languages)
			{
				originalCode = codeText.Replace(lang, "");
			}
			originalCode = codeText.Trim();

			var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Constants.CodeBlockUxmlPath);
			Add(visualTree.Instantiate());

			var languagesPattern = string.Join("|", Highlighter.Languages);
			var languageMatch = Regex.Match(codeText, $"^({languagesPattern})\\s+");
			if (languageMatch.Success)
			{
				language = languageMatch.Value.Trim();
			}

			var codeLabel = this.Q<Label>("codeBlock_label");
			codeText = Highlighter.Highlight(codeText, highlightSettings);

			codeLabel.text = codeText.Replace("\\n", "\n");

			var copyButton = this.Q<Button>("copyButton");
			copyButton.clicked += () => CopyCode(copyButton);

			var languageLabel = this.Q<Label>("language_label");
			languageLabel.text = language;
		}

		void CopyCode(Button copyButton)
		{
			EditorGUIUtility.systemCopyBuffer = originalCode;

			copyButton.text = "✓ Copied!";
			copyButton.schedule.Execute(() => copyButton.text = "Copy code").StartingIn(3000);
		}
	}
}