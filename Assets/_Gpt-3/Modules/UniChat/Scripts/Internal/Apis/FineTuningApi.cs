using System.Threading.Tasks;
using OpenAI.FineTuning;
using System.Net.Http;
using UnityEngine;
using OpenAI;

using Modules.UniChat.External.DataObjects.Interfaces;


namespace Modules.UniChat.Internal.Apis
{
	public class FineTuningApi : IFineTuningApi
	{
		readonly OpenAIClient openAiApi;


		public FineTuningApi()
			=> openAiApi = new OpenAIClient();

		public async Task<FineTuneJob> CreateFineTuneJobAsync(CreateFineTuneJobRequest request, bool logging = false)
		{
			if (logging)
			{
				Log("Creating a fine-tune job...");
			}

			try
			{
				var fineTuneJob = await openAiApi.FineTuningEndpoint.CreateFineTuneJobAsync(request);

				if (logging)
				{
					Log($"Fine-tune job created with ID: {fineTuneJob.Id}");
				}

				return fineTuneJob;
			}
			catch (HttpRequestException ex)
			{
				Debug.LogError($"OpenAI API error: {ex.Message}");
				throw;
			}
		}

		public async Task LogActiveJobsAsync(bool logging = false)
		{
			if (logging)
			{
				Log("Listing fine-tune jobs...");
			}

			try
			{
				var fineTuneJobs = await openAiApi.FineTuningEndpoint.ListFineTuneJobsAsync();

				if (logging)
				{
					Log("Active fine-tune jobs:");
					foreach (var job in fineTuneJobs)
					{
						if (job.Status is "running" or "waiting")
						{
							Log($"Job ID: {job.Id}, Status: {job.Status}");
						}
					}
				}
			}
			catch (HttpRequestException ex)
			{
				Debug.LogError($"OpenAI API error: {ex.Message}");
				throw;
			}
		}

		void Log(string message)
			=> Debug.Log($"<color=#E0E08D><b>>>> FineTuningApi: {message.Replace("\n", "")}</b></color>");
	}
}