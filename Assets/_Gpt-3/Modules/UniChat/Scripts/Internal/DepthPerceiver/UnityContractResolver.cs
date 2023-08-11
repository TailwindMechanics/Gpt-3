using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;


namespace Modules.UniChat.Internal.DepthPerceiver
{
	public class UnityContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
	{
		protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(Type type, Newtonsoft.Json.MemberSerialization memberSerialization)
		{
			var props = base.CreateProperties(type, memberSerialization);
			return props.Where(p => p.DeclaringType != typeof(Vector2) && p.PropertyName != "normalized").ToList();
		}
	}
}