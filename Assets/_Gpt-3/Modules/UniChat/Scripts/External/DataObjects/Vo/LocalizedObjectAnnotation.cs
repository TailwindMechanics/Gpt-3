using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class LocalizedObjectAnnotation
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("score")]
		public float Score { get; set; }

		[JsonProperty("boundingPoly")]
		public BoundingPoly BoundingPoly { get; set; }

		public override string ToString()
			=> $"{Name}, score: {Score}";
	}
}