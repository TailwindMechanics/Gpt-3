using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class GoogleCloudVisionRequestVo
	{
		[JsonProperty("requests")]
		public List<ImageRequest> Requests { get; set; }
	}
}