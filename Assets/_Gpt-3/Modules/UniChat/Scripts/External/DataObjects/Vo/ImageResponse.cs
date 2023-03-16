using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class ImageResponse
	{
		[JsonProperty("labelAnnotations")]
		public List<LabelAnnotation> LabelAnnotations { get; set; }
	}
}