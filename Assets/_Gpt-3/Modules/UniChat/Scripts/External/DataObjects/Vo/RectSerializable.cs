using Newtonsoft.Json;
using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class RectSerializable
	{
		[JsonProperty("x")]
		public float X { get; set; }

		[JsonProperty("y")]
		public float Y { get; set; }

		[JsonProperty("width")]
		public float Width { get; set; }

		[JsonProperty("height")]
		public float Height { get; set; }

		public RectSerializable(float x, float y, float width, float height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public Rect Value()
			=> new(X, Y, Width, Height);

		public override string ToString()
			=> $"({X:F2},{Y:F2},{Width:F2},{Height:F2})";
	}
}