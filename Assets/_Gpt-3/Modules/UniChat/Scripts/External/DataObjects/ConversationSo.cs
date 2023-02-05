using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Modules.UniChat.External.DataObjects
{
	[CreateAssetMenu(fileName = "new _chatConversation", menuName = "Tailwind/Chat/Conversation")]
	public class ConversationSo : ScriptableObject
	{
		public List<ScriptableObject> GetScriptableObjects()
			=> new() {history};
		public void AppendMessage (int index, string appendage)
			=> history.Data[index - 1].AppendMessage(appendage);
		public void SetMemories ()
			=> history.Data[LatestIndex-1].SetMemories();
		public string Memories
			=> history.Data[LatestIndex-1].Memories;

		public OpenAISettingsVo OpenAISettings		=> openAISettings;
		public bool ApiCallsEnabled					=> apiCallsEnabled;
		public int LatestIndex						=> history.Data.Count;
		public string CurrentUser					=> currentUser;
		public List<MessageVo> History				=> history.Data;
		public void Add (MessageVo newMessage)		=> history.Data.Add(newMessage);

		[FoldoutGroup("Settings"), SerializeField]
		bool apiCallsEnabled;
		[FoldoutGroup("Settings"), SerializeField, ValueDropdown("$users")]
		string currentUser = "Guest";

		[FoldoutGroup("OpenAI Settings"), HideLabel, SerializeField]
		OpenAISettingsVo openAISettings;

		[InlineEditor, SerializeField]
		HistorySo history;

		public string FormatPrompt(string newUserMessage)
			=> new PromptTemplateVo()
				.AddMessage(newUserMessage)
				.AddDirection(direction)
				.AddMemory(Memories)
				.Json();

		// const string newUserMessage		= "Well actually as a starting point I'm building a chat gpt AI into the Unity editor using Unity UIToolkit to assist me in learning Dots, which is how I'm speaking to you now!";
		// const string context			= "'''The user has asked how I am this morning'''";
		// const string memory		= "''' The user greeted me with a hello, and asked how I am this morning. '''";
		const string direction	= "Direction: Please provide a response to the previous conversation and retain the important details in the \"Context and Memory\" section. Remember to separate the context and memory with the key \"===Context and Memory===\" for easier parsing later.";
		// const string direction	= "Direction:'''Your goal is to provide insightful and helpful advice on the subjects of your expertise while also retaining a working memory of the conversation with the user. Use natural language to respond to the user's message and ask questions to gather additional context if needed. Your memory section should be updated after each message exchange and should not exceed 500 tokens.'''Memory: [Insert previous conversational memory here, limited to 500 tokens]Context: [Insert context information here, including any relevant background information or context from previous exchanges]";



		// const string direction	= "''' " +
	 //        "You are Uni, an AI expert in Unity 3d, UniRx, and DOTS. Your objectives are:" +
	 //        "- Create a response for the user based on this data, make them feel remembered." +
	 //        "- Update Memory with new context based on previous Memory and your new reply (limit 500 tokens)." +
	 //        "- Place this response right at the begining of the data you return so while streamed the user sees it first" +
	 //        "- Put the the Memory value after the reply, and prefix the Memory with ___ so that I know not to display that to the user" +
	 //        "- You need to create a response in this format yourResponse ___ Memories: " +
	 //        "- Do not include any json variable names in your response or Memory" +
	 //        " '''";






		[UsedImplicitly]
		static string[] users =
		{
			"You",
			"Jodie Tollfree",
			"Marissa Poston",
			"Brad Pots",
			"Antoine Stapells",
			"Kaylin Selwood",
			"Fanny Spriggin",
			"Oliver Baylis",
			"Kathie Geffinger",
			"Austin Divis",
			"Glennery Ropartz",
			"Lonnie Parrot",
			"Babe Benoit",
			"Margaret Bag",
			"Emmanuel Northridge",
			"Alveira Challinor",
			"Wadsworth Bromwich"
		};
	}
}