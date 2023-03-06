using OpenAI.Models;
using UnityEngine;
using System;
using Sirenix.OdinInspector;


namespace Modules.UniChat.External.DataObjects
{
	[Serializable]
	public class ChatBotSettingsVo
	{
		public Model Model					=> model.Model;
		public float FrequencyPenalty		=> frequencyPenalty;
		public float PresencePenalty		=> presencePenalty;
		public float Temperature			=> temperature;
		public int MaxTokens				=> maxTokens;
		public bool UseChatCompletion		=> useChatCompletion;
		public float TopP					=> topP;

		[SerializeField] bool useChatCompletion;
		[Range(0f, 1f), SerializeField] float frequencyPenalty		= .1f;
		[Range(0f, 1f), SerializeField] float presencePenalty		= .1f;
		[Range(0f, 1f), SerializeField] float temperature			= .5f;
		[Range(10, 500), SerializeField] int maxTokens				= 200;
		[Range(0f, 1f), SerializeField] float topP					= 1f;
		[HideLabel, SerializeField] SerializableModel model;
	}
}