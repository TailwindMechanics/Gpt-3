using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class HistoryVo
	{
		public List<MessageVo> GetMostRecent(int count, bool logging = false)
		{
			var result = new List<MessageVo>();
			var itemCount = Mathf.Min(count, Data.Count);
			for (var i = Data.Count - 1; i >= Data.Count - itemCount; i--)
			{
				result.Add(Data[i]);
			}

			if (logging)
			{
				Log($"Got {result.Count} most recent of {count} requested.");
			}

			return result;
		}

		public List<MessageVo> GetManyByIdList (List<Guid> queries, bool logging = false)
		{
			var result = new List<MessageVo>();
			foreach (var log in Data)
			{
				if (log.Id == Guid.Empty) continue;

				foreach (var query in queries)
				{
					if (query != log.Id) continue;

					result.Add(log);
				}
			}

			if (!logging) return result;

			foreach (var vo in result)
			{
				Log($"Match: {vo.SenderName}, {vo.Message}");
			}

			Log($"Found {result.Count} matches of {queries.Count} queries.");

			return result;
		}

		public MessageVo GetById (Guid query)
			=> Data.FirstOrDefault(item => item.Id == query);
		public void Add (MessageVo newMessage, bool logging = false)
		{
			history.Add(newMessage);

			if (logging)
			{
				Log($"Added new entry: {JsonUtility.ToJson(newMessage)}");
			}
		}

		public void Clear ()
			=> history.Clear();
		public List<MessageVo> Data
			=> history;

		[SerializeField] List<MessageVo> history;

		void Log (string message)
			=> Debug.Log($"<color=#ffb6c1><b>>>> HistoryVo: {message.Replace("\n", "")}</b></color>");
	}
}