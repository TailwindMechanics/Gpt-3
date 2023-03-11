using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class PineConeSettingsVo
	{
		public string ApiKey				=> apiKey;
		public string IndexUrl				=> indexUrl;

		[TextArea(1,2), SerializeField] string apiKey;
		[TextArea(1,2), SerializeField] string indexUrl;
	}
}