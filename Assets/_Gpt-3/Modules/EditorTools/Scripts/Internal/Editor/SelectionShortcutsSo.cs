#if UNITY_EDITOR

using UnityEditor.ShortcutManagement;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEditor;
using UnityEngine;

using Modules.Utilities.External;


namespace Modules.EditorTools.Internal.Editor
{
	public class SelectionShortcuts : ScriptableObject
	{
		[InfoBox("➝ Alt_0 always selects this asset\n➸ No need to remove (Clone) from names.")]
		[SerializeField] bool showLogs = true;
		[HideLabel, SerializeField] string shortcut1;
		[HideLabel, SerializeField] string shortcut2;
		[HideLabel, SerializeField] string shortcut3;
		[HideLabel, SerializeField] string shortcut4;
		[HideLabel, SerializeField] string shortcut5;
		[HideLabel, SerializeField] string shortcut6;
		[HideLabel, SerializeField] string shortcut7;
		[HideLabel, SerializeField] string shortcut8;
		[HideLabel, SerializeField] string shortcut9;

		string ObjectNames(int index)
			=> index switch
				{
					1 => shortcut1,
					2 => shortcut2,
					3 => shortcut3,
					4 => shortcut4,
					5 => shortcut5,
					6 => shortcut6,
					7 => shortcut7,
					8 => shortcut8,
					9 => shortcut9,
					_ => null
				};

		[Shortcut("Tailwind/Shortcuts/Selection_0", KeyCode.Alpha0, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_0() => SelectThisObject();
		[Shortcut("Tailwind/Shortcuts/Selection_1", KeyCode.Alpha1, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_1() => SetSelectedObject(GetObjectName(1));
		[Shortcut("Tailwind/Shortcuts/Selection_2", KeyCode.Alpha2, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_2() => SetSelectedObject(GetObjectName(2));
		[Shortcut("Tailwind/Shortcuts/Selection_3", KeyCode.Alpha3, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_3() => SetSelectedObject(GetObjectName(3));
		[Shortcut("Tailwind/Shortcuts/Selection_4", KeyCode.Alpha4, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_4() => SetSelectedObject(GetObjectName(4));
		[Shortcut("Tailwind/Shortcuts/Selection_5", KeyCode.Alpha5, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_5() => SetSelectedObject(GetObjectName(5));
		[Shortcut("Tailwind/Shortcuts/Selection_6", KeyCode.Alpha6, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_6() => SetSelectedObject(GetObjectName(6));
		[Shortcut("Tailwind/Shortcuts/Selection_7", KeyCode.Alpha7, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_7() => SetSelectedObject(GetObjectName(7));
		[Shortcut("Tailwind/Shortcuts/Selection_8", KeyCode.Alpha8, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_8() => SetSelectedObject(GetObjectName(8));
		[Shortcut("Tailwind/Shortcuts/Selection_9", KeyCode.Alpha9, ShortcutModifiers.Alt)]
		public static void SelectObjectOfName_9() => SetSelectedObject(GetObjectName(9));

		static readonly HexColor selectThis = new("#FFF4AD");
		static readonly HexColor hierarchy = new("#92FFF4");
		static readonly HexColor project = new("#92FFBB");

		static SelectionShortcuts instance;
		static SelectionShortcuts Instance
		{
			get
			{
				if (instance == null)
				{
					var guids = AssetDatabase.FindAssets("t:" + typeof(SelectionShortcuts).FullName);
					if (guids.Length == 0) instance = null;

					instance = AssetDatabase.LoadAssetAtPath<SelectionShortcuts>(AssetDatabase.GUIDToAssetPath(guids[0]));
				}
				return instance;
			}
		}

		static void SelectThisObject()
		{
			Selection.SetActiveObjectWithContext(Instance, Instance);
			// TailwindLog($"'{Instance.name}\' Project object selected.", selectThis);
		}

		static void TailwindLog (string message, HexColor hex)
		{
			if (!Instance.showLogs) return;

			Debug.Log($"<color={selectThis.Hex}><b>➝ </b></color><color={hex.Hex}><b>{message}</b></color>");
		}

		static void SetSelectedObject((string objName, SelectionShortcuts scriptableObj) tuple)
		{
			if (string.IsNullOrWhiteSpace(tuple.objName))
			{
				SelectThisObject();
				return;
			}

			var obj = FindSceneGameObject(tuple.objName);
			if (obj != null)
			{
				Selection.SetActiveObjectWithContext(obj, obj);

				TailwindLog($"'{obj.name}\' Hierarchy object selected.", hierarchy);
				return;
			}

			obj = FindProjectAsset(tuple.objName);
			if (obj != null)
			{
				Selection.SetActiveObjectWithContext(obj, obj);
				TailwindLog($"'{obj.name}\' Project object selected.", project);
				return;
			}

			SelectThisObject();
		}

		static (string, SelectionShortcuts) GetObjectName(int index)
		{
			var guids = AssetDatabase.FindAssets("t:" + typeof(SelectionShortcuts).FullName);
			var paths = guids.Select(AssetDatabase.GUIDToAssetPath).ToList();

			var scriptableObj = (SelectionShortcuts)AssetDatabase.LoadAssetAtPath(paths[0], typeof(SelectionShortcuts));
			return (scriptableObj.ObjectNames(index), scriptableObj);
		}

		static Object FindSceneGameObject (string gameObjectName)
		{
			Object result = GameObject.Find(gameObjectName);
			if (result == null) result = GameObject.Find(gameObjectName + "(Clone)");

			return result;
		}

		static Object FindProjectAsset (string assetName)
		{
			var guid = AssetDatabase.FindAssets(assetName);
			if (guid.Length < 1) return null;

			var assetPath = AssetDatabase.GUIDToAssetPath(guid[0]);
			var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
			return asset;
		}
	}
}

#endif