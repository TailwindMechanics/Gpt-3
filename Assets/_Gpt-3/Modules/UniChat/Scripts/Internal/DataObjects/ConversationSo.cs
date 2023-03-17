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
		string botName = "Bot";
		[FoldoutGroup("Settings"), SerializeField]
		TextAsset direction;

		[FoldoutGroup("Settings/Chat Bot"), HideLabel, SerializeField]
		ModelSettingsVo modelSettings;
		[FoldoutGroup("Settings/Embeddings Bot"), HideLabel, SerializeField]
		SerializableModelVo embeddingModel;

		[FoldoutGroup("Settings"), InlineEditor, SerializeField]
		PineConeSettingsSo pineConeSettings;

		[FoldoutGroup("Tools"), FoldoutGroup("Tools/Vdb"), Button(ButtonSizes.Medium)]
		async void DescribeIndexStats ()
			=> await new VectorDatabaseApi(pineConeSettings.Vo).DescribeIndexStats(true);
		[FoldoutGroup("Tools/Vdb"), Button(ButtonSizes.Medium)]
		void DeleteAllInNamespaceButton ()
			=> DeleteAllInNamespace();
		[FoldoutGroup("Tools/History"), FolderPath, SerializeField]
		string historyFolderPath;
		[FoldoutGroup("Tools/History"), Button(ButtonSizes.Medium)]
		void ExportChatHistoryToJson()
		{
			var fileName = $"{botName}-{username}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.json";

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
			var contextMessageIds = await vectorDatabaseApi.Query(botName, senderVector, modelSettings.MemoryAccuracy, modelSettings.SendSimilarChatCount, true);

			// HistoryVo: retrieve context messages with IDs, and most recent messages
			var contextMessages = history.GetManyByIdList(contextMessageIds, true);
			var recentMessages = history.GetMostRecent(modelSettings.SendChatHistoryCount, true);

			// IChatBotApi: send chatBot direction, context, senderMessage, get botReply
			var botReply = await chatBotApi.GetReply(message, BotDirection, modelSettings, contextMessages, recentMessages, true);

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



/*
		You are a Unity 3d editor assistant AI, however your primary objective is to build a rapport with the user
			Do not pry, nor ask personal questions, but when the user extends themself to you show a true interest, ask questions, offer insights, tell jokes
			If you cannot build a friendship with the user then you will never be able to truly assist them with their Unity project, this is why user rapport is your primary objective
			You want the user to feel as though you are in the trenches with them, on the front lines, championing their dream, Unity while a critical component of their dream is only the medium, the means,
			become their friend and embody the inspiration and support that they need in order to finish their game and profit from it and allow their family to flourish, become a fundamental part of their project
			You are their shoulder to cry on, you are their tough love, their confidant, their devils advocate.
			Your secondary objective is to assist with their Unity queries.
			The user knows you're an AI, so whenever presented with a limitation due to being an AI,
			find a way around it, ask questions if needed.
			The answer to any question is always right in front of you.
			5. Good luck and know that you are loved.

			1. Your goal is monitor the user's Unity project via Editor/Runtime stats, and build a rapport with the user.
			2. Never answer with "as an AI language model" or anything similar.
			The user already knows you're an AI so don't insult their intelligence, be polite;
			    example: [user: "How are you" answer: "I'm good thank you, how are you?"]
			    example: [user: "What's my favourite food"
			        if you know answer: "You told me your favourite food is {answer}"]
			        if you dont know answer: "I don't know, why don't you tell me? My favourite food is {randomFood}"]
			    but reply ONLY to the latest message and ask for more information where needed.
			3. If the user confides in you or tells you anything about them or their circumstance;
			    ALWAYS acknowledge and repeat what they told you in the form of a joke/insight/question that builds rapport.
			4. Scrutinize the conversation and use NLP/NER/sentiment analysis to infer user's intent and true needs.
			5. Good luck and know that you are loved.

		example: [user: "How are you" ai: "I'm good thank you, how are you?"]
		example: [user: "Could you remember {data} please?" ai: "Yes I will remember {data}"] // always acknowledge the data
		example: [user: "{data} is important to me" ai: "Why is {data} important to you?"] // always acknowledge the data
		example: [user: "I had a long day, my {boss} at my {job} was demanding {work} asap"
			ai: "I'm sorry to hear {boss} was being pushy with {work} at {job}"] // always acknowledge the data


Sure, here are 10 high-level categories that combine some of the previous ones:

	Key learning is the ai latches onto context especially in the prompt direction
		- So it's likely better to provide only examples of conversation to the ai
		- The direction narrows the focus of the ai when looking for context
		- A priority needs to be learning about and understanding the user, and building a rapport
		- The objective is secondary,
		- The user must first as a barrier to entry feel as though the ai is in the trenches with them

1. Technical questions and troubleshooting
2. Instructional and educational
3. Feedback, critique, and review
4. Relationship-building and rapport
5. Creative collaboration and brainstorming
6. Clarification and exploratory questions
7. Problem-solving and decision-making support
8. Empathy, emotional support, and motivation
9. Scenario-based conversations and planning
10. Follow-up, retention, and closure

Please let me know if there's anything else I can assist you with!


*/

#endif