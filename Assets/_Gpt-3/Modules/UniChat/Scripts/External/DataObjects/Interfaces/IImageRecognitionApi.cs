using System.Threading.Tasks;

using Modules.UniChat.External.DataObjects.So;
using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.Interfaces
{
	public interface IImageRecognitionApi
	{
		Task<GoogleCloudVisionResponseVo> AnalyzeImage(string imagePath, GoogleCloudVisionSettingsSo settings, bool logging = false);
	}
}