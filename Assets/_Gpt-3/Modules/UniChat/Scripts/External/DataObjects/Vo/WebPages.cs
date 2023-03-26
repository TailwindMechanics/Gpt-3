using System.Collections.Generic;
using Newtonsoft.Json;


namespace Modules.UniChat.External.DataObjects.Vo
{
	public class WebPages
	{
		[JsonProperty("value")]
		public List<SearchResult> Value { get; set; }
	}
}