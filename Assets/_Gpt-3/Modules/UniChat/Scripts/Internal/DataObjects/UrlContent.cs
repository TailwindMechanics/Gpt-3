using System;


namespace Modules.UniChat.Internal.DataObjects
{
	[Serializable]
	public class UrlContent
	{
		public string Url { get; }
		public string Content { get; }

		public UrlContent (string url, string content)
		{
			Url = url;
			Content = content;
		}
	}
}