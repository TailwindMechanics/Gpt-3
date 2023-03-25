using UnityEngine.UIElements;
using UnityEditor;


namespace Modules.UniChat.Internal.Editor
{
	public class CodeBlockVisualElement : VisualElement
	{
		public CodeBlockVisualElement(string codeText)
		{
			var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(Constants.CodeBlockUxmlPath);
			Add(visualTree.Instantiate());

			var codeLabel = this.Q<Label>("codeBlock_label");
			codeLabel.text = codeText.Replace("\\n", "\n");
		}
	}
}