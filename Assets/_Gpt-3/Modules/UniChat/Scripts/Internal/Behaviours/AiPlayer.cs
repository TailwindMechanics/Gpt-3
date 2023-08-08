using UnityEngine;

using Modules.UniChat.External.DataObjects.So;


namespace Modules.UniChat.Internal.Behaviours
{
    public class AiPlayer : MonoBehaviour
    {
        public ModelSettingsSo Model => model;
        public Camera Camera        => cam;
        public Transform Sensor     => sensor;
        public Transform Pointer    => pointer;

        [SerializeField] ModelSettingsSo model;
        [SerializeField] Camera cam;
        [SerializeField] Transform sensor;
        [SerializeField] Transform pointer;
    }
}