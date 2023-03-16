using Newtonsoft.Json;
using System;


namespace Modules.UniChat.Internal.DataObjects
{
	[Serializable]
	public class Feature
	{
		[JsonProperty("type")]
		public string Type { get; set; }
	}
}