using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class GoogleSearchResponse
	{
		[JsonProperty("items")]
		public List<GoogleSearchResult> Items { get; set; }
	}
}