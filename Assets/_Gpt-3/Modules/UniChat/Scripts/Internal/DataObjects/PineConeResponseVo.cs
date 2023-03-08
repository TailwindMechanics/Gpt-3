using Newtonsoft.Json;
using System;


namespace Modules.UniChat.Internal.DataObjects
{
	[Serializable]
	public class PineConeResponseVo
	{
		[JsonProperty("results")]
		public object[] Results { get; set; }

		[JsonProperty("matches")]
		public PineConeMatchVo[] Matches { get; set; }

		[JsonProperty("namespace")]
		public string Namespace { get; set; }
	}
}