using Sirenix.OdinInspector;
using UnityEngine;


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


        public void TurnBy(float degreesDelta)
        {
            var rotatedForward = Quaternion.Euler(0, degreesDelta, 0) * player.forward;
            lookAtTarget = player.position + rotatedForward;
            allowUpdate = true;
        }


        public void GoToPosition(Vector3 newDestination)
        {
            var playerPos = player.position;
            newDestination.y = playerPos.y;
            destination = newDestination;

            var directionToDestination = (newDestination - playerPos).normalized;
            lookAtTarget = playerPos + directionToDestination * 1f;

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