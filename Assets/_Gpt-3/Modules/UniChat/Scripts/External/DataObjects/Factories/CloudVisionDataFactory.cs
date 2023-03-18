using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.Factories
{
	// public class CloudVisionDataFactory
	// {
	// 	public static CloudVisionData CreateFromVisionResponse(GoogleCloudVisionResponseVo visionResponse)
	// 	{
	// 		var response = visionResponse.Responses[0];
	// 		var cloudVisionData = new CloudVisionData
	// 		{
	// 			LabelAnnotations = response.LabelAnnotations.ConvertAll(item => item.ToString()),
	// 			TextAnnotations = response.TextAnnotations.ConvertAll(item => item.ToString()),
	// 			FaceAnnotations = response.FaceAnnotations.ConvertAll(item => item.ToString()),
	// 			LandmarkAnnotations = response.LandmarkAnnotations.ConvertAll(item => item.ToString()),
	// 			LogoAnnotations = response.LogoAnnotations.ConvertAll(item => item.ToString()),
	// 			ImagePropertiesAnnotation = response.ImagePropertiesAnnotation.ToString(),
	// 			SafeSearchAnnotation = response.SafeSearchAnnotation.ToString(),
	// 			WebDetection = response.WebDetection.ToString()
	// 		};
	//
	// 		return cloudVisionData;
	// 	}
	// }
}