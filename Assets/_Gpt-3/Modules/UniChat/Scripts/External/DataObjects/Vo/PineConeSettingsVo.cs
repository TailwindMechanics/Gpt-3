using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class PineConeSettingsVo
	{
		public string ApiKey				=> apiKey;
		public string IndexEndpoint			=> indexEndpoint;
		public string UpsertEndpoint		=> upsertEndpoint;
		public string DeleteEndpoint		=> deleteEndpoint;
		public string QueryEndpointPath		=> queryEndpoint;
		public int NumberOfNeighbours		=> numberOfNeighbours;

		[TextArea(1,2), SerializeField] string apiKey;
		[TextArea(1,2), SerializeField] string indexEndpoint;
		[TextArea(1,2), SerializeField] string queryEndpoint;
		[TextArea(1,2), SerializeField] string upsertEndpoint;
		[TextArea(1,2), SerializeField] string deleteEndpoint;
		[SerializeField] int numberOfNeighbours = 5;
	}
}