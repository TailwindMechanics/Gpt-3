using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class CameraCapturedSceneData
	{
		public CameraCapturedSceneData()
			=> Objects = new List<SceneObjectData>();

		[JsonProperty("timestamp")]
		public string TimeStamp { get; set; }

		[JsonProperty("camera_position")]
		public Vector3Serializable CameraPosition { get; set; }

		[JsonProperty("camera_rotation")]
		public QuaternionSerializable CameraRotation { get; set; }

		[JsonProperty("objects")]
		public List<SceneObjectData> Objects  { get; set; }
	}

	[Serializable]
	public class SceneObjectData
	{
		public SceneObjectData ()
			=> Components = new List<string>();

		[JsonProperty("object_name")]
		public string ObjectName { get; set; }

		[JsonProperty("prefab_root")]
		public string PrefabRoot { get; set; }

		[JsonProperty("object_position")]
		public Vector3Serializable ObjectPosition { get; set; }

		[JsonProperty("object_rotation")]
		public QuaternionSerializable ObjectRotation { get; set; }

		[JsonProperty("object_scale")]
		public Vector3Serializable ObjectScale { get; set; }

		[JsonProperty("components")]
		public List<string> Components { get; set; }
	}
}