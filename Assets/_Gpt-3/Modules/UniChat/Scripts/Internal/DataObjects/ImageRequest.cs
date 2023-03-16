using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.Internal.DataObjects
{
	[Serializable]
	public class ImageRequest
	{
		[JsonProperty("image")]
		public ImageContent Image { get; set; }

		[JsonProperty("features")]
		public List<Feature> Features { get; set; }
	}
}