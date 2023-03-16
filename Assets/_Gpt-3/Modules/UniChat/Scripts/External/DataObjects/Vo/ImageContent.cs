using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class ImageContent
	{
		[JsonProperty("content")]
		public string Content { get; set; }
	}
}