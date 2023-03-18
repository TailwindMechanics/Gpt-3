using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class LogoAnnotation
	{
		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("score")]
		public float Score { get; set; }

		public override string ToString()
			=> $"Description: {Description}, Score: {Score}";
	}
}