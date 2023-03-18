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

		[JsonProperty("textAnnotations")]
		public List<TextAnnotation> TextAnnotations { get; set; }

		[JsonProperty("faceAnnotations")]
		public List<FaceAnnotation> FaceAnnotations { get; set; }

		[JsonProperty("landmarkAnnotations")]
		public List<LandmarkAnnotation> LandmarkAnnotations { get; set; }

		[JsonProperty("logoAnnotations")]
		public List<LogoAnnotation> LogoAnnotations { get; set; }

		[JsonProperty("localizedObjectAnnotations")]
		public List<LocalizedObjectAnnotation> LocalizedObjectAnnotations { get; set; }

		[JsonProperty("imagePropertiesAnnotation")]
		public ImagePropertiesAnnotation ImagePropertiesAnnotation { get; set; }

		[JsonProperty("safeSearchAnnotation")]
		public SafeSearchAnnotation SafeSearchAnnotation { get; set; }

		[JsonProperty("webDetection")]
		public WebDetection WebDetection { get; set; }
	}
}