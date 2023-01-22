using System;
using UnityEngine;

namespace Modules.OpenAI.External.DataObjects
{
	[Serializable]
	public class OpenApiCredentialsVo
	{
		public string ApiKey => apiKey;
		[SerializeField] string apiKey;
	}
}