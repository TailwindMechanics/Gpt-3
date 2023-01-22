using UnityEngine;


namespace Modules.OpenAI.External.DataObjects
{
	[CreateAssetMenu(fileName = "new _openApiCredentials", menuName = "Tailwind/OpenApi/Credentials")]
	public class OpenApiCredentialsSo : ScriptableObject
	{
		public string ApiKey => apiKey;
		[SerializeField] string apiKey;
	}
}