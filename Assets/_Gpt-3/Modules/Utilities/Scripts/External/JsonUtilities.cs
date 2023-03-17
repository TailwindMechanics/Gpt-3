using UnityEngine;
using System.IO;
using Newtonsoft.Json;


namespace Modules.Utilities.External
{
	public static class JsonUtilities
	{
		public static string SaveAsJsonFile(string folderPath, string fileName, object obj)
		{
			fileName = fileName.Replace(".json", "");
			fileName += ".json";

			var path = Path.Combine(folderPath, fileName);
			var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
			Debug.Log(json);
			File.WriteAllText(path, json);

			return path;
		}
	}
}