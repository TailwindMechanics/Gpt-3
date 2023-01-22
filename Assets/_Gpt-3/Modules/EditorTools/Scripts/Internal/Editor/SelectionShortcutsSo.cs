#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Modules.EditorTools.Internal.Editor
{
	// [CreateAssetMenu(fileName = "SelectionShortcuts", menuName = "Tailwind/Tools/New SelectionShortcuts")]
	public class SelectionShortcutsSo : ScriptableObject
	{
		public List<string> GameObjectNames = new();

		[MenuItem("Tailwind/Tools/Selection/Shortcut 1 &1")] // alt_1
		public static void SelectObjectOfName_1() => SetSelectedObject(GetObjectName(0));
		[MenuItem("Tailwind/Tools/Selection/Shortcut 2 &2")] // alt_2
		public static void SelectObjectOfName_2() => SetSelectedObject(GetObjectName(1));
		[MenuItem("Tailwind/Tools/Selection/Shortcut 3 &3")] // alt_3
		public static void SelectObjectOfName_3() => SetSelectedObject(GetObjectName(2));
		[MenuItem("Tailwind/Tools/Selection/Shortcut 4 &4")] // alt_4
		public static void SelectObjectOfName_4() => SetSelectedObject(GetObjectName(3));
		[MenuItem("Tailwind/Tools/Selection/Shortcut 5 &5")] // alt_5
		public static void SelectObjectOfName_5() => SetSelectedObject(GetObjectName(4));



		static void SetSelectedObject((string objName, SelectionShortcutsSo scriptableObj) tuple)
		{
			if (string.IsNullOrWhiteSpace(tuple.objName)) return;

			var obj = GameObject.Find(tuple.objName);
			if (obj == null) obj = GameObject.Find(tuple.objName + "(Clone)");

			if (obj != null)
			{
				Selection.SetActiveObjectWithContext(obj, obj);
				Debug.Log($"<color=green><b>~~~\'{obj.name}\' selected.</b></color>");
			}
			else PrintLog(tuple.objName, tuple.scriptableObj);
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

			Debug.Log($"<color=orange><b>~~~\'{targetName}\' not found.</b></color>\n<color=orange><b>(Names defined on SelectionShortcuts asset).</b></color>", selectionObj);

			if (selectionObj != null)
			{
				Selection.SetActiveObjectWithContext(selectionObj, selectionObj);
			}
		}
	}
}

#endif