using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class WebDetection
	{
		[JsonProperty("webEntities")]
		public List<WebEntity> WebEntities { get; set; } = new();

		public override string ToString()
		{
			var result = "___";
			foreach (var webEntity in WebEntities)
			{
				result += $", {webEntity}";
			}
			return result.Replace("___, ", "");
		}
	}

	[Serializable]
	public class WebEntity
	{
		[JsonProperty("entityId")]
		public string EntityId { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("score")]
		public float Score { get; set; }

		public override string ToString()
			=> $"{Description}: {Score}";
	}
}