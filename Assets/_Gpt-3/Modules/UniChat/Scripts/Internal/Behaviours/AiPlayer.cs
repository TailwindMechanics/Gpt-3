using Sirenix.OdinInspector;
using UnityEngine;


namespace Modules.UniChat.Internal.Behaviours
{
    public class AiPlayer : MonoBehaviour
    {
        [FoldoutGroup("References"), SerializeField]
        Camera cam;
        [FoldoutGroup("References"), SerializeField]
        Transform volumeSensor;
        [InlineEditor, SerializeField]
        ImageRecognizer imageRecognizer;

        [FoldoutGroup("Tools"), Button(ButtonSizes.Medium)]
        void CaptureSight ()
        {
            imageRecognizer.Capture(cam, volumeSensor);
        }
    }
}