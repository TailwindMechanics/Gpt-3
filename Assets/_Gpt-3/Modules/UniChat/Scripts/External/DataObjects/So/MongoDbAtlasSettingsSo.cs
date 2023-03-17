using Sirenix.OdinInspector;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.So
{
    [CreateAssetMenu(fileName = "new _mongoDbAtlasSettings", menuName = "Tailwind/MongoDb Atlas/Settings")]
    public class MongoDbAtlasSettingsSo : ScriptableObject
    {
        public MongoDbAtlasSettingsVo Vo => settings;
        [HideLabel, SerializeField] MongoDbAtlasSettingsVo settings;
    }
}