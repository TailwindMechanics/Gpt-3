using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.Internal.DataObjects
{
	[Serializable]
	public class VectorDatabaseUpsertItem
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("metadata")]
		public Dictionary<string, object> Metadata { get; set; }

		[JsonProperty("values")]
		public List<double> Values { get; set; }
	}
}