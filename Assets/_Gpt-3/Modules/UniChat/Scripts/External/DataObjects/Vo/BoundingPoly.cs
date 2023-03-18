using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class BoundingPoly
	{
		[JsonProperty("normalizedVertices")]
		public List<NormalizedVertex> NormalizedVertices { get; set; }
	}
}