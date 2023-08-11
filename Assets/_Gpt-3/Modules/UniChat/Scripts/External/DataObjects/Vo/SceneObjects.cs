using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class SceneObjects
	{
		[JsonProperty("objects_within_1m")]
		public List<string> WithinOne { get; set; } = new();

		[JsonProperty("objects_within_1_to_3m")]
		public List<string> WithinThree { get; set; } = new();

		[JsonProperty("objects_within_3_to_5m")]
		public List<string> WithinFive { get; set; } = new();

		[JsonProperty("objects_within_5_to_10m")]
		public List<string> WithinTen { get; set; } = new();

		[JsonProperty("objects_beyond_15m")]
		public List<string> Beyond { get; set; } = new();
	}
}