#if UNITY_EDITOR

using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using System;

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
			DeleteAllInNamespace();
		}

		[FoldoutGroup("Settings"), Range(0f, 1f), SerializeField]
		float memoryAccuracy = 0.8f;
		[FoldoutGroup("Settings"), SerializeField]
		bool upsertUserLog;
		[FoldoutGroup("Settings"), SerializeField]
		string username = "Guest";
		[FoldoutGroup("Settings"), SerializeField]
		string botName = "Bot";
		[FoldoutGroup("Settings"), SerializeField]
		TextAsset direction;
		[FoldoutGroup("Settings"), InlineEditor, SerializeField]
		PineConeSettingsSo pineConeSettings;

		[FoldoutGroup("Embedding Model"), HideLabel, SerializeField]
		SerializableModelVo embeddingModel;

		[FoldoutGroup("Vdb Tools"), SerializeField]
		Color color;
		[FoldoutGroup("Vdb Tools"), Button(ButtonSizes.Medium)]
		async void DescribeIndexStats ()
			=> await new VectorDatabaseApi(pineConeSettings.Vo).DescribeIndexStats(true);
		[FoldoutGroup("Vdb Tools"), Button(ButtonSizes.Medium)]
		async void DeleteAllInNamespace ()
			=> await new VectorDatabaseApi(pineConeSettings.Vo).DeleteAllVectorsInNamespace(botName, true);

		[HideLabel, SerializeField]
		HistoryVo history;

		string BotDirection => $"Your name: '{botName}', the users name: '{username}'.{direction.text}";
		public List<MessageVo> History				=> history.Data;
		public string Username						=> username;
		public string BotName						=> botName;


		// todo Next thing to do is Temporal memories:
			// todo when a match is found retrieve the message prev and next, or some combination
			// or perhaps retrieve chunks to be send to a cheaper ai to summarise
			// internal monologue
		public async Task<string> GetChatBotReply(string sender, string message)
		{
			Log($"Requesting bot reply for user message: '{message}'");

			var embeddingsApi		= new EmbeddingsApi() as IEmbeddingsApi;
			var vectorDatabaseApi	= new VectorDatabaseApi(pineConeSettings.Vo) as IVectorDatabaseApi;
			var chatBotApi			= new ChatBotApi() as IChatBotApi;

			// IEmbeddingsApi: convert the senderMessage to a senderVector
			var senderVector = await embeddingsApi.ConvertToVector(embeddingModel.Model, sender, message, true);

			// IVectorDatabaseApi: query the senderVector, receive relevant contextual message IDs
			var contextMessageIds = await vectorDatabaseApi.Query(botName, senderVector, memoryAccuracy, true);

			// HistoryVo: retrieve context messages with IDs, and most recent messages
			var contextMessages = history.GetManyByIdList(contextMessageIds, true);
			var recentMessages = history.GetMostRecent(4, true);

			// IChatBotApi: send chatBot direction, context, senderMessage, get botReply
			var botReply = await chatBotApi.GetReply(message, BotDirection, contextMessages, recentMessages, true);

			// IEmbeddingsApi: convert the botReply to a botVector
			var botVector = await embeddingsApi.ConvertToVector(embeddingModel.Model, botName, botReply, true);

			// IVectorDatabaseApi: Upsert the senderVector, receive the index
			var senderId = Guid.Empty;
			if (upsertUserLog) senderId = await vectorDatabaseApi.Upsert(botName, senderVector, true);

			// HistoryVo: Store the senderMessage locally with its index
			history.Add(new MessageVo(senderId, sender, message, false), true);

			// IVectorDatabaseApi: Upsert the botVector, receive the index
			var botMessageId = await vectorDatabaseApi.Upsert(botName, botVector, true);
			// HistoryVo: Store the botReply locally with its index
			history.Add(new MessageVo(botMessageId, botName, botReply, true), true);

			// Return bots reply for display in UI
			Log($"Returning chat bot reply: '{botReply}'");
			return botReply;
		}

		void Log (string message)
			=> Debug.Log($"<color=#ECC492><b>>>> ConversationSo: {message.Replace("\n", "")}</b></color>");
	}
}

#endif