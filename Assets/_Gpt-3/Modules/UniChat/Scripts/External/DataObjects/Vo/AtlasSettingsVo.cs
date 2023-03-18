using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class AtlasSettingsVo
	{
		public string Password => password;
		public string ApiKey => apiKey;
		public string BaseUrl => baseUrl;
		public string Endpoint => endpoint;

		[TextArea(1,2), SerializeField] string password;
		[TextArea(1,2), SerializeField] string apiKey;
		[TextArea(1,2), SerializeField] string baseUrl;
		[TextArea(1,2), SerializeField] string endpoint;
	}
}