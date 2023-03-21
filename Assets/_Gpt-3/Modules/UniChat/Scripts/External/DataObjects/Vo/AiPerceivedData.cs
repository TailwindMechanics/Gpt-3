using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class AiPerceivedData
	{
		public AiPerceivedData()
			=> AreaContent = new List<string>();

		[JsonProperty("time")]
		public string Time { get; set; }

		[JsonProperty("day")]
		public string Day { get; set; }

		[JsonProperty("season")]
		public string Season { get; set; }

		[JsonProperty("year")]
		public string Year { get; set; }

		// [JsonProperty("weather")]
		// public string Weather { get; set; }

		// [JsonProperty("sounds")]
		// public string Sounds { get; set; }
		//
		// [JsonProperty("smells")]
		// public string Smells { get; set; }
		//
		// [JsonProperty("geographic")]
		// public string Geographic { get; set; }
		//
		// [JsonProperty("feelings")]
		// public string Feelings { get; set; }
		//
		// [JsonProperty("doings")]
		// public string Doings { get; set; }

		[JsonProperty("your_position")]
		public string YourPosition { get; set; }

		[JsonProperty("your_rotation")]
		public string YourRotation { get; set; }

		[JsonProperty("area_content")]
		public List<string> AreaContent { get; set; }

		[JsonProperty("cloud_vision_data")]
		public CloudVisionData CloudVisionData { get; set; }
	}
}