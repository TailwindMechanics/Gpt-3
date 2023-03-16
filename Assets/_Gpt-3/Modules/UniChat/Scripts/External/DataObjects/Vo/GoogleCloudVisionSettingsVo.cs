using UnityEngine;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
    [Serializable]
    public class GoogleCloudVisionSettingsVo
    {
        public string Url => $"{endpoint}?key={apiKey}";

        [TextArea(1,2), SerializeField] string apiKey;
        [TextArea(1,2), SerializeField] string endpoint;
    }
}