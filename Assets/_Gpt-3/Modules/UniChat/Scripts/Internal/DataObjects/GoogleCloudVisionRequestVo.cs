using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Modules.UniChat.Internal.DataObjects
{
	[Serializable]
	public class GoogleCloudVisionRequestVo
	{
		[JsonProperty("requests")]
		public List<ImageRequest> Requests { get; set; }
	}
}