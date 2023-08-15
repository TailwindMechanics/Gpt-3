using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class SceneObjects
	{
		[JsonProperty("ranges")]
		public List<Range> Ranges { get; set; } = new();
	}

	[Serializable]
	public class Range
	{
		[JsonProperty("min_distance")]
		public string MinDistance { get; set; }

		[JsonProperty("max_distance")]
		public string? MaxDistance { get; set; }

		[JsonProperty("objects")]
		public List<string> Objects { get; set; } = new();
	}

}