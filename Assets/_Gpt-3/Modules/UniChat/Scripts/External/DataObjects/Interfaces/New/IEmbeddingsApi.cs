using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI.Models;


namespace Modules.UniChat.External.DataObjects.Interfaces.New
{
	public interface IEmbeddingsApi
	{
		Task<IReadOnlyList<double>> ConvertToVector(Model model, string sender, string message, bool logging = false);
	}
}