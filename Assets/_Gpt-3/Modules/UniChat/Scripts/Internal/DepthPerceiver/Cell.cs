using System.Collections.Generic;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.DepthPerceiver
{
	public class Cell
	{
		public List<ObjectData> ContainedObjects { get; set; }
		public RectSerializable Bounds { get; set; }
		public string Label { get; set; }
		public int ID { get; set; }
	}
}