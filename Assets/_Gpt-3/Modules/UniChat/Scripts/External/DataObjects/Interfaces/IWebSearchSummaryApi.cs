using System.Threading.Tasks;


namespace Modules.UniChat.External.DataObjects.Interfaces
{
	public interface IWebSearchSummaryApi
	{
		Task<string> SearchAndGetSummary(string query);
	}
}