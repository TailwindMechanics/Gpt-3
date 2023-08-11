using Newtonsoft.Json;
using System.IO;


namespace Modules.Utilities.External
{
	public static class JsonUtilities
	{
		public static (string json, string filePath) SaveAsJsonFile(string folderPath, string fileName, object obj, Formatting formatting = Formatting.None)
		{
			fileName = fileName.Replace(".json", "");
			fileName += ".json";

			var path = Path.Combine(folderPath, fileName);
			var json = JsonConvert.SerializeObject(obj, formatting);
			File.WriteAllText(path, json);

			return (json, path);
		}
	}
}