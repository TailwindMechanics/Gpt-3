using Sirenix.OdinInspector;
using UnityEngine;
using OpenAI.Chat;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.Behaviours
{
    public class AiPlayer : MonoBehaviour
    {
        public Transform Mover => playerMover;
        public Camera Camera => cam;

        [FoldoutGroup("References"), SerializeField]
        Camera cam;
        [FoldoutGroup("References"), SerializeField]
        Transform playerMover;

        AiAbilities Abilities => abilities ??= GetComponent<AiAbilities>();
        AiAbilities abilities;


        public void ReceivedFunction (Function function, ModelSettingsVo settings)
        {
            if (function == null)
            {
                Log($"No functions received.");
                return;
            }

            Abilities.ReceivedFunction(function, settings, OnFunctionComplete);
        }

        void OnFunctionComplete (string functionName, bool success)
        {
            Log($"OnFunctionComplete: {functionName}, success: {success}");
        }

        void Log (string message)
            => Debug.Log($"<color=#976ccc><b>>>> AiPlayer: {message.Replace("\n", "")}</b></color>");
    }
}