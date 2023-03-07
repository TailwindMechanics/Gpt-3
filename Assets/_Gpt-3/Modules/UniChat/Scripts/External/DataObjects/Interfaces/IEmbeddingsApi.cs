using System.Threading.Tasks;
using OpenAI.Models;


namespace Modules.UniChat.External.DataObjects.Interfaces
{
	public interface IEmbeddingsApi
	{
		Task<string> GetEmbedding(string message, Model model);
	}
}