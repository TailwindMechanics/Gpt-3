using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class WebSearchSettingsVo
	{
		public string BaseUrl => baseUrl;
		public string ApiKey => apiKey;
		public string EngineId => engineId;

		[TextArea(1,2), SerializeField] string baseUrl = "https://www.googleapis.com/customsearch/v1?key=";
		[TextArea(1,2), SerializeField] string apiKey;
		[TextArea(1,2), SerializeField] string engineId;
	}
}