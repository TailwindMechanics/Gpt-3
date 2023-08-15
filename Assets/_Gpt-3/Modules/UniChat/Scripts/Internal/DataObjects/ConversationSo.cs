#if UNITY_EDITOR

using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.Internal.DataObjects.Schemas;
using Modules.UniChat.External.DataObjects.So;
using Modules.UniChat.External.DataObjects.Vo;
using Modules.UniChat.Internal.Behaviours;
using Modules.UniChat.Internal.Apis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI.Chat;
using UniRx;
using UnityEngine.Serialization;


namespace Modules.UniChat.Internal.DataObjects
{
	[CreateAssetMenu(fileName = "new _chatConversation", menuName = "Tailwind/Chat/Conversation")]
	public class ConversationSo : ScriptableObject
	{
		[Button(ButtonSizes.Gigantic), PropertyOrder(-1)]
		async void Reset ()
		{
			history.Clear();
			EditorApplication.ExecuteMenuItem("Tailwind/Tools/Clear Console");
			EditorApplication.ExecuteMenuItem("Tailwind/UniChat");
			await DeleteAllInNamespace();
			DescribeIndexStats();
		}

		[FoldoutGroup("Settings"), SerializeField]
		string username = "Guest";

		[InlineEditor, SerializeField]
		ModelSettingsSo chatBotSettings;
		[FoldoutGroup("Settings/Embeddings Bot"), HideLabel, SerializeField]
		SerializableModelVo embeddingModel;

		[FoldoutGroup("Settings"), InlineEditor, SerializeField]
		PineConeSettingsSo pineConeSettings;
		[FoldoutGroup("Settings"), InlineEditor, SerializeField]
		WebSearchSettingsSo webSearchSettings;
		[FoldoutGroup("Settings"), InlineEditor, SerializeField]
		GoogleCloudVisionSettingsSo cloudVisionCreds;

		[FoldoutGroup("Tools"), FoldoutGroup("Tools/Vdb"), Button(ButtonSizes.Medium)]
		async void DescribeIndexStats ()
			=> await new VectorDatabaseApi(pineConeSettings.Vo).DescribeIndexStats(true);
		[FoldoutGroup("Tools/Vdb"), Button(ButtonSizes.Medium)]
		async void DeleteAllInNamespaceButton ()
			=> await DeleteAllInNamespace();
		[FoldoutGroup("Tools/History"), FolderPath, SerializeField]
		string historyFolderPath;
		[FoldoutGroup("Tools/History"), Button(ButtonSizes.Medium)]
		void ExportChatHistoryToJson()
		{
			var fileName = $"{chatBotSettings.Vo.BotName}-{username}_{DateTime.Now:yyyyMMdd_HHmmss}.json";

			var path = Path.Combine(historyFolderPath, fileName);
			var json = JsonUtility.ToJson(history, true);
			File.WriteAllText(path, json);

			AssetDatabase.Refresh();

			var exportedAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
			EditorGUIUtility.PingObject(exportedAsset);
			Selection.activeObject = exportedAsset;

			Log($"Chat history exported to {path}");
		}

		async Task DeleteAllInNamespace ()
		{
			if (chatBotSettings == null) return;

			await new VectorDatabaseApi(pineConeSettings.Vo).DeleteAllVectorsInNamespace(chatBotSettings.Vo.BotName, true);
		}

		[HideLabel, SerializeField]
		HistoryVo history;

		string BotDirection => $"Your name: '{chatBotSettings.Vo.BotName}', the users name: '{username}'. {chatBotSettings.Vo.Direction}";
		public List<MessageVo> History	=> history.Data;
		public string Username			=> username;
		public string BotName			=> chatBotSettings != null ? chatBotSettings.Vo.BotName : null;


		public async Task<string> GetSearchBotReply (string botName, string message)
		{
			var api = new WebSearchSummaryApi(webSearchSettings.Vo, true) as IWebSearchSummaryApi;
			var botReply = await api.SearchAndGetSummary(message);

			var botMessageId = Guid.NewGuid();
			if (chatBotSettings != null)
			{
				var embeddingsApi = new EmbeddingsApi() as IEmbeddingsApi;
				var vectorDatabaseApi = new VectorDatabaseApi(pineConeSettings.Vo) as IVectorDatabaseApi;
				var botVector = await embeddingsApi.ConvertToVector(embeddingModel.Model, botName, botReply, true);
				botMessageId = await vectorDatabaseApi.Upsert(botName, botVector, true);
			}

			history.Add(new MessageVo(botMessageId, botName, botReply, true), true);

			return botReply;
		}

