using System.Collections.Generic;
using System.Threading.Tasks;


namespace Modules.UniChat.External.DataObjects
{
	public interface IPineConeApi
	{
		public Task<string> AddItemsAsync(List<Dictionary<string, object>> items);
		public Task<List<Dictionary<string, object>>> NearestNeighborsAsync(List<float> embedding, int numNeighbors);
	}
}