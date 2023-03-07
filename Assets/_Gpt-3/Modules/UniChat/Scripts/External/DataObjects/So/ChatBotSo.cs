using System.Threading.Tasks;
using OpenAI.Completions;
using OpenAI.Models;
using UnityEngine;
using OpenAI;

using Modules.UniChat.External.DataObjects.Interfaces;


namespace Modules.UniChat.External.DataObjects.So
{
	[CreateAssetMenu(fileName = "New Chat Bot", menuName = "Chat Bot")]
	public class ChatBot : ScriptableObject, IChat
	{
		public async Task<string> GetResponse(string message)
		{
			var client = new OpenAIClient(Model.Davinci);
			var request = new CompletionRequest
			{
				Prompt = message + "\n",
				MaxTokens = 128,
				Temperature = 0.5,
				TopP = 1,
				FrequencyPenalty = 0,
				PresencePenalty = 0,
			};

			var response = await client.CompletionsEndpoint.CreateCompletionAsync(request);

			return response.Completions[0].Text.Trim();
		}
	}
}