using System.Threading.Tasks;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.Interfaces
{
	public interface IAiPerceiver
	{
		Task<string> CaptureVision (Camera cam, Transform volume, AiPerceptionSettingsVo settings);
	}
}