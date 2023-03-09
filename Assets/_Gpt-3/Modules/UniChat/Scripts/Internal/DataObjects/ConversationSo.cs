#if UNITY_EDITOR

using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.So;
using Modules.UniChat.External.DataObjects.Vo;
using Modules.UniChat.Internal.Apis;


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
		[FoldoutGroup("Embedding Model"), HideLabel, SerializeField] SerializableModelVo embeddingModel;
		[HideLabel, SerializeField] HistoryVo history;

		string BotDirection => $"Your name: '{botName}', the users name: '{username}'.{direction}";
		public List<MessageVo> History				=> history.Data;
		public string Username						=> username;
		public string BotName						=> botName;


		public async Task<string> GetChatBotReply(string sender, string message)
		{
			Log($"Requesting bot reply for user message: '{message}'");

			var embeddingsApi		= new EmbeddingsApi() as IEmbeddingsApi;
			var vectorDatabaseApi	= new VectorDatabaseApi(pineConeSettings.Vo) as IVectorDatabaseApi;
			var chatBotApi			= new ChatBotApi() as IChatBotApi;

			// todo IEmbeddingsApi: convert the senderMessage to a senderVector
			var senderVector = await embeddingsApi.ConvertToVector(embeddingModel.Model, sender, message, true);

			// todo IVectorDatabaseApi: query the senderVector, receive relevant contextual message IDs
			var contextMessageIds = await vectorDatabaseApi.Query(senderVector, true);

			// todo HistoryVo: retrieve context messages with IDs, and most recent messages
			var contextMessages = history.GetManyByIdList(contextMessageIds, true);
			var recentMessages = history.GetMostRecent(4, true);

			// todo IChatBotApi: send chatBot direction, context, senderMessage, get botReply
			var botReply = await chatBotApi.GetReply(message, BotDirection, contextMessages, recentMessages, true);

			// todo IEmbeddingsApi: convert the botReply to a botVector
			var botVector = await embeddingsApi.ConvertToVector(embeddingModel.Model, botName, botReply, true);

			// todo IVectorDatabaseApi: Upsert the senderVector, receive the index
			var senderMessageId = await vectorDatabaseApi.Upsert(senderVector, true);

			// todo HistoryVo: Store the senderMessage locally with its index
			history.Add(new MessageVo(senderMessageId, sender, message, false), true);

			// todo IVectorDatabaseApi: Upsert the botVector, receive the index
			var botMessageId = await vectorDatabaseApi.Upsert(botVector, true);

			// todo HistoryVo: Store the botReply locally with its index
			history.Add(new MessageVo(botMessageId, botName, botReply, true), true);

			// todo Return bots reply for display in UI
			Log($"Returning chat bot reply: '{botReply}'");
			return botReply;
		}

		void Log (string message)
			=> Debug.Log($"<color=#c49c6b><b>>>> ConversationSo: {message}</b></color>");
	}
}

#endif