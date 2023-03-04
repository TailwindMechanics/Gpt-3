using System.Collections.Generic;
using Sirenix.OdinInspector;
using OpenAI.Models;
using UnityEngine;
using OpenAI;
using System;


namespace Modules.UniChat.External.DataObjects
{
	[Serializable]
	public class ChatBotSettingsVo
	{
		public Model Model					=> new(model);
		public float FrequencyPenalty		=> frequencyPenalty;
		public float PresencePenalty		=> presencePenalty;
		public float Temperature			=> temperature;
		public int MaxTokens				=> maxTokens;
		public bool UseChatCompletion		=> useChatCompletion;
		public float TopP					=> topP;

		[SerializeField] bool useChatCompletion						= false;
		[ValueDropdown("$allModels"), SerializeField] string model	= "text-davinci-003";
		[Range(0f, 1f), SerializeField] float frequencyPenalty		= .1f;
		[Range(0f, 1f), SerializeField] float presencePenalty		= .1f;
		[Range(0f, 1f), SerializeField] float temperature			= .5f;
		[Range(10, 500), SerializeField] int maxTokens				= 200;
		[Range(0f, 1f), SerializeField] float topP					= 1f;
		[PropertyOrder(2), SerializeField] List<string> allModels	= new();

		[Button(ButtonSizes.Medium)]
		async void RefreshModels ()
		{
			Debug.Log("<color=orange><b>>>> Fetching models...</b></color>");

			var api		= new OpenAIClient();
			var models	= await api.ModelsEndpoint.GetModelsAsync();

			var newModelsCount = 0;
			foreach (var mod in models)
			{
				if (allModels.Contains(mod.ToString())) continue;

				newModelsCount++;
				var newModel = mod.ToString();
				Debug.Log($"<color=cyan><b>>>> New model added: \"{newModel}\"</b></color>");
				allModels.Add(newModel);
			}

			Debug.Log(newModelsCount == 0
				? "<color=orange><b>>>> No new models found.</b></color>"
				: $"<color=white><b>>>> {newModelsCount} new models found.</b></color>");
		}
	}
}