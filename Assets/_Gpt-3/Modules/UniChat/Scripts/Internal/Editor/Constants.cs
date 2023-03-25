namespace Modules.UniChat.Internal.Editor
{
	public static class Constants
	{
		const string ModuleRootPath					= "Assets/_Gpt-3/Modules/UniChat";
		public const string HighlightSettings		= ModuleRootPath + "/Data/HighlightSettings/Main_highlightSettings.asset";
		public const string ConversationPath		= ModuleRootPath + "/Data/Chat/Main_conversation.asset";
		public const string CodeBlockUxmlPath		= ModuleRootPath + "/UI/CodeBlock_uxml.uxml";
		public const string CodeBlockUssPath		= ModuleRootPath + "/UI/CodeBlock_uss.uss";
		public const string WindowUxmlPath			= ModuleRootPath + "/UI/OpenAI_uxml.uxml";
		public const string WindowUssPath			= ModuleRootPath + "/UI/OpenAI_uss.uss";
		public const string InputBoxTextFieldName	= "inputBox_textField";
		public const string ChatBoxScrollViewName	= "chatBox_scrollView";

		public const string MessageUxmlPath			= ModuleRootPath + "/UI/ChatMessage_uxml.uxml";
		public const string RootContainerName		= "rootContainer_visualElement";
		public const string TimestampLabelName		= "timestamp_label";
		public const string MessageLabelName		= "message_label";
		public const string SenderLabelName			= "sender_label";
	}
}