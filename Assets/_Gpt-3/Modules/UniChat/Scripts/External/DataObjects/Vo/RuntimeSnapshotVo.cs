#if UNITY_EDITOR

using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class RuntimeSnapshotVo
	{
		[JsonProperty] public string FrameRate;
		[JsonProperty] public string Timestamp;
		[JsonProperty] public string TimeSinceStartup;

		[JsonProperty] public string FramesCaptured;
		[JsonProperty] public string FramesSampleSeconds;

		// [JsonProperty] public string MemoryUsage;
		// [JsonProperty] public string SceneLoadTime;

		public RuntimeSnapshotVo (float sampleSeconds, int framesCaptured)
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