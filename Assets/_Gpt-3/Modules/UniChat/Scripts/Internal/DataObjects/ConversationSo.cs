#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine.Serialization;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.So;
using Modules.UniChat.External.DataObjects.Vo;
using Modules.UniChat.Internal.Behaviours;


namespace Modules.UniChat.Internal.DataObjects
{
	[CreateAssetMenu(fileName = "new _chatConversation", menuName = "Tailwind/Chat/Conversation")]
	public class ConversationSo : ScriptableObject
	{
		[Button(ButtonSizes.Gigantic), PropertyOrder(-1)]
		void Reset ()
		{
			history.Clear();
			EditorApplication.ExecuteMenuItem("Tailwind/Tools/Clear Console");
			EditorApplication.ExecuteMenuItem("Tailwind/UniChat");
		}


		[FoldoutGroup("Settings"), SerializeField] string username = "Guest";
		[FoldoutGroup("Settings"), SerializeField] string botName = "Bot";
		[FoldoutGroup("Settings"), InlineEditor, SerializeField] PineConeSettingsSo pineConeSettings;
		[FoldoutGroup("Settings/Direction"), TextArea(6, 6), SerializeField] string direction = "Unity 3d, Dots, UniRx";
		[FoldoutGroup("Chat Bot Settings"), HideLabel, SerializeField] ChatBotSettingsVo botSettings;
		[FormerlySerializedAs("embeddingModel")] [FoldoutGroup("Embedding Model"), HideLabel, SerializeField] SerializableModelVo embeddingModelVo;
		[HideLabel, SerializeField] HistoryVo history;

		string BotDirection => $"Your name: '{botName}', the users name: '{username}'.\n{direction}";
		public void Add (MessageVo newMessage)		=> history.Data.Add(newMessage);
		public List<MessageVo> History				=> history.Data;
		public string Username						=> username;
		public string BotName						=> botName;

		// text-embedding-ada-002 has 1536 dimensions.
        IConversationHistoryManager historyManager;
		IChatBotApi chatBotApi;


		void OnEnable()
        {
			chatBotApi = new ChatBotApi();
            historyManager = new ConversationHistoryManager(embeddingModelVo.Model, chatBotApi, pineConeSettings.Vo);
        }

		public async Task<(string response, List<float> embedding)> GetAiReply(string messageText)
		{
			var conversationHistory = await historyManager.RetrieveConversationHistoryAsync(messageText, history);
			// var directionWithHistory = $"{BotDirection}\nHistory: {string.Join(", ", conversationHistory.Select(x => $"({string.Join(", ", x)})"))}";
			// Debug.Log(directionWithHistory);
			// return await chatBotApi.GetChatReply(directionWithHistory, history, embeddingModel.Model);
			return (null, null);
		}

		public string GetPromptJson(string sender, string message)
			=> new PromptTemplateVo()
				.AddSystem(BotDirection)
				.AddUsername(sender)
				.AddMessage(message)
				.Json();
	}
}

#endif