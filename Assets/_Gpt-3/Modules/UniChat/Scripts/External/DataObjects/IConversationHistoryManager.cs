using System.Collections.Generic;
using System.Threading.Tasks;


namespace Modules.UniChat.External.DataObjects
{
	public interface IConversationHistoryManager
	{
		Task<List<List<float>>> RetrieveConversationHistoryAsync(string userMessage, HistoryVo chatHistory, int nearestNeighbourQueryCount = 5);
	}
}