using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

using Modules.UniChat.External.DataObjects.Vo;


namespace Modules.UniChat.Internal.Behaviours
{
    [ExecuteAlways]
    public class ProjectMonitor : MonoBehaviour
    {
        [Range(.01f, 10f), SerializeField]
        float sampleSeconds = 3f;

        [TextArea(5,5), UsedImplicitly, SerializeField]
        string output;

        [UsedImplicitly]
        string buttonLabel => Application.isPlaying
            ? "Take Runtime snapshot" : "Take Editor snapshot";

        [Button("$buttonLabel", ButtonSizes.Medium), DisableIf("$processing")]
        void TakeSnapshotButton() => TakeSnapshot();

        int framesCaptured;
        bool processing;
        float elapsed;


        void Update()
        {
            if (!processing) return;

            elapsed += Time.deltaTime;
            framesCaptured++;

            if (elapsed < sampleSeconds) return;

            PackJson();
            processing = false;
        }

        void Reset ()
        {
            output = "...";
            framesCaptured = 0;
            elapsed = 0f;
        }

        void TakeSnapshot()
        {
            Reset();
            processing = true;
            Debug.Log(Application.isPlaying
                ? "<color=yellow><b>>>> Taking Runtime snapshot...</b></color>"
                : "<color=yellow><b>>>> Taking Editor snapshot...</b></color>");
        }

        void PackJson ()
        {
            processing = true;
            var json = Application.isPlaying ? new RuntimeSnapshotVo(sampleSeconds, framesCaptured).Json
                : new EditorSnapshotVo(sampleSeconds, framesCaptured).Json;

            var snapshotType = Application.isPlaying ? "Runtime" : "Editor";
            var log = $"{snapshotType}: {json}";
            Debug.Log($"<color=cyan><b>>>> {log}</b></color>");
            output = log;
        }
    }
}