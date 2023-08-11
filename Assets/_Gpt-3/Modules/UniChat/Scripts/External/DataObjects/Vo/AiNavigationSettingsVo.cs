using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
	[Serializable]
	public class AiNavigationSettingsVo
	{
		public float MoveSpeed => moveSpeed * 10f;
		public float TurnSpeed => turnSpeed * 2f;

		[Range(0f, 1f), SerializeField] float moveSpeed = 1f;
		[Range(0f, 1f), SerializeField] float turnSpeed = 1f;
	}
}