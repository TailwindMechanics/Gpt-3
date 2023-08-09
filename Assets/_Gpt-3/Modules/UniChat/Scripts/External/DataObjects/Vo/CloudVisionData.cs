using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class CloudVisionData
	{
		public CloudVisionData(GoogleCloudVisionResponseVo visionResponse)
		{
			var response = visionResponse.Responses[0];
			LabelAnnotations = response.LabelAnnotations?.ConvertAll(item => item.ToString());

			if (response.TextAnnotations is {Count: > 1})
			{
				var meaningful = new List<string>();
				for (var i = 1; i < response.TextAnnotations.Count; i++)
				{
					var item = response.TextAnnotations[i];
					if (item.Description.Length > 4 && !meaningful.Contains(item.Description))
					{
						meaningful.Add(item.Description);
					}
				}

				TextAnnotations = "___";
				meaningful.ForEach(item => TextAnnotations += $", {item}");
				TextAnnotations = TextAnnotations.Replace("___, ", "");
			}

			// FaceAnnotations = response.FaceAnnotations?.ConvertAll(item => item.ToString());
			// LandmarkAnnotations = response.LandmarkAnnotations?.ConvertAll(item => item.ToString());
			// LogoAnnotations = response.LogoAnnotations?.ConvertAll(item => item.ToString());
			// SafeSearchAnnotation = response.SafeSearchAnnotation?.ToString();

			ImagePropertiesAnnotation = response.ImagePropertiesAnnotation?.ToString();
			WebDetection = response.WebDetection?.ToString();
			LocalizedObjectAnnotations = response.LocalizedObjectAnnotations?.ConvertAll(item => item.ToString());
		}

		[JsonProperty("label_annotations")]
		public List<string> LabelAnnotations { get; set; }

		[JsonProperty("text_annotations")]
		public string TextAnnotations { get;  set; }

		// [JsonProperty("face_annotations")]
		// public List<string> FaceAnnotations { get; set; }

		// [JsonProperty("landmark_annotations")]
		// public List<string> LandmarkAnnotations { get; set; }

		// [JsonProperty("logo_annotations")]
		// public List<string> LogoAnnotations { get; set; }

		// [JsonProperty("safe_search_annotation")]
		// public string SafeSearchAnnotation { get; set; }

		[JsonProperty("image_properties_annotation")]
		public string ImagePropertiesAnnotation { get; set; }

		[JsonProperty("web_detection")]
		public string WebDetection { get; set; }

		[JsonProperty("localized_object_annotations")]
		public List<string> LocalizedObjectAnnotations { get; set; }
	}
}