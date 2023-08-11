using UnityEngine;
using System;

using Modules.UniChat.External.DataObjects.Vo;
using Sirenix.OdinInspector;


namespace Modules.UniChat.Internal.Behaviours
{
    [RequireComponent(typeof(AiPlayer)), ExecuteAlways]
    public class AiAbilities : MonoBehaviour
    {
        Transform player;
        Transform Player => player ??= GetComponent<Transform>();

        [FoldoutGroup("Debug"), SerializeField] Vector3 lookAtTarget;
        [FoldoutGroup("Debug"), SerializeField] Vector3 destination;
        [FoldoutGroup("Debug"), SerializeField] bool allowUpdate;
        [FoldoutGroup("Debug"), SerializeField] float turnSpeed;
        [FoldoutGroup("Debug"), SerializeField] float moveSpeed;

        public void MoveInDirection(float headingDegrees, float travelMeters, AiNavigationSettingsVo settings, Action<bool> onComplete)
        {
            var rotatedForward = Quaternion.Euler(0, headingDegrees, 0) * Player.forward;
            var playerPos = Player.position;
            destination = playerPos + rotatedForward * travelMeters;
            lookAtTarget = playerPos + rotatedForward * (travelMeters + 1);
            allowUpdate = true;
        }

        void LateUpdate()
        {
            if (!allowUpdate) return;
            var lookComplete = false;
            var moveComplete = false;

            var targetRotation = Quaternion.LookRotation((lookAtTarget - Player.position).normalized);
            var angleDifference = Quaternion.Angle(Player.rotation, targetRotation);
            if (angleDifference > 1.0f)
            {
                Player.rotation = Quaternion.RotateTowards(Player.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
            else lookComplete = true;

            if (Vector3.Distance(Player.position, destination) > 0.1f)
            {
                Player.position = Vector3.MoveTowards(Player.position, destination, moveSpeed * Time.deltaTime);
            }
            else moveComplete = true;

            allowUpdate = !(lookComplete && moveComplete);
        }
    }
}