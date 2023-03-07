using Newtonsoft.Json;
using System;


namespace Modules.UniChat.Internal.DataObjects
{
	[Serializable]
	public class PineConeQueryVo
	{
		[JsonProperty("vector")]
		public double[] Vector { get; set; }

		[JsonProperty("topK")]
		public int TopK { get; set; }

		[JsonProperty("includeMetadata")]
		public bool IncludeMetadata { get; set; }

		[JsonProperty("includeValues")]
		public bool IncludeValues { get; set; }

		[JsonProperty("namespace")]
		public string Namespace { get; set; }
	}

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