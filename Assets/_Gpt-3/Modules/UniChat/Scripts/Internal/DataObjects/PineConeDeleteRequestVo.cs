using System.Collections.Generic;
using Newtonsoft.Json;


namespace Modules.UniChat.Internal.DataObjects
{
	public class PineConeDeleteRequestVo
	{
		[JsonProperty("ids")]
		public List<string> Ids { get; set; }

		[JsonProperty("deleteAll")]
		public bool DeleteAll { get; set; }

		[JsonProperty("namespace")]
		public string Namespace { get; set; }

		[JsonProperty("filter")]
		public object Filter { get; set; }
	}
}