using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

using Modules.UniChat.External.DataObjects.So;
using Modules.Utilities.External;


namespace Modules.UniChat.Internal.Behaviours
{
    public class AiPlayer : MonoBehaviour
    {
        public ModelSettingsSo Model    => model;
        public Camera Camera            => cam;
        public Transform Sensor         => sensor;
        public Transform Pointer        => pointer;
        public Transform Mover    => playerMover;

        [FoldoutGroup("References"), SerializeField] Camera cam;
        [FoldoutGroup("References"), SerializeField] Transform sensor;
        [FoldoutGroup("References"), SerializeField] Transform pointer;
        [FoldoutGroup("References"), SerializeField] Transform playerMover;
        [FoldoutGroup("References"), InlineEditor, SerializeField]
        ModelSettingsSo model;


        [TextArea, SerializeField]
        string prompt;
        [Button(ButtonSizes.Medium)]
        async void SolveObjective()
        {
            var aiAbilities = GetComponent<AiAbilities>();
            Log("Capturing vision data...");
            visionResult = await aiAbilities.CaptureVision(Camera, Sensor, Model.Vo.Perception.Vo);
            Log("Deducing direction...");
            direction = await aiAbilities.DeduceDirection(prompt, visionResult, Model.Vo);
            Log($"Moving in direction: {direction}");
            aiAbilities.MoveInDirection(direction, Model.Vo.Navigation.Vo, OnComplete);
        }
        [FoldoutGroup("$VisionDataLabel"), HideLabel, TextArea(20, 20), PropertyOrder(1), SerializeField]
        string visionResult;
        [UsedImplicitly]
        string VisionDataLabel => $"Vision Data ({StringUtilities.Ellipses(visionResult)})";

        [SerializeField] Vector3 direction;
        [Button(ButtonSizes.Medium)]
        void ForceMove ()
        {
            var aiAbilities = GetComponent<AiAbilities>();
            aiAbilities.MoveInDirection(direction, Model.Vo.Navigation.Vo, OnComplete);
        }

        void OnComplete (bool arrived)
        {
            Log($"OnComplete, arrived at new destination: {arrived}");
        }

        void Log (string message)
            => Debug.Log($"<color=#976ccc><b>>>> AiPlayer: {message.Replace("\n", "")}</b></color>");
    }
}