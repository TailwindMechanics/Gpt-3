using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.Internal.DataObjects
{
	[Serializable]
	public class VectorDatabaseUpsertRequest
	{
		[JsonProperty("vectors")]
		public List<VectorDatabaseUpsertItem> Vectors { get; set; }

		[JsonProperty("namespace")]
		public string Namespace { get; set; }
	}
}