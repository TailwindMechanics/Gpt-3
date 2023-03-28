using System.Threading.Tasks;
using OpenAI.FineTuning;


namespace Modules.UniChat.External.DataObjects.Interfaces
{
	public interface IFineTuningApi
	{
		Task<FineTuneJob> CreateFineTuneJobAsync(CreateFineTuneJobRequest request, bool logging = false);
		Task LogActiveJobsAsync(bool logging = false);
	}
}