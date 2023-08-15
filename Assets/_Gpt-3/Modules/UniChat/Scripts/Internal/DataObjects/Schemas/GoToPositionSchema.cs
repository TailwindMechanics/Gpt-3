using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OpenAI.Chat;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.DataObjects.Schemas
{
    public class GoToPositionSchema
    {
        public const string Name = "GoToPosition";
        public const string DestinationParam = "destination";
        public const string Description = "Agent can use this to move to a destination world position.";

        public class ResponseSchema
        {
            [JsonProperty(DestinationParam)]
            public Vector3Serializable Destination { get; set; }
        }

        public Function Function ()
            => FunctionSchema(Name, Description, DestinationParam);

        Function FunctionSchema(string functionName, string functionDescription, string destination)
            => new (
                functionName,
                functionDescription,
                new JObject
                {
                    ["type"] = "object",
                    ["properties"] = new JObject
                    {
                        [destination] = new JObject
                        {
                            ["type"] = "object",
                            ["description"] = "World destination position to move to.",
                            ["properties"] = new JObject
                            {
                                ["x"] = new JObject
                                {
                                    ["type"] = "number",
                                    ["description"] = "X coordinate."
                                },
                                ["y"] = new JObject
                                {
                                    ["type"] = "number",
                                    ["description"] = "Y coordinate."
                                },
                                ["z"] = new JObject
                                {
                                    ["type"] = "number",
                                    ["description"] = "Z coordinate."
                                }
                            },
                            ["required"] = new JArray { "x", "y", "z" }
                        }
                    },
                    ["required"] = new JArray { destination }
                }
            );
    }
}