using Sirenix.OdinInspector;
using UnityEngine;

using Modules.UniChat.External.DataObjects.So;


namespace Modules.UniChat.Internal.Behaviours
{
    public class AiPlayer : MonoBehaviour
    {
        public ModelSettingsSo Model    => model;
        public Camera Camera            => cam;
        public Transform Sensor         => sensor;
        public Transform Pointer        => pointer;
        public Transform PlayerMover    => playerMover;

        [FoldoutGroup("References"), SerializeField] Camera cam;
        [FoldoutGroup("References"), SerializeField] Transform sensor;
        [FoldoutGroup("References"), SerializeField] Transform pointer;
        [FoldoutGroup("References"), SerializeField] Transform playerMover;
        [FoldoutGroup("References"), InlineEditor, SerializeField]
        ModelSettingsSo model;

        [TextArea, SerializeField]
        string objective;
        [Button(ButtonSizes.Medium)]
        void SolveObjective()
        {

        }
    }
}