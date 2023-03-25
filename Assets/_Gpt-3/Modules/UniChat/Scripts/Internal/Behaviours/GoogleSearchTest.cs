using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;

using Modules.UniChat.External.DataObjects.Interfaces;
using Modules.UniChat.External.DataObjects.So;
using Modules.UniChat.Internal.Apis;
using Modules.Utilities.External;


namespace Modules.UniChat.Internal.Behaviours
{
    public class GoogleSearchTest : MonoBehaviour
    {
        [FolderPath, SerializeField] string savePath;
        [FolderPath, SerializeField] string fileName;
        [SerializeField] WebSearchSettingsSo settings;
        [SerializeField, TextArea] string query;

        [Button(ButtonSizes.Medium)] async void DoSearch ()
        {
            var api = new WebSearchApi(settings.Vo) as IGoogleSearchApi;
            var result = await api.Search(query, true);
            JsonUtilities.SaveAsJsonFile(savePath, fileName, result);

            AssetDatabase.Refresh();
        }
    }
}