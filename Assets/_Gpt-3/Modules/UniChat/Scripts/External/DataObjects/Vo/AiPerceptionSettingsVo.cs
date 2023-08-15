using Sirenix.OdinInspector;
using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class AiPerceptionSettingsVo
	{
		public string SavePath				=> savePath;
		public Vector2Int Resolution		=> resolution;
		public double MinPixelThreshold		=> minPixelThreshold;
		public double MaxTokens				=> maxTokens;

		[SerializeField] Vector2Int resolution;
		[FolderPath, SerializeField] string savePath;
        [SerializeField] double minPixelThreshold = 5;
        [Range(0, 2000), SerializeField] double maxTokens = 1000;
	}
}