using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OpenAI.Chat;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.DataObjects.Schemas
{
	public class VectorFunctionSchema
	{
		public Function Function { get; private set; }

		public class Response
		{
			[JsonProperty("direction")]
			public Vector3Serializable Direction { get; set; }
		}

		public VectorFunctionSchema(string functionName, string functionDescription, string vectorKeyName)
			=> Function = GenerateFunction(functionName, functionDescription, vectorKeyName);

		Function GenerateFunction(string functionName, string functionDescription, string vectorKeyName)
		{
			return new Function(
				functionName,
				functionDescription,
				new JObject
				{
					["type"] = "object",
					["properties"] = new JObject
					{
						[vectorKeyName] = new JObject
						{
							["type"] = "object",
							["properties"] = new JObject
							{
								["x"] = new JObject
								{
									["type"] = "number",
									["description"] = $"The x component of the {vectorKeyName} vector."
								},
								["y"] = new JObject
								{
									["type"] = "number",
									["description"] = $"The y component of the {vectorKeyName} vector."
								},
								["z"] = new JObject
								{
									["type"] = "number",
									["description"] = $"The z component of the {vectorKeyName} vector."
								}
							},
							["required"] = new JArray { "x", "y", "z" }
						}
					},
					["required"] = new JArray { vectorKeyName }
				}
			);
		}
	}
}