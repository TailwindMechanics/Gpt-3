using Newtonsoft.Json;


namespace Modules.UniChat.External.DataObjects.Vo
{
	public class SearchResult
	{
		[JsonProperty("url")]
		public string Url { get; set; }
	}
}