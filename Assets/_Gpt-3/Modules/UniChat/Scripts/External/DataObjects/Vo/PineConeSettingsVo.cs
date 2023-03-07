using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class PineConeSettingsVo
	{
		public string ApiKey => apiKey;
		public string UpsertEndpoint => upsertEndpoint;
		public string QueryEndpointPath => queryEndpoint;

		[SerializeField] string apiKey;
		[SerializeField] string queryEndpoint;
		[SerializeField] string upsertEndpoint;
	}
}