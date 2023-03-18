using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class FaceAnnotation
	{
		[JsonProperty("joyLikelihood")]
		public string JoyLikelihood { get; set; }

		[JsonProperty("sorrowLikelihood")]
		public string SorrowLikelihood { get; set; }

		[JsonProperty("angerLikelihood")]
		public string AngerLikelihood { get; set; }

		[JsonProperty("surpriseLikelihood")]
		public string SurpriseLikelihood { get; set; }

		public override string ToString()
			=> $"JoyLikelihood:{JoyLikelihood}, " +
			   $"SorrowLikelihood:{SorrowLikelihood}, " +
			   $"AngerLikelihood:{AngerLikelihood}, " +
			   $"SurpriseLikelihood: {SurpriseLikelihood}";
	}
}