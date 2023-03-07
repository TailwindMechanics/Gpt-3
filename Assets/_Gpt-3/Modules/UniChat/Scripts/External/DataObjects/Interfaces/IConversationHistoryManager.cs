using System.Collections.Generic;
using System.Threading.Tasks;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.Interfaces
{
	public interface IConversationHistoryManager
	{
		Task<List<List<float>>> RetrieveConversationHistoryAsync(string userMessage, HistoryVo chatHistory, int nearestNeighbourQueryCount = 5);
	}
}