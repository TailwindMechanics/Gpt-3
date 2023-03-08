#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine.Serialization;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Interfaces.New;
using Modules.UniChat.External.DataObjects.So;
using Modules.UniChat.External.DataObjects.Vo;
using Modules.UniChat.Internal.Apis;
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
		[FormerlySerializedAs("embeddingModel")] [FoldoutGroup("Embedding Model"), HideLabel, SerializeField] SerializableModelVo embeddingModel;
		[HideLabel, SerializeField] HistoryVo history;

		string BotDirection => $"Your name: '{botName}', the users name: '{username}'.{direction}";
		public void Add (MessageVo newMessage)		=> history.Data.Add(newMessage);
		public List<MessageVo> History				=> history.Data;
		public string Username						=> username;
		public string BotName						=> botName;

		// text-embedding-ada-002 has 1536 dimensions.
		// pinkish: ffb6c1
		// brownish: c49c6b
  //       IConversationHistoryManager historyManager;
		// IChatBotApi chatBotApi;


		void OnEnable()
        {
			// chatBotApi = new ChatBotApi();
   //          historyManager = new ConversationHistoryManager(embeddingModelVo.Model, chatBotApi, pineConeSettings.Vo);
        }

		public async Task<string> GetChatBotReply(string sender, string message)
		{
			// todo IEmbeddingsApi: convert the senderMessage to a senderVector
			var embeddingsApi = new EmbeddingsApi() as IEmbeddingsApi;
			var senderVector = await embeddingsApi.ConvertToVector(embeddingModel.Model, sender, message, true);

			// todo IVectorDatabaseApi: query the senderVector, receive relevant contextual message IDs
			var vectorDatabaseApi = new VectorDatabaseApi(pineConeSettings.Vo) as IVectorDatabaseApi;
			var contextMessageIds = await vectorDatabaseApi.Query(senderVector, true);

			// todo HistoryVo: retrieve context messages with IDs, and most recent messages
			var contextMessages = history.GetManyByIdList(contextMessageIds, true);
			var recentMessages = history.GetMostRecent(4, true);

			// todo IChatBotApi: send chatBot direction, context, senderMessage, get botReply
			var chatBotApi = new ChatBotApi() as IChatBotApi;
			var botReply = await chatBotApi.GetReply(message, BotDirection, contextMessages, recentMessages, true);

			// todo IEmbeddingsApi: convert the botReply to a botVector
			var botVector = await embeddingsApi.ConvertToVector(embeddingModel.Model, botName, botReply, true);

			// todo IVectorDatabaseApi
				// todo Upsert the senderVector, receive the index
				// todo Store the senderMessage locally with its index

			// todo IVectorDatabaseApi
				// todo Upsert the botVector, receive the index
				// todo Store the botReply locally with its index

			// todo Return bots reply for display in UI
			return null;
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