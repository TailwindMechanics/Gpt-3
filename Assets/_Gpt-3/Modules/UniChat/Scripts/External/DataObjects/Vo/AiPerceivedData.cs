using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class AiPerceivedData
	{
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

		[JsonProperty("agent_body_radius")]
		public string AgentBodyRadius { get; set; }

		[JsonProperty("scene_objects")]
		public SceneObjects SceneObjects { get; set; } = new();

		[JsonProperty("cloud_vision_data")]
		public CloudVisionData CloudVisionData { get; set; }
	}
}