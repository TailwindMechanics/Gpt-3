using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using System;


namespace Modules.OpenAI.External.DataObjects
{
	[Serializable]
	public class ChatMessageVo
	{
		public ChatMessageVo (string newSender, string newMessage)
		{
			senderName = newSender;
			message = newMessage;
			SetTimestamp();
		}

		public void SetTimestamp ()
			=> timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

		public string SenderName => !string.IsNullOrWhiteSpace(senderName)
			? senderName
			: "Unassigned";

		public DateTime TimestampDateTime => DateTime.Parse(Timestamp);
		public string Timestamp => !string.IsNullOrWhiteSpace(timestamp)
			? timestamp
			: "Unassigned";

		public string Message => !string.IsNullOrWhiteSpace(message)
			? message
			: "Unassigned";

		[FoldoutGroup("$groupName"), SerializeField]
		string senderName;
		[FoldoutGroup("$groupName"), SerializeField]
		string timestamp;
		[FoldoutGroup("$groupName"), TextArea, SerializeField]
		string message;

		[UsedImplicitly]
		string groupName => $"{Timestamp}: {SenderName}, {Message.Substring(0, Math.Min(Message.Length, 10))}...";
	}
}