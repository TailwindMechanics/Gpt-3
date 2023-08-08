#if UNITY_EDITOR

using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.So;
using Modules.UniChat.External.DataObjects.Vo;
using Modules.UniChat.Internal.Behaviours;
using Modules.UniChat.Internal.Apis;


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

		public async void AddUserMessage(string userName, string message)
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

		    var botPlayer           = FindObjectOfType<AiPlayer>();
		    var embeddingsApi       = new EmbeddingsApi() as IEmbeddingsApi;
		    var vectorDatabaseApi   = new VectorDatabaseApi(pineConeSettings.Vo) as IVectorDatabaseApi;
		    var chatBotApi          = new ChatBotApi() as IChatBotApi;
		    var botPerceiver        = new AiPerceiver() as IAiPerceiver;

		    var queryResult			= await DoQuery(botPlayer, botPerceiver, history.GetPreviousBotReply(), true);
		    var senderVector		= await embeddingsApi.ConvertToVector(embeddingModel.Model, sender, message, true);
		    var contextMessageIds	= await vectorDatabaseApi.Query(chatBotSettings.Vo.BotName, senderVector, chatBotSettings.Vo.MemoryAccuracy, chatBotSettings.Vo.SendSimilarChatCount, true);
		    var contextMessages		= history.GetManyByIdList(contextMessageIds, true);
		    var recentMessages		= history.GetMostRecent(chatBotSettings.Vo.SendChatHistoryCount, true);
		    var botReply			= await chatBotApi.GetReply(message, $"{BotDirection}\n{queryResult}", chatBotSettings.Vo, contextMessages, recentMessages, true);
		    var botVector			= await embeddingsApi.ConvertToVector(embeddingModel.Model, chatBotSettings.Vo.BotName, botReply, true);

		    var botMessageId = await vectorDatabaseApi.Upsert(chatBotSettings.Vo.BotName, botVector, true);
		    history.Add(new MessageVo(botMessageId, chatBotSettings.Vo.BotName, botReply, true), true);

		    DoCommand(botReply);

		    Log($"Returning chat bot reply: '{botReply}'");
		    return botReply;
		}

		void DoCommand(string botReply)
		{
			var json = ParseJsonFromAIResponse(botReply);
			if (json == null) return;

			if (json.ContainsKey("point_in_direction"))
			{
				PointInDirection(json);
			}
		}

		async Task<string> DoQuery (AiPlayer botPlayer, IAiPerceiver botPerceiver, string previousBotReply, bool logging = false)
		{
			if (string.IsNullOrWhiteSpace(previousBotReply))
			{
				if (logging)
				{
					Log("No previous bot reply to query from");
				}

				return null;
			}

			var json = ParseJsonFromAIResponse(previousBotReply);
			if (json == null) return null;

			if (json.ContainsKey("take_snapshot"))
			{
				return await TakeSnapshot(botPlayer, botPerceiver);
			}

			return null;
		}

		void PointInDirection (JObject json)
		{
			var pointer = GameObject.Find("Pointer").transform;
			var newDir = JsonConvert.DeserializeObject<PointCommand>(json.ToString());
			pointer.rotation = Quaternion.LookRotation(-newDir.Direction.Value());
			var scale = pointer.localScale;
			pointer.localScale = new Vector3(scale.x, scale.y, newDir.Direction.Value().magnitude);
		}

		async Task<string> TakeSnapshot (AiPlayer botPlayer, IAiPerceiver botPerceiver)
			=> await botPerceiver.CaptureVision(botPlayer.Camera, botPlayer.Sensor, chatBotSettings.Vo.Perception.Vo);

		JObject ParseJsonFromAIResponse(string botReply)
		{
			var split = botReply.Split("```");
			if (split.Length < 2)
			{
				Log("Could not find ```json delimiter in the AI response. Check the response format.");
				return null;
			}

			var jsonString = split[1].Replace("```", "").Replace("json", "").Trim();

			if (string.IsNullOrWhiteSpace(jsonString))
			{
				Log("Empty JSON string. Check the AI response format.");
				return null;
			}

			try
			{
				var json = JObject.Parse(jsonString);
				return json;
			}
			catch (JsonReaderException ex)
			{
				Log($"Error parsing JSON string: {ex.Message}");
				return null;
			}
		}

		[Serializable]
		public class PointCommand
		{
			[JsonProperty("point_in_direction")]
			public Vector3Serializable Direction {get;set;}
		}

		[Serializable]
		public class BotNavigateCommand
		{
			[JsonProperty("new_position")]
			public Vector3Serializable NewPosition {get;set;}
			[JsonProperty("new_rotation")]
			public Vector3Serializable NewRotation {get;set;}
		}

		void Log (string message)
			=> Debug.Log($"<color=#ECC492><b>>>> ConversationSo: {message.Replace("\n", "")}</b></color>");
	}
}

#endif