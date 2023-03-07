using System.Collections.Generic;
using Sirenix.OdinInspector;
using OpenAI.Models;
using UnityEngine;
using System;
using OpenAI;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class SerializableModelVo
	{
		public Model Model => new(model);

		[ValueDropdown("$allModels"), SerializeField] string model	= "text-davinci-003";
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