using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.Internal.DataObjects
{
	[Serializable]
	public class GoogleCloudVisionResponseVo
	{
		[JsonProperty("responses")]
		public List<ImageResponse> Responses { get; set; }
	}
}