using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
    [Serializable]
    public class OpenAiSettingsVo
    {
        public string ApiKey => apiKey;
        public string OrgId => orgId;

        [TextArea(2,3), SerializeField] string apiKey;
        [TextArea(2,3), SerializeField] string orgId;
    }
}