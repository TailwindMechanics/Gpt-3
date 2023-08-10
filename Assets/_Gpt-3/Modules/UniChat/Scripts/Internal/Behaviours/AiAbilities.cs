using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OpenAI.Chat;
using UnityEngine;
using System;
using UniRx;

using Modules.UniChat.External.DataObjects.Vo;


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


		public void ReceivedFunction(Function function, ModelSettingsVo settings, Action<string, bool> callback)
		{
			Log($" ReceivedFunction: {JsonConvert.SerializeObject(function)}");
			switch (function.Name)
			{
				case "MoveInDirection":
					var arguments = JObject.Parse(function.Arguments.ToString());
					var directionObj = arguments["direction"];
					if (directionObj == null)
					{
						Log($"No direction found in function: {function.Name}");
						callback(function.Name, false);
						return;
					}

					var directionSerializable = directionObj.ToObject<Vector3Serializable>();
					var direction = directionSerializable.Value();
					Log($"direction: {direction}");
					MoveInDirection(direction, settings.Navigation.Vo, success =>
					{
						callback(function.Name, success);
					});
					break;
			}
		}

		void MoveInDirection(Vector3 direction, AiNavigationSettingsVo settings, Action<bool> callback)
		{
			completed.OnNext(false);

			// Transform the direction vector based on the camera's rotation
			direction = Player.Camera.transform.TransformDirection(direction);

			var playerPos = Player.transform.position;
			destination = playerPos + direction;
			destination.y = Player.Mover.position.y;

			Log($"MoveInDirection: {direction}, destination: {destination}, distance: {Vector3.Distance(playerPos, destination)}");

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

		// todo Update is used here so it can be run in edit mode
		void LateUpdate()
		{
			if (!move) return;

			if (!isFacingDestination)
			{
				var directionToFace = (destination - Player.Camera.transform.position).normalized;
				directionToFace.y = 0;
				var targetRotation = Quaternion.LookRotation(directionToFace);
				Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

				if (Quaternion.Angle(Player.transform.rotation, targetRotation) < 1f)
				{
					isFacingDestination = true;
				}
			}
			else
			{
				// Use Vector3.MoveTowards to move at a constant speed
				Player.transform.position = Vector3.MoveTowards(Player.transform.position, destination, moveSpeed * Time.deltaTime);
				if (Vector3.Distance(Player.transform.position, destination) < 0.1f)
				{
					Arrived();
				}
			}
		}

		void Log (string message)
			=> Debug.Log($"<color=#d6952d><b>>>> AiAbilities: {message.Replace("\n", "")}</b></color>");
	}
}