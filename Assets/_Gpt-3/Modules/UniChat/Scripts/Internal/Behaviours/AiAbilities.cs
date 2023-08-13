using UnityEngine;
using System;
using Modules.UniChat.External.DataObjects.Vo;
using Sirenix.OdinInspector;


namespace Modules.UniChat.Internal.Behaviours
{
    [RequireComponent(typeof(AiPlayer)), ExecuteAlways]
    public class AiAbilities : MonoBehaviour
    {
        [SerializeField] Transform player;
        [FoldoutGroup("Debug"), SerializeField] Vector3 lookAtTarget;
        [FoldoutGroup("Debug"), SerializeField] Vector3 destination;
        [FoldoutGroup("Debug"), SerializeField] bool allowUpdate;
        [FoldoutGroup("Debug"), SerializeField] float turnSpeed;
        [FoldoutGroup("Debug"), SerializeField] float moveSpeed;

        public void MoveInDirection(float bearingDegrees, float travelMeters, AiNavigationSettingsVo settings, Action<bool> onComplete)
        {
            var rotatedForward = Quaternion.Euler(0, bearingDegrees, 0) * player.forward;
            var playerPos = player.position;
            destination = playerPos + rotatedForward * travelMeters;
            lookAtTarget = playerPos + rotatedForward * (travelMeters + 1);
            allowUpdate = true;
        }

        void LateUpdate()
        {
            if (!allowUpdate) return;
            var lookComplete = false;
            var moveComplete = false;

            var targetRotation = Quaternion.LookRotation((lookAtTarget - player.position).normalized);
            var angleDifference = Quaternion.Angle(player.rotation, targetRotation);
            if (angleDifference > 1.0f)
            {
                player.rotation = Quaternion.RotateTowards(player.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
            else lookComplete = true;

            if (Vector3.Distance(player.position, destination) > 0.1f)
            {
                player.position = Vector3.MoveTowards(player.position, destination, moveSpeed * Time.deltaTime);
            }
            else moveComplete = true;

            allowUpdate = !(lookComplete && moveComplete);
        }
    }
}