using UnityEngine;
using System;


namespace Modules.OpenAI.External.DataObjects
{
	[Serializable]
	public class OpenAISettingsVo
	{
		public OpenAiModel Model		=> model;
		public float FrequencyPenalty	=> frequencyPenalty;
		public float PresencePenalty	=> presencePenalty;
		public float Temperature		=> temperature;
		public int MaxTokens			=> maxTokens;

		[SerializeField] OpenAiModel model						= OpenAiModel.DavinciText;
		[Range(0f, 1f), SerializeField] float frequencyPenalty	= .1f;
		[Range(0f, 1f), SerializeField] float presencePenalty	= .1f;
		[Range(0f, 1f), SerializeField] float temperature		= .5f;
		[Range(10, 500), SerializeField] int maxTokens			= 200;
	}
}