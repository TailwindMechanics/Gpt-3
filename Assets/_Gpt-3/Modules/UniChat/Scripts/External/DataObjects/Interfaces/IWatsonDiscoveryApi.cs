using System.Threading.Tasks;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.Interfaces
{
	public interface IWatsonDiscoveryApi
	{
		Task<WatsonDiscoveryResponse> Search(string query, bool logging = false);
	}
}