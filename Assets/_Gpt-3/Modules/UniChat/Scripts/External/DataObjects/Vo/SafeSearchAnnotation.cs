using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class SafeSearchAnnotation
	{
		[JsonProperty("adult")]
		public string Adult { get; set; }

		[JsonProperty("spoof")]
		public string Spoof { get; set; }

		[JsonProperty("medical")]
		public string Medical { get; set; }

		[JsonProperty("violence")]
		public string Violence { get; set; }

		[JsonProperty("racy")]
		public string Racy { get; set; }

		public override string ToString()
			=> $"Adult:{Adult}, Spoof:{Spoof}, Medical:{Medical}, Violence:{Violence}, Racy:{Racy}";
	}
}