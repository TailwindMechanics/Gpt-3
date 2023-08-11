using Newtonsoft.Json.Linq;
using UnityEngine;
using OpenAI.Chat;

using Modules.UniChat.External.DataObjects.Vo;
using Modules.UniChat.Internal.DataObjects.Schemas;


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
            var heading = (float) args["heading_degrees"];
            var travel = (float) args["travel_meters"];

            Log($"OnFunctionReceived: {function.Name}");

            if (function.Name == MoveInDirectionFunction.Name)
            {
                Log($"MoveInDirection: {heading}, {travel}");
                abilities.MoveInDirection(heading, travel, settings.Navigation.Vo, arrived =>
                {
                    Log($"MoveInDirection, arrived: {arrived}");
                });
            }
        }

        void Log (string message)
            => Debug.Log($"<color=#976ccc><b>>>> AiPlayer: {message.Replace("\n", "")}</b></color>");
    }
}