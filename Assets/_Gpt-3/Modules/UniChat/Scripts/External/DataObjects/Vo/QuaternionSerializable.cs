using Newtonsoft.Json;
using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class QuaternionSerializable
	{
		[JsonProperty("x")]
		public float X { get; set; }

		[JsonProperty("y")]
		public float Y { get; set; }

		[JsonProperty("z")]
		public float Z { get; set; }

		[JsonProperty("w")]
		public float W { get; set; }

		public QuaternionSerializable(Quaternion quaternion)
		{
			X = quaternion.x;
			Y = quaternion.y;
			Z = quaternion.z;
			W = quaternion.w;
		}

		public override string ToString()
			=> $"({X:F2},{Y:F2},{Z:F2},{W:F2})";
	}
}