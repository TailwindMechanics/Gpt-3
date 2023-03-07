using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class PineConeSettingsVo
	{
		public string ApiKey			=> apiKey;
		public string UpsertEndpoint	=> upsertEndpoint;
		public string QueryEndpointPath => queryEndpoint;
		public int NumberOfNeighbours	=> numberOfNeighbours;

		[SerializeField] string apiKey;
		[SerializeField] string queryEndpoint;
		[SerializeField] string upsertEndpoint;
		[SerializeField] int numberOfNeighbours = 5;
	}
}