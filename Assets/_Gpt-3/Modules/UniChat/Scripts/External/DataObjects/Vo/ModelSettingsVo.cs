using Sirenix.OdinInspector;
using OpenAI.Models;
using UnityEngine;
using System;

using Modules.UniChat.External.DataObjects.So;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class ModelSettingsVo
	{
		public string BotName				=> botName;
		public string Direction				=> direction.text;
		public int SendSimilarChatCount		=> sendSimilarChatCount;
		public int SendChatHistoryCount		=> sendChatHistoryCount;
		public float MemoryAccuracy			=> memoryAccuracy;
		public int MaxTokens				=> maxTokens;
		public double Temperature			=> temperature;
		public double TopP					=> topP;
		public double PresencePenalty		=> presencePenalty;
		public double FrequencyPenalty		=> frequencyPenalty;
		public Model Model					=> model.Model;
		public AiPerceptionSettingsSo Perception => perceptionSettings;

		[Tooltip("Display name of the model"), SerializeField]
		string botName = "Bot";
		[Tooltip("Text file containing prompt direction"),  SerializeField]
		TextAsset direction;
		[Range(0, 100), Tooltip("The max number of similar chat history messages sent to the bot."), SerializeField]
		int sendSimilarChatCount = 5;
		[Range(0, 100), Tooltip("The number of recent chat history messages sent to the bot."), SerializeField]
		int sendChatHistoryCount = 4;
		[Range(0f, 1f), Tooltip("The minimum score threshold for vdb matches, higher number = more accurate."), SerializeField]
		float memoryAccuracy = 0.8f;
		[Range(1, 8192), Tooltip("Maximum number of tokens the bot can use to generate a response."), SerializeField]
		int maxTokens = 2048;
		[Range(0, 1), Tooltip("Controls the randomness of the bots response."), SerializeField]
		double temperature = 0.7;
		[Range(0, 1), Tooltip("Controls the diversity of the bots response."), SerializeField]
		double topP = 1.0;
		[Range(0, 1), Tooltip("Controls how much the bot avoids repeating itself."), SerializeField]
		double presencePenalty = 0.0;
		[Range(0, 1), Tooltip("Controls how much the bot avoids using words it has recently used."), SerializeField]
		double frequencyPenalty = 0.0;
		[Tooltip("Only chat models are valid currently."), HideLabel, SerializeField]
		SerializableModelVo model;
		[Tooltip("Settings for how this bot perceives"), InlineEditor, SerializeField]
		AiPerceptionSettingsSo perceptionSettings;
	}
}