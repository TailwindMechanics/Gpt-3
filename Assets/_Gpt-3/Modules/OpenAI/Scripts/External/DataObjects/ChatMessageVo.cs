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
		}

		public string SenderName => !string.IsNullOrWhiteSpace(senderName)
			? senderName
			: "Unassigned";

		public string Message => !string.IsNullOrWhiteSpace(message)
			? message
			: "Unassigned";

		[FoldoutGroup("$groupName"), SerializeField]
		string senderName;
		[FoldoutGroup("$groupName"), TextArea, SerializeField]
		string message;

		[UsedImplicitly]
		string groupName => $"{SenderName}, {Message.Substring(0, Math.Min(Message.Length, 10))}...";
	}
}