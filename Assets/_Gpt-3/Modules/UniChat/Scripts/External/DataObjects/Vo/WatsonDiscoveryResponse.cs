using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace Modules.UniChat.External.DataObjects.Vo
{
	public class WatsonDiscoveryResponse
	{
		[JsonProperty("matching_results")]
		public int MatchingResults;

		[JsonProperty("results")]
		public JArray Results;
	}
}