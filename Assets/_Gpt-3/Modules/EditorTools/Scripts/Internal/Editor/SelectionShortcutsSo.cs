#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Modules.EditorTools.Internal.Editor
{
	// [CreateAssetMenu(fileName = "SelectionShortcuts", menuName = "Tailwind/Tools/New SelectionShortcuts")]
	public class SelectionShortcutsSo : ScriptableObject
	{
		public List<string> GameObjectNames = new();

		[Shortcut("Tailwind/Shortcuts/Selection_00", KeyCode.Alpha0, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_0() => SetSelectedObject(GetObjectName(-1));
		[Shortcut("Tailwind/Shortcuts/Selection_01", KeyCode.Alpha1, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_1() => SetSelectedObject(GetObjectName(0));
		[Shortcut("Tailwind/Shortcuts/Selection_02", KeyCode.Alpha2, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_2() => SetSelectedObject(GetObjectName(1));
		[Shortcut("Tailwind/Shortcuts/Selection_03", KeyCode.Alpha3, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_3() => SetSelectedObject(GetObjectName(2));
		[Shortcut("Tailwind/Shortcuts/Selection_04", KeyCode.Alpha4, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_4() => SetSelectedObject(GetObjectName(3));
		[Shortcut("Tailwind/Shortcuts/Selection_05", KeyCode.Alpha5, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_5() => SetSelectedObject(GetObjectName(4));
		[Shortcut("Tailwind/Shortcuts/Selection_06", KeyCode.Alpha6, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_6() => SetSelectedObject(GetObjectName(5));
		[Shortcut("Tailwind/Shortcuts/Selection_07", KeyCode.Alpha7, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_7() => SetSelectedObject(GetObjectName(6));
		[Shortcut("Tailwind/Shortcuts/Selection_08", KeyCode.Alpha8, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_8() => SetSelectedObject(GetObjectName(7));
		[Shortcut("Tailwind/Shortcuts/Selection_09", KeyCode.Alpha9, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_9() => SetSelectedObject(GetObjectName(8));



		static void SetSelectedObject((string objName, SelectionShortcutsSo scriptableObj) tuple)
		{
			if (string.IsNullOrWhiteSpace(tuple.objName)) return;

			Object obj = GameObject.Find(tuple.objName);
			if (obj == null) obj = GameObject.Find(tuple.objName + "(Clone)");

			if (obj != null)
			{
				Selection.SetActiveObjectWithContext(obj, obj);
				Debug.Log($"<color=green><b>>>> \'{obj.name}\' <color=white> Hierarchy</color> object selected.</b></color>");
				return;
			}

			obj = FindProjectAsset(tuple.objName);

			if (obj != null)
			{
				Selection.SetActiveObjectWithContext(obj, obj);
				Debug.Log($"<color=green><b>>>> \'{obj.name}\' <color=yellow> Project</color> object selected.</b></color>");
				return;
			}

			PrintLog(tuple.objName, tuple.scriptableObj);
		}

		static (string, SelectionShortcutsSo) GetObjectName(int index)
		{
			var guids = AssetDatabase.FindAssets("t:" + typeof(SelectionShortcutsSo).FullName);
			var paths = guids.Select(AssetDatabase.GUIDToAssetPath).ToList();

			var scriptableObj = (SelectionShortcutsSo)AssetDatabase.LoadAssetAtPath(paths[0], typeof(SelectionShortcutsSo));
			if (index < 0 || index >= scriptableObj.GameObjectNames.Count)
			{
				PrintLog(null, scriptableObj);
				return (null, scriptableObj);
			}

			return (scriptableObj.GameObjectNames[index], scriptableObj);
		}

		static void PrintLog(string targetName, SelectionShortcutsSo selectionObj)
		{
			if (string.IsNullOrWhiteSpace(targetName)) targetName = "untitled";

			Debug.Log($"<color=orange><b>~~~\'{targetName}\' not found.</b></color>\n<color=orange></color>", selectionObj);

			if (selectionObj != null)
			{
				Selection.SetActiveObjectWithContext(selectionObj, selectionObj);
			}
		}

		static Object FindProjectAsset (string assetName)
		{
			var guid = AssetDatabase.FindAssets(assetName);
			var assetPath = AssetDatabase.GUIDToAssetPath(guid[0]);
			var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
			return asset;
		}
	}
}

#endif