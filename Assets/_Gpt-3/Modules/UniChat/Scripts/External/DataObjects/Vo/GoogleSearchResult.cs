using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class GoogleSearchResult
	{
		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("link")]
		public string Url { get; set; }

		[JsonProperty("snippet")]
		public string Snippet { get; set; }
	}
}