using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class GoogleCloudVisionResponseVo
	{
		[JsonProperty("responses")]
		public List<ImageResponse> Responses { get; set; }
	}
}