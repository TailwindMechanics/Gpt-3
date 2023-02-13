using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using System.IO;


namespace Modules.UniChat.External.DataObjects
{
	[CreateAssetMenu(fileName = "new _chatConversation", menuName = "Tailwind/Chat/Conversation")]
	public class ConversationSo : ScriptableObject
	{
		public List<ScriptableObject> GetScriptableObjects()
			=> new() {history};
		public void AppendMessage (int index, string appendage)
			=> history.Data[index - 1].AppendMessage(appendage);
		public void SetMemories (string message)
		{
			var split		= message.Split("|.Memory.|");
			var response	= split[0].TrimEnd();
			var memory		= split[1].TrimStart();

			File.WriteAllText(AssetDatabase.GetAssetPath(workingMemory), memory);
			EditorUtility.SetDirty(workingMemory);
		}

		public MessageVo GetMostRecentReceived ()
		{
			for (var i = history.Data.Count - 1; i >= 0; i--)
			{
				if (history.Data[i].SenderName == CurrentUser) continue;

				return history.Data[i];
			}

			return null;
		}

		public string AiName						=> aiName;
		public OpenAISettingsVo OpenAISettings		=> openAISettings;
		public bool ApiCallsEnabled					=> apiCallsEnabled;
		public int LatestIndex						=> history.Data.Count;
		public string CurrentUser					=> currentUser;
		public List<MessageVo> History				=> history.Data;
		public void Add (MessageVo newMessage)		=> history.Data.Add(newMessage);

		[FoldoutGroup("Settings"), SerializeField]
		bool apiCallsEnabled;
		[FoldoutGroup("Settings"), SerializeField]
		string aiName = "Uni";
		[FoldoutGroup("Settings"), SerializeField]
		string currentUser = "Guest";
		[FoldoutGroup("Settings"), SerializeField]
		TextAsset direction;
		[FoldoutGroup("Settings"), SerializeField]
		TextAsset workingMemory;

		[FoldoutGroup("OpenAI Settings"), HideLabel, SerializeField]
		OpenAISettingsVo openAISettings;

		[InlineEditor, SerializeField]
		HistorySo history;


		public string FormatPrompt(string newUserMessage)
		{
			return new PromptTemplateVo()
				.AddUsername(CurrentUser)
				.AddMessage(newUserMessage)
				.AddDirection(direction.text)
				.AddMemory(workingMemory.text)
				.Json();
		}
	}
}