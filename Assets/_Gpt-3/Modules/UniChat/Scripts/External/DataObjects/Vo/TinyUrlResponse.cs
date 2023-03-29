using Newtonsoft.Json;


namespace Modules.UniChat.External.DataObjects.Vo
{
	public class TinyUrlResponse
	{
		[JsonProperty("data")]
		public TinyUrlData Data { get; set; }
	}

	public class TinyUrlData
	{
		[JsonProperty("tiny_url")]
		public string TinyUrl { get; set; }
	}
}