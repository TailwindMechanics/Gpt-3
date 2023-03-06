using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;


namespace Modules.UniChat.External.DataObjects
{
	[CreateAssetMenu(fileName = "new _chatConversation", menuName = "Tailwind/Chat/Conversation")]
	public class ConversationSo : ScriptableObject
	{
		[FoldoutGroup("Settings"), SerializeField] string username = "Guest";
		[FoldoutGroup("Settings"), SerializeField] string botName = "Bot";
		[FoldoutGroup("Settings"), InlineEditor, SerializeField] PineConeSettingsSo pineConeSettings;
		[FoldoutGroup("Settings/Direction"), TextArea(6, 6), SerializeField] string direction = "Unity 3d, Dots, UniRx";
		[FoldoutGroup("Chat Bot Settings"), HideLabel, SerializeField] ChatBotSettingsVo botSettings;
		[FoldoutGroup("Embedding Model"), HideLabel, SerializeField] SerializableModel embeddingModel;
		[HideLabel, SerializeField] HistoryVo history;

		string BotDirection => $"Your name: '{botName}', the users name: '{username}'.\n{direction}";
		public void Add (MessageVo newMessage)		=> history.Data.Add(newMessage);
		public List<MessageVo> History				=> history.Data;
		public string Username						=> username;
		public string BotName						=> botName;

        IConversationHistoryManager historyManager;
		IChatBotApi chatBotApi;

		// text-embedding-ada-002 has 1536 dimensions.

		void OnEnable()
        {
			chatBotApi = new ChatBotApi();
            historyManager = new ConversationHistoryManager(embeddingModel.Model, chatBotApi, pineConeSettings.Vo);
        }

		public async Task<(string response, List<float> embedding)> GetAiReply(string messageText)
		{
			var conversationHistory = await historyManager.RetrieveConversationHistoryAsync(messageText, history);
			var directionWithHistory = $"{BotDirection}\nHistory: {string.Join(", ", conversationHistory.Select(x => $"({string.Join(", ", x)})"))}";
			return await chatBotApi.GetChatReply(directionWithHistory, history, embeddingModel.Model);
		}

		public string GetPromptJson(string sender, string message)
			=> new PromptTemplateVo()
				.AddSystem(BotDirection)
				.AddUsername(sender)
				.AddMessage(message)
				.Json();
	}
}