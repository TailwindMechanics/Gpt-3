using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Modules.OpenAI.External.DataObjects
{
	[CreateAssetMenu(fileName = "new _chatHistory", menuName = "Tailwind/Chat/History")]
	public class ChatHistorySo : ScriptableObject
	{
		public bool ApiCallsEnabled => apiCallsEnabled;
		public ChatMessageVo Latest => history.Count > 0 ? history[^1] : null;
		public string CurrentUser => currentUser;
		public List<ChatMessageVo> History => history;
		public void Add (ChatMessageVo newMessage) => history.Add(newMessage);

		[SerializeField] bool apiCallsEnabled;
		[SerializeField, ValueDropdown("$users")]
		string currentUser = "Guest";
		[SerializeField]
		List<ChatMessageVo> history = new();

		[UsedImplicitly]
		static string[] users = {"John Smith", "Jane Doe"};
	}
}