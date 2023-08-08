using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.Internal.DataObjects.Schemas;
using Modules.UniChat.External.DataObjects.Vo;
using Modules.UniChat.Internal.Apis;


namespace Modules.UniChat.Internal.Behaviours
{
	[RequireComponent(typeof(AiPlayer))]
	public class AiAbilities : MonoBehaviour
	{
		AiPlayer AiPlayer => aiPlayer ? aiPlayer : aiPlayer = GetComponent<AiPlayer>();
		AiPlayer aiPlayer;

		[TextArea, SerializeField] string prompt;
		[TextArea, SerializeField] string visionResult;
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
			var schemaGenerator = new VectorSchemaGenerator(
				"PointInDirection",
				"Points in the direction of the given vector.",
				"direction"
			);

			var response = await chatbotApi.GetStructuredReply<VectorSchemaGenerator.ResponseData>(
				prompt,
				context,
				schemaGenerator.Schema,
				AiPlayer.Model.Vo,
				true
			);

			var direction = response.Direction.Value();
			AiPlayer.Pointer.rotation = Quaternion.LookRotation(-direction);
			var scale = AiPlayer.Pointer.localScale;
			AiPlayer.Pointer.localScale = new Vector3(scale.x, scale.y, direction.magnitude);
		}
	}
}