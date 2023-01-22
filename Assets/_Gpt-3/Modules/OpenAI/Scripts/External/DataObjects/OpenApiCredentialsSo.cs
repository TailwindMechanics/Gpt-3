using Sirenix.OdinInspector;
using UnityEngine;


namespace Modules.OpenAI.External.DataObjects
{
	[CreateAssetMenu(fileName = "new _openApiCredentials", menuName = "Tailwind/OpenApi/Credentials")]
	public class OpenApiCredentialsSo : ScriptableObject
	{
		public OpenApiCredentialsVo Credentials => credentials;
		[HideLabel, SerializeField] OpenApiCredentialsVo credentials;
	}
}