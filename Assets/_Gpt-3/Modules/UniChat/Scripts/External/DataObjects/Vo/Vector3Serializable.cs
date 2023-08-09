using Newtonsoft.Json;
using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class Vector3Serializable
	{
		[JsonProperty("x")]
		public float X { get; set; }

		[JsonProperty("y")]
		public float Y { get; set; }

		[JsonProperty("z")]
		public float Z { get; set; }

		public Vector3Serializable(Vector3 vector3)
		{
			X = vector3.x;
			Y = vector3.y;
			Z = vector3.z;
		}

		public Vector3 Value()
			=> new(X, Y, Z);

		public override string ToString()
			=> $"({X:F2},{Y:F2},{Z:F2})";
	}
}