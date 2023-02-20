using System;
using System.Linq;
using UniRx;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace Modules.EditorTools.Internal.Editor
{
    [Overlay(typeof(SceneView), overlayId, overlayTitle), Icon(iconPath)]
    public class TimeOfDaySceneOverlay : Overlay
    {
        const string iconPath = "Assets/_Tailwind/EditorTools/Data/Icons/icons8-time-machine-solid-100.png";
        const string overlayId = "tailwind_time_of_day";
        const string overlayTitle = "Time Of Day";

        readonly Subject<Unit> onSomethingChanged = new();

        int GetHierarchyGameObjectCount ()
            => Resources.FindObjectsOfTypeAll(typeof(GameObject))
                .Count(obj => (obj.hideFlags & HideFlags.HideInHierarchy) != HideFlags.HideInHierarchy);

        public override VisualElement CreatePanelContent()
        {
            var root        = new VisualElement();
            var timeField   = new Slider(label: "12:00", start: 0.0167f, end: 24f);

            root.style.width = new StyleLength(new Length(300, LengthUnit.Pixel));
            timeField.value = 12f;
            timeField.style.flexGrow = new StyleFloat(1f);
            timeField.RegisterValueChangedCallback(ctx =>
            {
                timeField.label = GetTimeAsString(ctx.newValue);
                // EditorToolsSingleton.Instance.SetTimeOfDay(ctx.newValue);
            });

            root.Add(timeField);

            return root;
        }

        string GetTimeAsString (float hours)
        {
            var span = TimeSpan.FromHours(hours);
            return string.Format(format: "{0:00}:{1:00}", span.Hours, span.Minutes);
        }

        void Later ()
        {
            onSomethingChanged
                .DelayFrame(1)
                .Select(_ => GetHierarchyGameObjectCount())
                .Pairwise()
                .Where(tuple => tuple.Current > 0)
                .Where(tuple => tuple.Current != tuple.Previous)
                .Subscribe(tuple =>
                {
                    Debug.Log($"<color=yellow><b>>>> EditorApplication.hierarchyChanged, old count: {tuple.Previous}, new count: {tuple.Current}</b></color>");

                });

            // CompilationPipeline.compilationFinished += _ => compileInProgress = false;
            EditorApplication.hierarchyChanged += () => onSomethingChanged.OnNext(Unit.Default);
        }
    }
}