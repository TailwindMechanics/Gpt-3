#if UNITY_EDITOR

using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects
{
	[Serializable]
	public class RuntimeSnapshot
	{
		[JsonProperty] public string FrameRate;
		[JsonProperty] public string Timestamp;
		[JsonProperty] public string TimeSinceStartup;

		[JsonProperty] public string FramesCaptured;
		[JsonProperty] public string FramesSampleSeconds;

		// [JsonProperty] public string MemoryUsage;
		// [JsonProperty] public string SceneLoadTime;

		public RuntimeSnapshot (float sampleSeconds, int framesCaptured)
		{
			FrameRate               = (framesCaptured / sampleSeconds).ToString("F2") + "fps";
			TimeSinceStartup        = EditorApplication.timeSinceStartup.ToString("F2") + "s";
			Timestamp               = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

			FramesCaptured          = framesCaptured.ToString();
			FramesSampleSeconds     = sampleSeconds.ToString("F2") + "s";

			// SceneLoadTime           = sceneLoadTime;
			// MemoryUsage             = memoryUsage;
		}

		public string Json => JsonUtility.ToJson(this);
	}
}

#endif