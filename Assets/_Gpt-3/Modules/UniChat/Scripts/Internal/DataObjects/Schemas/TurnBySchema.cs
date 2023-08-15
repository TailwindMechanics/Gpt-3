using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OpenAI.Chat;

namespace Modules.UniChat.Internal.DataObjects.Schemas
{
	public class TurnBySchema
	{
		public const string Name = "TurnBy";
		public const string DegreesDelta = "y_rotation_degrees_delta";
		public const string Description = "Turns the agent by a relative/local Y-axis rotation in degrees. Values should be between -180 and 180 degrees.";

		public class ResponseSchema
		{
			[JsonProperty(DegreesDelta)]
			public float RotationDegreesDelta { get; set; }
		}

		public Function Function()
			=> FunctionSchema(Name, Description, DegreesDelta);

		Function FunctionSchema(string functionName, string functionDescription, string degreesDelta)
			=> new(
				functionName,
				functionDescription,
				new JObject
				{
					["type"] = "object",
					["properties"] = new JObject
					{
						[degreesDelta] = new JObject
						{
							["type"] = "number",
							["minimum"] = -180,
							["maximum"] = 180,
							["description"] = "Relative/local Y-axis rotation change in degrees. Values should be between -180 and 180."
						},
					},
					["required"] = new JArray { functionName, degreesDelta }
				}
			);
	}
}