		async void AddUserMessage(string userName, string message)
		{
			var userMessageId = Guid.NewGuid();
			if (chatBotSettings != null)
			{
				var embeddingsApi = new EmbeddingsApi() as IEmbeddingsApi;
				var vectorDatabaseApi = new VectorDatabaseApi(pineConeSettings.Vo) as IVectorDatabaseApi;

				var userVector = await embeddingsApi.ConvertToVector(embeddingModel.Model, userName, message, true);
				userMessageId = await vectorDatabaseApi.Upsert(userName, userVector, true);
			}

			history.Add(new MessageVo(userMessageId, userName, message, false), true);
		}

		public async Task<string> GetChatBotReply(string sender, string message)
		{
		    if (chatBotSettings == null) return null;

		    Log($"Requesting bot reply for user message: '{message}'");

		    var agentPlayer = FindObjectOfType<AiPlayer>();
		    var embeddingsApi       = new EmbeddingsApi() as IEmbeddingsApi;
		    var vectorDatabaseApi   = new VectorDatabaseApi(pineConeSettings.Vo) as IVectorDatabaseApi;
		    var chatBotApi          = new ChatBotApi() as IChatBotApi;
		    var agentPerceiver      = new AiPerceiver(cloudVisionCreds.Vo) as IAiPerceiver;
		    var sightData			= await agentPerceiver.CaptureVision(agentPlayer.Camera, chatBotSettings.Vo.Perception.Vo);
		    var contextMessages		= new List<MessageVo>();

		    if (chatBotSettings.Vo.SendSimilarChatCount > 0)
		    {
			    var senderVector = await embeddingsApi.ConvertToVector(embeddingModel.Model, sender, message, true);
			    var contextMessageIds = await vectorDatabaseApi.Query(chatBotSettings.Vo.BotName, senderVector, chatBotSettings.Vo.MemoryAccuracy, chatBotSettings.Vo.SendSimilarChatCount, true);
			    contextMessages = history.GetManyByIdList(contextMessageIds, true);
		    }

			var recentMessages = history.GetMostRecent(chatBotSettings.Vo.SendChatHistoryCount, true);
			var functions = new List<Function>
			{
				new GoToPositionSchema().Function(),
				new TurnBySchema().Function()
			};

			AddUserMessage(sender, message);

			var agentReply = await chatBotApi.GetReply(sender, message, sightData, BotDirection, chatBotSettings.Vo, contextMessages, recentMessages, functions, true);

			var botReply = agentReply.Message.Content;
			if (!string.IsNullOrWhiteSpace(agentReply.Message.Content))
			{
			    var botVector = await embeddingsApi.ConvertToVector(embeddingModel.Model, chatBotSettings.Vo.BotName, botReply, true);
			    var botMessageId = await vectorDatabaseApi.Upsert(chatBotSettings.Vo.BotName, botVector, true);
				history.Add(new MessageVo(botMessageId, chatBotSettings.Vo.BotName, botReply, true), true);
			}
			if (agentReply.Function != null)
			{
				if (string.IsNullOrWhiteSpace(botReply))
				{
					var args = JObject.Parse(agentReply.Function.Arguments.ToString());
					var formattedArgs = string.Join(", ", args.Values().Select(v => v is JObject ? string.Join(", ", v.Values()) : v.ToString()));
					botReply = $"<color=#ECC492><b>... {agentReply.Function.Name}({formattedArgs})...</b></color>";
					history.Add(new MessageVo(new Guid(), chatBotSettings.Vo.BotName, botReply, true), true);
				}

				agentPlayer.OnFunctionReceived(agentReply.Function, chatBotSettings.Vo);
			}


		    Log($"Returning chat bot reply: '{botReply}'");
		    return botReply;
		}

		void Log (string message)
			=> Debug.Log($"<color=#ECC492><b>>>> ConversationSo: {message.Replace("\n", "")}</b></color>");
	}
}

#endif