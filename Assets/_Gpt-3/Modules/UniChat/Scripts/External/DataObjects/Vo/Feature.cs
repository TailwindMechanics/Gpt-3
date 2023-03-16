using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class Feature
	{
		[JsonProperty("type")]
		public string Type { get; set; }
	}
}