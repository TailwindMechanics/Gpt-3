using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class CameraViewData
	{
		public CameraViewData()
			=> AreaContent = new List<string>();

		[JsonProperty("timestamp")]
		public string TimeStamp { get; set; }

		[JsonProperty("camera_position")]
		public Vector3Serializable CameraPosition { get; set; }

		[JsonProperty("camera_rotation")]
		public Vector3Serializable CameraRotation { get; set; }

		// [JsonProperty("area_size")]
		// public Vector3Serializable AreaSize { get; set; }

		[JsonProperty("area_content")]
		public List<string> AreaContent { get; set; }

		[JsonProperty("cloud_vision_data")]
		public CloudVisionData CloudVisionData { get; set; }
	}
}