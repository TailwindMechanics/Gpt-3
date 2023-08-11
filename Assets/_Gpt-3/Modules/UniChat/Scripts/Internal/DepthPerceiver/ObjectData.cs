using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.DepthPerceiver
{
	public class ObjectData
	{
		public string Name { get; set; }
		public Vector3Serializable WorldPosition { get; set; }
		public Vector3Serializable DirectionFromCamera { get; set; }
		public Vector3Serializable Size { get; set; }
	}
}