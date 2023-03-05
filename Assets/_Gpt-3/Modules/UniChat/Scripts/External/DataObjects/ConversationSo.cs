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
		[FoldoutGroup("Settings"), TextArea(6, 6), SerializeField] string direction = "Unity 3d, Dots, UniRx";
		[FoldoutGroup("Bot Settings"), HideLabel, SerializeField] ChatBotSettingsVo botSettings;
		[HideLabel, SerializeField] HistoryVo history;

		string BotDirection => $"Your name: '{botName}', the users name: '{username}'.\n{direction}";
		public void Add (MessageVo newMessage)		=> history.Data.Add(newMessage);
		public List<MessageVo> History				=> history.Data;
		public string Username						=> username;
		public string BotName						=> botName;
		IChatBotApi api;

		[FoldoutGroup("Output Logs"), Button(ButtonSizes.Medium)]
		void OutputLogs ()
		{
			output = history.Data.Aggregate("", (current, item) => current + item.Json);
		}
		[FoldoutGroup("Output Logs"), TextArea(6, 6), SerializeField]
		string output = "";

		void OnEnable()
			=> api = new ChatBotApi();

		public async Task<string> GetAiReply(string messageText)
		{
			if (botSettings.UseChatCompletion)
			{
				return await api.GetChatReply(BotDirection, history);
			}

			return await api.GetTextReply(messageText, botSettings);
		}

		public string GetPromptJson(string sender, string message)
			=> new PromptTemplateVo()
				.AddSystem(BotDirection)
				.AddUsername(sender)
				.AddMessage(message)
				.Json();
	}
}