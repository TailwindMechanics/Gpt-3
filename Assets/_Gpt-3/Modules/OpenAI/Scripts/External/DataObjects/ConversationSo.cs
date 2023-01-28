using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Modules.OpenAI.External.DataObjects
{
	[CreateAssetMenu(fileName = "new _chatConversation", menuName = "Tailwind/Chat/Conversation")]
	public class ConversationSo : ScriptableObject
	{
		public void AppendMessage (int index, string appendage)
			=> history.Data[index - 1].AppendMessage(appendage);

		public OpenAISettingsVo OpenAISettings	=> openAISettings;
		public bool ApiCallsEnabled				=> apiCallsEnabled;
		public int LatestIndex					=> history.Data.Count;
		public string CurrentUser				=> currentUser;
		public List<MessageVo> History			=> history.Data;
		public void Add (MessageVo newMessage)	=> history.Data.Add(newMessage);

		[FoldoutGroup("Settings"), SerializeField]
		bool apiCallsEnabled;
		[FoldoutGroup("Settings"), SerializeField, ValueDropdown("$users")]
		string currentUser = "Guest";

		[FoldoutGroup("OpenAI Settings"), HideLabel, SerializeField]
		OpenAISettingsVo openAISettings;

		[InlineEditor, SerializeField]
		HistorySo history;

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