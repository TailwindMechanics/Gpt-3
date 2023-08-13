using Newtonsoft.Json.Linq;
using OpenAI.Chat;
using UnityEngine;

using Modules.UniChat.Internal.DataObjects.Schemas;
using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.Behaviours
{
    public class AiPlayer : MonoBehaviour
    {
        public Camera Camera => cam;

        [SerializeField] Camera cam;
        [SerializeField] AiAbilities abilities;


        public void OnFunctionReceived(Function function, ModelSettingsVo settings)
        {
            var args = JObject.Parse(function.Arguments.ToString());
            var bearing = (float) args[MoveInDirectionFunction.Bearing];
            var travel = (float) args[MoveInDirectionFunction.Travel];

            Log($"OnFunctionReceived: {function.Name}");

            if (function.Name == MoveInDirectionFunction.Name)
            {
                Log($"MoveInDirection: {bearing}, {travel}");
                abilities.MoveInDirection(bearing, travel, settings.Navigation.Vo, arrived =>
                {
                    Log($"MoveInDirection, arrived: {arrived}");
                });
            }
        }

        void Log (string message)
            => Debug.Log($"<color=#976ccc><b>>>> AiPlayer: {message.Replace("\n", "")}</b></color>");
    }
}