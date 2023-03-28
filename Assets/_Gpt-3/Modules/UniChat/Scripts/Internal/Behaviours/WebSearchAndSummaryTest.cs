using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using System.Net;
using System.IO;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.So;
using Modules.UniChat.Internal.Apis;
using Modules.Utilities.External;


namespace Modules.UniChat.Internal.Behaviours
{
	public class WebSearchAndSummaryTest : MonoBehaviour
	{
		[Range(1, 5), SerializeField] int resultCount = 1;
		[FolderPath, SerializeField] string savePath;
		[SerializeField] string fileName;
		[SerializeField] WebSearchSettingsSo settings;
		[SerializeField, TextArea(5,5)] string query;

		[FoldoutGroup("Url"), PropertyOrder(2), SerializeField]
		string url = "https://docs.unity3d.com/Packages/com.unity.rendering.hybrid@0.4/";

		[FoldoutGroup("Url"), PropertyOrder(2), Button(ButtonSizes.Medium)]
		async void GetUnityDocsData()
		{
			try
			{
				var request = WebRequest.Create(url);
				var response = await request.GetResponseAsync();
				await using var stream = response.GetResponseStream();
				if (stream == null) return;

				using var reader = new StreamReader(stream);
				var data = await reader.ReadToEndAsync();
				Debug.Log(data);
			}
			catch (WebException ex)
			{
				Debug.LogError($"Error fetching data: {ex.Message}");
			}
		}

		[Button(ButtonSizes.Medium)]
		async void DoSearch()
		{
			var api = new WebSearchSummaryApi(settings.Vo, true) as IWebSearchSummaryApi;
			var result = await api.SearchAndGetSummary(query);
			JsonUtilities.SaveAsJsonFile(savePath, fileName, result);

			AssetDatabase.Refresh();
		}
	}
}