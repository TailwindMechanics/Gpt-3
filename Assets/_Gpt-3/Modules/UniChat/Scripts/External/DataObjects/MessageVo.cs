using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects
{
	[Serializable]
	public class MessageVo
	{
		public MessageVo (string newSender, string newMessage)
		{
			senderName = newSender;
			message = newMessage;
			SetTimestamp();
		}

		public void SetTimestamp ()
			=> timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
		public void AppendMessage (string appendage)
		{
			message += appendage;
			message = message.Replace("#Response", "").TrimStart();
		}

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
		[FoldoutGroup("$groupName"), TextArea(6,6), SerializeField]
		string message;

		[UsedImplicitly]
		string groupName => $"{Timestamp.Substring(0, 5)} "
		                    + $"{SenderName.Substring(0, Math.Min(SenderName.Length, 10)).TrimEnd()}... "
		                    + $"{Message.Substring(0, Math.Min(Message.Length, 10)).Split("\n")[0]}...";
	}
}