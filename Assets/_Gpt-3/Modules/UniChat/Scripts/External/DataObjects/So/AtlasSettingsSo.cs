using Sirenix.OdinInspector;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.External.DataObjects.So
{
    [CreateAssetMenu(fileName = "new _atlasSettings", menuName = "Tailwind/Atlas/Settings")]
    public class AtlasSettingsSo : ScriptableObject
    {
        public AtlasSettingsVo Vo => settings;
        [HideLabel, SerializeField] AtlasSettingsVo settings;
    }
}