using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.Internal.DataObjects
{
	[Serializable]
	public class ImageResponse
	{
		[JsonProperty("labelAnnotations")]
		public List<LabelAnnotation> LabelAnnotations { get; set; }
	}
}