using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OpenAI.Chat;


namespace Modules.UniChat.Internal.DataObjects.Schemas
{
    public class MoveInDirectionFunction
    {
        public const string Name = "MoveAndOrTurn";
        public const string Bearing = "agent_relative_bearing_degrees";
        public const string Travel = "agent_relative_travel_meters";
        public const string Description = "Agent can use this to move and/or turn.";

        public class ResponseSchema
        {
            [JsonProperty(Bearing)]
            public float BearingDegrees { get; set; }

            [JsonProperty(Travel)]
            public float TravelMeters { get; set; }
        }

        public Function Function ()
            => FunctionSchema(Name, Description, Bearing, Travel);

        Function FunctionSchema(string functionName, string functionDescription, string bearing, string travel)
            => new (
                functionName,
                functionDescription,
                new JObject
                {
                    ["type"] = "object",
                    ["properties"] = new JObject
                    {
                        [bearing] = new JObject
                        {
                            ["type"] = "number",
                            ["description"] = "Your relative bearing in degrees."
                        },
                        [travel] = new JObject
                        {
                            ["type"] = "number",
                            ["description"] = "The quantity of meters you want to advance from your current position, pass in zero if you just want to turn."
                        },
                    },
                    ["required"] = new JArray { functionName, bearing, travel }
                }
            );
    }
}