using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OpenAI.Chat;


namespace Modules.UniChat.Internal.DataObjects.Schemas
{
    public class MoveInDirectionFunction
    {
        public const string Name = "MoveAndOrTurn";

        public class ResponseSchema
        {
            [JsonProperty("heading_degrees")]
            public float HeadingDegrees { get; set; }

            [JsonProperty("travel_meters")]
            public float TravelMeters { get; set; }
        }

        public Function Function ()
            => FunctionSchema(Name,
                "You can use this to move and/or turn",
                "heading_degrees",
                "travel_meters");

        Function FunctionSchema(string functionName, string functionDescription, string heading, string travel)
            => new (
                functionName,
                functionDescription,
                new JObject
                {
                    ["type"] = "object",
                    ["properties"] = new JObject
                    {
                        [heading] = new JObject
                        {
                            ["type"] = "number",
                            ["description"] = "Your local rotation in degrees -360 to 360, 0:Forward, 180:Backward, -90:Left, 90:Right."
                        },
                        [travel] = new JObject
                        {
                            ["type"] = "number",
                            ["description"] = "The quantity of meters you want to advance from your current position, pass in zero if you just want to turn."
                        },
                    },
                    ["required"] = new JArray { functionName, heading, travel }
                }
            );
    }
}