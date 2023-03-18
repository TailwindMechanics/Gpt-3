using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class NormalizedVertex
	{
		[JsonProperty("x")]
		public float X { get; set; }

		[JsonProperty("y")]
		public float Y { get; set; }
	}
}