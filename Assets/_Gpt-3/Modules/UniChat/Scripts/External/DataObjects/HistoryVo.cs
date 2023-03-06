using System.Collections.Generic;
using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects
{
	[Serializable]
	public class HistoryVo
	{
		public void Clear () => history.Clear();
		public List<MessageVo> Data => history;
		[SerializeField] List<MessageVo> history;
	}
}