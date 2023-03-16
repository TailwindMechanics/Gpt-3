using Newtonsoft.Json;
using System;


namespace Modules.UniChat.Internal.DataObjects
{
	[Serializable]
	public class ImageContent
	{
		[JsonProperty("content")]
		public string Content { get; set; }
	}
}