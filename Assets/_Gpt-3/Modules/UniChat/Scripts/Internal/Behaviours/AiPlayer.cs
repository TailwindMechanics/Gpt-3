using UnityEngine;


namespace Modules.UniChat.Internal.Behaviours
{
    public class AiPlayer : MonoBehaviour
    {
        public Camera Camera        => cam;
        public Transform Sensor     => sensor;
        public Transform Pointer    => pointer;

        [SerializeField] Camera cam;
        [SerializeField] Transform sensor;
        [SerializeField] Transform pointer;
    }
}