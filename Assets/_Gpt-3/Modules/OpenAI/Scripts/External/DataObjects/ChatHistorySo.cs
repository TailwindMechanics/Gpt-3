using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Modules.OpenAI.External.DataObjects
{
	[CreateAssetMenu(fileName = "new _ChatHistory", menuName = "Tailwind/Chat/History")]
	public class ChatHistorySo : ScriptableObject
	{
		public string CurrentUser => currentUser;
		public List<ChatMessageVo> History => history;
		public void Add (ChatMessageVo newMessage)
			=> history.Add(newMessage);

		[SerializeField, ValueDropdown("$users")]
		string currentUser = "Guest";
		[SerializeField]
		List<ChatMessageVo> history = new();

		[UsedImplicitly]
		static string[] users = {"John Smith", "Jane Doe"};
	}
}