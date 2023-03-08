using Newtonsoft.Json;
using System;


namespace Modules.UniChat.Internal.DataObjects
{
	[Serializable]
	public class PineConeMatchVo
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("score")]
		public double Score { get; set; }

		[JsonProperty("values")]
		public double[] Values { get; set; }
	}
}