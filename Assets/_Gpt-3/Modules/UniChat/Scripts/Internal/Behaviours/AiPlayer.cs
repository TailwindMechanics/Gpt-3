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

            Log($"OnFunctionReceived: {function.Name}");

            if (function.Name == TurnBySchema.Name)
            {
                var heading = (float) args[TurnBySchema.DegreesDelta];

                Log($"MoveInDirection: {heading}");
                abilities.TurnBy(heading);
            }
            else if (function.Name == GoToPositionSchema.Name)
            {
                var jToken = args[GoToPositionSchema.DestinationParam];
                if (jToken == null) return;

                var parsedDestination = jToken.ToObject<Vector3Serializable>();
                Log($"GoToPosition: {parsedDestination}");
                abilities.GoToPosition(parsedDestination.Value());
            }
        }

        void Log (string message)
            => Debug.Log($"<color=#976ccc><b>>>> AiPlayer: {message.Replace("\n", "")}</b></color>");
    }
}