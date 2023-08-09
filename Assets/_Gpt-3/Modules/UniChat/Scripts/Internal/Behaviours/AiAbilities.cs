using System.Threading.Tasks;
using UnityEngine;
using System;
using UniRx;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.Internal.DataObjects.Schemas;
using Modules.UniChat.External.DataObjects.Vo;
using Modules.UniChat.Internal.Apis;


namespace Modules.UniChat.Internal.Behaviours
{
	[RequireComponent(typeof(AiPlayer)), ExecuteAlways]
	public class AiAbilities : MonoBehaviour
	{
		readonly ISubject<bool> completed = new Subject<bool>();
		AiPlayer Player => player ??= GetComponent<AiPlayer>();
		bool isFacingDestination;
		Vector3 destination;
		float turnSpeed;
		float moveSpeed;
		AiPlayer player;
		bool move;


		public async Task<string> CaptureVision(Camera cam, Transform volume, AiPerceptionSettingsVo settings)
		{
			var perceiver = new AiPerceiver() as IAiPerceiver;
			return await perceiver.CaptureVision(cam, volume, settings);
		}

		public async Task<Vector3> DeduceDirection(string prompt, string context, ModelSettingsVo settings)
		{
			var chatbotApi = new StructuredChatApi() as IStructuredChatApi;
			var schemaGenerator = new VectorSchema(
				"MoveInDirection",
				"Moves the AI agent in the direction.",
				"direction"
			);

			var response = await chatbotApi.GetStructuredReply<VectorSchema.Response>(
				prompt,
				context,
				schemaGenerator.Schema,
				settings,
				true
			);

			return -response.Direction.Value();
		}

		public void MoveInDirection(Vector3 direction, AiNavigationSettingsVo settings, Action<bool> callback)
		{
			completed.OnNext(false);

			destination = Player.Camera.transform.position + direction;
			destination.y = Player.Mover.position.y;

			moveSpeed = settings.MoveSpeed;
			turnSpeed = settings.TurnSpeed;

			isFacingDestination = false;
			move = true;
			completed.Take(1).Subscribe(callback);
		}

		void Arrived ()
		{
			move = false;
			completed.OnNext(true);
		}

		// Update is used here so it can be run in edit mode
		void LateUpdate()
		{
			if (!move) return;

			Player.transform.position = Vector3.Lerp(Player.transform.position, destination, moveSpeed * Time.deltaTime);
			if (Vector3.Distance(Player.transform.position, destination) < 0.1f)
			{
				Arrived();
			}

			return;
			if (!isFacingDestination)
			{
				var directionToFace = (destination - Player.Camera.transform.position).normalized;
				directionToFace.y = 0; // Ensure rotation only around Y-axis
				var targetRotation = Quaternion.LookRotation(directionToFace);
				Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

				if (Quaternion.Angle(Player.transform.rotation, targetRotation) < 1f)
				{
					isFacingDestination = true;
				}
			}
			else
			{
				Player.transform.position = Vector3.Lerp(Player.transform.position, destination, moveSpeed * Time.deltaTime);
				if (Vector3.Distance(Player.transform.position, destination) < 0.1f)
				{
					Arrived();
				}
			}
		}
	}
}