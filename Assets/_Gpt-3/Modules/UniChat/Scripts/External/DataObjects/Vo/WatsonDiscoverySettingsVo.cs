using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class WatsonDiscoverySettingsVo
	{
		public string BaseUrl => baseUrl;
		public string ApiKey => apiKey;
		public string EnvironmentId => environmentId;
		public string CollectionId => collectionId;
		public string Version => version;

		[TextArea(1, 2), SerializeField] string baseUrl;
		[TextArea(1, 2), SerializeField] string apiKey;
		[TextArea(1, 2), SerializeField] string environmentId;
		[TextArea(1, 2), SerializeField] string collectionId;
		[TextArea(1, 2), SerializeField] string version;
	}
}