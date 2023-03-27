using Sirenix.OdinInspector;
using UnityEngine;
using System;

using Modules.UniChat.External.DataObjects.So;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class WebSearchSettingsVo
	{

		public ModelSettingsVo SummaryModel => modelSettings.Vo;
		public string BaseUrl => baseUrl;
		public string ApiKey => apiKey;
		public string EngineId => engineId;

		[InlineEditor, SerializeField] ModelSettingsSo modelSettings;
		[TextArea(1,2), SerializeField] string baseUrl = "https://www.googleapis.com/customsearch/v1?key=";
		[TextArea(1,2), SerializeField] string apiKey;
		[TextArea(1,2), SerializeField] string engineId;
	}
}