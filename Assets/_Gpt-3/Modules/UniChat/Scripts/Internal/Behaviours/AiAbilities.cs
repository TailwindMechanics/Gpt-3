using System.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.Internal.DataObjects.Schemas;
using Modules.UniChat.External.DataObjects.Vo;
using Modules.UniChat.Internal.Apis;
using Modules.Utilities.External;


namespace Modules.UniChat.Internal.Behaviours
{
	[RequireComponent(typeof(AiPlayer))]
	public class AiAbilities : MonoBehaviour
	{
		AiPlayer AiPlayer => aiPlayer ? aiPlayer : aiPlayer = GetComponent<AiPlayer>();
		AiPlayer aiPlayer;

		[UsedImplicitly]
		string VisionDataLabel => $"Vision Data ({StringUtilities.Ellipses(visionResult)})";
		[TextArea, SerializeField] string prompt;
		[FoldoutGroup("$VisionDataLabel"), HideLabel, TextArea(20, 20), PropertyOrder(1), SerializeField]
		string visionResult;
		[Button(ButtonSizes.Medium)]
		async void Capture ()
		{
			visionResult = await CaptureVision(AiPlayer.Camera, AiPlayer.Sensor, AiPlayer.Model.Vo.Perception.Vo);
            await PointInDirection(prompt, visionResult);
		}

		async Task<string> CaptureVision(Camera cam, Transform volume, AiPerceptionSettingsVo settings)
		{
			var perceiver = new AiPerceiver() as IAiPerceiver;
			return await perceiver.CaptureVision(cam, volume, settings);
		}

		async Task PointInDirection(string prompt, string context)
		{
			var chatbotApi = new StructuredChatApi() as IStructuredChatApi;
			var schemaGenerator = new VectorSchema(
				"PointInDirection",
				"Points in the direction of the given vector.",
				"direction"
			);

			var response = await chatbotApi.GetStructuredReply<VectorSchema.Response>(
				prompt,
				context,
				schemaGenerator.Schema,
				AiPlayer.Model.Vo,
				true
			);

			var direction = response.Direction.Value();
			Debug.Log($"<color=magenta><b>>>> direction: {direction}</b></color>");
			direction = direction == Vector3.zero ? Vector3.one : direction;

			AiPlayer.Pointer.rotation = Quaternion.LookRotation(-direction);
			var scale = AiPlayer.Pointer.localScale;
			AiPlayer.Pointer.localScale = new Vector3(scale.x, scale.y, direction.magnitude);
		}
	}
}