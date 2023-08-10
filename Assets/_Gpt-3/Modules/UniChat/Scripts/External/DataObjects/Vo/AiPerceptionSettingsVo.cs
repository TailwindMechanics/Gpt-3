using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System;

using Modules.UniChat.External.DataObjects.So;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class AiPerceptionSettingsVo
	{
		public string SavePath					=> savePath;
		public Vector2Int Resolution			=> resolution;
		public List<string> BlackList			=> areaBlackList;
		public double MaxSightDistance			=> maxSightDistance;
		public List<string> RemoveFromNames		=> removeFromNames;
		public GoogleCloudVisionSettingsSo CloudVisionCreds => cloudVisionCreds;

		[SerializeField] Vector2Int resolution;
		[FolderPath, SerializeField] string savePath;
        [SerializeField] double maxSightDistance = 20;
		[SerializeField] GoogleCloudVisionSettingsSo cloudVisionCreds;
		[SerializeField] List<string> areaBlackList = new();
		[SerializeField] List<string> removeFromNames = new();
	}
}