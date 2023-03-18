using Sirenix.OdinInspector;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class LabelAnnotation
	{
		[FoldoutGroup("$label"), JsonProperty("mid")]
		public string Mid;

		[FoldoutGroup("$label"), JsonProperty("description")]
		public string Description;

		[FoldoutGroup("$label"), JsonProperty("score")]
		public float Score;

		[FoldoutGroup("$label"), JsonProperty("topicality")]
		public float Topicality;

		[UsedImplicitly]
		string label => string.IsNullOrWhiteSpace(Description) ? "Label" : $"{Description}: {Score}";

		public override string ToString()
			=> $"{Description}, score: {Score}";
	}
}