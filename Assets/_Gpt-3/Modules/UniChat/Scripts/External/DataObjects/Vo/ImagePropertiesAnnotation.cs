using System.Collections.Generic;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class ImagePropertiesAnnotation
	{
		[JsonProperty("dominantColors")]
		public DominantColors DominantColors { get; set; }



		public override string ToString()
		{
			var colours = "___";
			foreach (var dominantColor in DominantColors.Colors)
			{
				colours += $" - {dominantColor.Color}, Score: {dominantColor.Score}, PixelFraction: {dominantColor.PixelFraction}";
			}

			return colours.Replace("___ - ", "");
		}
	}

	[Serializable]
	public class DominantColors
	{
		[JsonProperty("colors")]
		public List<ColorInfo> Colors { get; set; }
	}

	[Serializable]
	public class ColorInfo
	{
		[JsonProperty("color")]
		public Color Color { get; set; }

		[JsonProperty("score")]
		public float Score { get; set; }

		[JsonProperty("pixelFraction")]
		public float PixelFraction { get; set; }
	}

	[Serializable]
	public class Color
	{
		[JsonProperty("red")]
		public int Red { get; set; }

		[JsonProperty("green")]
		public int Green { get; set; }

		[JsonProperty("blue")]
		public int Blue { get; set; }

		public override string ToString()
			=> $"R:{Red},G:{Green},B:{Blue}:";
	}
}