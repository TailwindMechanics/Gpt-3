﻿using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class TinyUrlSettingsVo
	{
		public string ApiKey => apiKey;
		public string Endpoint => endpoint;
		[TextArea(1,2), SerializeField] string apiKey;
		[TextArea(1,2), SerializeField] string endpoint;
	}
}