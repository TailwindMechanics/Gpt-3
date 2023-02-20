using System.Collections.Generic;
using System.Linq;
using Modules.Utilities.External;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif


namespace Modules.EditorTools.Internal.Editor
{
	internal static class MenuItems
	{
	#if UNITY_EDITOR
        [MenuItem("Tailwind/Tools/Collapse Inspector Components &c")] // hotkey = alt_c
        static void CollapseAllInspectorComponents()
        {
	        var tracker = ActiveEditorTracker.sharedTracker;
	        for (var i = 0; i < tracker.activeEditors.Length; i++)
	        {
		        tracker.SetVisible(i, 0);
		        tracker.activeEditors[i].Repaint();
	        }
        }

        [MenuItem("Tailwind/Tools/Convert Materials To Urp %&m")] // ctrl_alt_m
        static void FixMaterials ()
        {
		    var materials = new List<Material>();
		    var renderers = new List<Renderer>();

		    foreach (var gameObject in Selection.gameObjects)
		    {
			    var rends = gameObject.transform.GetComponentsInChildren<Renderer>().ToList();
			    rends.ForEach(rend => renderers.Add(rend));
		    }

	        if (renderers.Count < 1)
	        {
		        renderers = Object.FindObjectsOfType<Renderer>().ToList();
	        }

	        renderers.ForEach(rend => materials.Add(rend.sharedMaterial));
	        materials = materials.Distinct().ToList();

	        Debug.Log($"<color=yellow><b>>>> Materials found: {materials.Count}</b></color>");

	        Selection.objects = materials.ToArray();
	        EditorApplication.ExecuteMenuItem("Edit/Rendering/Materials/Convert Selected Built-in Materials to URP");
        }

        [MenuItem("Tailwind/Tools/Save All %&s")] // ctr_alt_s
		static void SaveAllAndClear()
			=> EditorSave.SaveAllAndClear();

		[MenuItem("Tailwind/Tools/Clear Console %&c")] // ctrl_alt_c
		static void ClearConsole()
			=> EditorSave.ClearConsole();

		[MenuItem("Tailwind/Tools/Reset Position And Rotation %&r")] // ctrl_alt_r
		static void ZeroOutTransform()
			=> Selection.gameObjects.ToList().ForEach(selected => selected.transform.ZeroOutLocalPositionAndRotation());
	#endif
	}
}