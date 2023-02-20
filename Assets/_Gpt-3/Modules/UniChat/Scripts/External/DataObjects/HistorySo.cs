using System.Collections.Generic;
using UnityEngine;

namespace Modules.UniChat.External.DataObjects
{
	[CreateAssetMenu(fileName = "new _history", menuName = "Tailwind/Chat/History")]
	public class HistorySo : ScriptableObject
	{
		public List<MessageVo> Data => data;
		[SerializeField] List<MessageVo> data;
	}
}