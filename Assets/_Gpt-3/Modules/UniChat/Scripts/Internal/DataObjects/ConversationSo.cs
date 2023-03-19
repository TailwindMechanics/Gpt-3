#if UNITY_EDITOR

using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.So;
using Modules.UniChat.External.DataObjects.Vo;
using Modules.UniChat.Internal.Apis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


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
		bool upsertUserLog;
		[FoldoutGroup("Settings"), SerializeField]
		string username = "Guest";
		[FoldoutGroup("Settings"), SerializeField]
		TextAsset direction;

		[InlineEditor, SerializeField]
		ModelSettingsSo modelSettings;
		[FoldoutGroup("Settings/Embeddings Bot"), HideLabel, SerializeField]
		SerializableModelVo embeddingModel;

		[FoldoutGroup("Settings"), InlineEditor, SerializeField]
		PineConeSettingsSo pineConeSettings;

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
			var fileName = $"{modelSettings.Vo.BotName}-{username}_{DateTime.Now:yyyyMMdd_HHmmss}.json";

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
			=> await new VectorDatabaseApi(pineConeSettings.Vo).DeleteAllVectorsInNamespace(modelSettings.Vo.BotName, true);

		[HideLabel, SerializeField]
		HistoryVo history;

		string BotDirection => $"Your name: '{modelSettings.Vo.BotName}', the users name: '{username}'.{direction.text}";
		public List<MessageVo> History				=> history.Data;
		public string Username						=> username;
		public string BotName						=> modelSettings.Vo.BotName;


		public async Task<string> GetChatBotReply(string sender, string message)
		{
			Log($"Requesting bot reply for user message: '{message}'");

			var embeddingsApi		= new EmbeddingsApi() as IEmbeddingsApi;
			var vectorDatabaseApi	= new VectorDatabaseApi(pineConeSettings.Vo) as IVectorDatabaseApi;
			var chatBotApi			= new ChatBotApi() as IChatBotApi;

			// IEmbeddingsApi: convert the senderMessage to a senderVector
			var senderVector = await embeddingsApi.ConvertToVector(embeddingModel.Model, sender, message, true);

			// IVectorDatabaseApi: query the senderVector, receive relevant contextual message IDs
			var contextMessageIds = await vectorDatabaseApi.Query(modelSettings.Vo.BotName, senderVector, modelSettings.Vo.MemoryAccuracy, modelSettings.Vo.SendSimilarChatCount, true);

			// HistoryVo: retrieve context messages with IDs, and most recent messages
			var contextMessages = history.GetManyByIdList(contextMessageIds, true);
			var recentMessages = history.GetMostRecent(modelSettings.Vo.SendChatHistoryCount, true);

			// IChatBotApi: send chatBot direction, context, senderMessage, get botReply
			var botReply = await chatBotApi.GetReply(message, BotDirection, modelSettings.Vo, contextMessages, recentMessages, true);

			// IEmbeddingsApi: convert the botReply to a botVector
			var botVector = await embeddingsApi.ConvertToVector(embeddingModel.Model, modelSettings.Vo.BotName, botReply, true);

			// IVectorDatabaseApi: Upsert the senderVector, receive the index
			var senderId = Guid.Empty;
			if (upsertUserLog) senderId = await vectorDatabaseApi.Upsert(modelSettings.Vo.BotName, senderVector, true);

			// HistoryVo: Store the senderMessage locally with its index
			history.Add(new MessageVo(senderId, sender, message, false), true);

			// IVectorDatabaseApi: Upsert the botVector, receive the index
			var botMessageId = await vectorDatabaseApi.Upsert(modelSettings.Vo.BotName, botVector, true);
			// HistoryVo: Store the botReply locally with its index
			history.Add(new MessageVo(botMessageId, modelSettings.Vo.BotName, botReply, true), true);

			// Detect command
			DoCommand(botReply);

			// Return bots reply for display in UI
			Log($"Returning chat bot reply: '{botReply}'");
			return botReply;
		}

		void DoCommand(string botReply)
		{
			// var botReply = "```json{ \"new_position\": { \"x\": -5.0, \"y\": 1.6, \"z\": 55.0 }, \"new_rotation\": { \"x\": 0, \"y\": 240, \"z\": 0 }}```";

			var json = ParseJsonFromAIResponse(botReply);
			if (json == null) return;

			var newPosition = json["new_position"]?.ToObject<Vector3Serializable>();
			var newRotation = json["new_rotation"]?.ToObject<Vector3Serializable>();

			Debug.Log($"New position: {newPosition}");
			Debug.Log($"New rotation: {newRotation}");

			Log("Moving");
			var brainwave = GameObject.Find("Brainwave").transform;
			brainwave.position = newPosition.Value;
			brainwave.eulerAngles = newRotation.Value;
		}

		JObject ParseJsonFromAIResponse(string botReply)
		{
			var split = botReply.Split("```");
			if (split.Length < 2)
			{
				Debug.LogError("Could not find ```json delimiter in the AI response. Check the response format.");
				return null;
			}

			var jsonString = split[1].Replace("```", "").Replace("json", "").Trim();

			if (string.IsNullOrWhiteSpace(jsonString))
			{
				Debug.LogError("Empty JSON string. Check the AI response format.");
				return null;
			}

			try
			{
				var json = JObject.Parse(jsonString);
				return json;
			}
			catch (JsonReaderException ex)
			{
				Debug.LogError($"Error parsing JSON string: {ex.Message}");
				return null;
			}
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