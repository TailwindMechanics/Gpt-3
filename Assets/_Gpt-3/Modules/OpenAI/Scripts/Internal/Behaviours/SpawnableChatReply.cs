using UnityEngine;
using TMPro;


namespace Modules.OpenAI.Internal.Behaviours
{
	public class SpawnableChatReply : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI titleText;
		[SerializeField] TextMeshProUGUI bodyText;

		public void SetText (string title, string body)
		{
			titleText.text = title;
			bodyText.text += body;
		}
	}
}