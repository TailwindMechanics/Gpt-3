namespace Modules.UniChat.Internal.Editor
{
	public static class Constants
	{
		const string moduleRootPath					= "Assets/_Gpt-3/Modules/UniChat";

		public const string ConversationPath		= moduleRootPath + "/Data/Chat/Main_conversation.asset";
		public const string WindowUxmlPath			= moduleRootPath + "/UI/OpenAI_uxml.uxml";
		public const string WindowUssPath			= moduleRootPath + "/UI/OpenAI_uss.uss";
		public const string InputBoxTextFieldName	= "inputBox_textField";
		public const string ChatBoxScrollViewName	= "chatBox_scrollView";

		public const string MessageUxmlPath			= moduleRootPath + "/UI/ChatMessage_uxml.uxml";
		public const string RootContainerName		= "rootContainer_visualElement";
		public const string TimestampLabelName		= "timestamp_label";
		public const string MessageLabelName		= "message_label";
		public const string SenderLabelName			= "sender_label";
	}
}