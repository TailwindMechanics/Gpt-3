#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
// using UnityToolbarExtender;


namespace Modules.EditorTools.Internal.Editor
{
	[InitializeOnLoad]
	public static class SceneSelectionToolbar
	{
		static List<SceneInfo> scenes;
		static SceneInfo sceneOpened;
		static int selectedIndex;
		static string[] displayedOptions;


		[Shortcut("Scene_01", KeyCode.Alpha1, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
		public static void OpenScene_01() => OpenScene(0);
		[Shortcut("Scene_02", KeyCode.Alpha2, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
		public static void OpenScene_02() => OpenScene(1);
		[Shortcut("Scene_03", KeyCode.Alpha3, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
		public static void OpenScene_03() => OpenScene(2);
		[Shortcut("Scene_04", KeyCode.Alpha4, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
		public static void OpenScene_04() => OpenScene(3);
		[Shortcut("Scene_05", KeyCode.Alpha5, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
		public static void OpenScene_05() => OpenScene(4);
		[Shortcut("Scene_06", KeyCode.Alpha6, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
		public static void OpenScene_06() => OpenScene(5);
		[Shortcut("Scene_07", KeyCode.Alpha7, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
		public static void OpenScene_07() => OpenScene(6);
		[Shortcut("Scene_08", KeyCode.Alpha8, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
		public static void OpenScene_08() => OpenScene(7);
		[Shortcut("Scene_09", KeyCode.Alpha9, ShortcutModifiers.Alt | ShortcutModifiers.Shift)]
		public static void OpenScene_09() => OpenScene(8);

		static SceneSelectionToolbar()
		{
			LoadFromPlayerPrefs();
			// ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
			EditorSceneManager.sceneOpened += HandleSceneOpened;
		}

		static void OnToolbarGUI()
		{
			GUILayout.FlexibleSpace();

			selectedIndex = EditorGUILayout.Popup(selectedIndex, displayedOptions);

			GUI.enabled = selectedIndex == 0;
			if (GUILayout.Button("+")) AddScene(sceneOpened);

			GUI.enabled = selectedIndex > 0;
			var index = selectedIndex - 1;

			if (GUILayout.Button("-"))
			{
				RemoveScene(index);
			}

			GUI.enabled = true;
			if (GUI.changed == false || selectedIndex <= 0 || scenes.Count <= index)
			{
				return;
			}

			var path = scenes[index].path;
			if (string.IsNullOrWhiteSpace(path))
			{
				RemoveScene(index);
			}
			else
			{
				EditorSceneManager.OpenScene(path);
			}
		}

		static void RefreshDisplayedOptions()
		{
			displayedOptions = new string[scenes.Count + 1];

			if (scenes.Count < 1)
			{
				displayedOptions[0] = "Click '+' to add a scene";
			}

			for (var i = 0; i < scenes.Count; i++)
			{
				displayedOptions[i + 1] = scenes[i].name + $" (shift_alt_{i+1})";
			}
		}

		static void HandleSceneOpened(Scene scene, OpenSceneMode mode) => SetOpenedScene(scene);

		static void SetOpenedScene(SceneInfo scene)
		{
			if (scene == null || string.IsNullOrEmpty(scene.path))
				return;

			for (var i = 0; i < scenes.Count; i++)
			{
				if (scenes[i].path != scene.path) continue;

				sceneOpened = scenes[i];
				selectedIndex = i + 1;
				SaveToPlayerPrefs(true);
				return;
			}

			sceneOpened = scene;
			selectedIndex = 0;
			SaveToPlayerPrefs(true);
		}

		static void SetOpenedScene(Scene scene)
			=> SetOpenedScene(new SceneInfo(scene));

		static void AddScene(SceneInfo scene)
		{
			if (scene == null)
				return;

			if (scenes.Any(s => s.path == scene.path))
			{
				RemoveScene(scene);
			}

			scenes.Add(scene);
			selectedIndex = scenes.Count;
			SetOpenedScene(scene);
			RefreshDisplayedOptions();
			SaveToPlayerPrefs();
		}

        static void RemoveScene (int index) => RemoveScene(scenes[index]);
		static void RemoveScene(SceneInfo scene)
		{
			scenes.Remove(scene);
			selectedIndex = 0;

			RefreshDisplayedOptions();
			SaveToPlayerPrefs();
		}

		static void SaveToPlayerPrefs(bool onlyLatestOpenedScene = false)
		{
			if (!onlyLatestOpenedScene)
			{
				var serialized = string.Join(";",
					scenes.Where(s => !string.IsNullOrEmpty(s.path)).Select(s => s.path));
				SetPref("SceneSelectionToolbar.Scenes", serialized);
			}

			if (sceneOpened != null)
				SetPref("SceneSelectionToolbar.LatestOpenedScene", sceneOpened.path);
		}

		static void LoadFromPlayerPrefs()
		{
			var serialized = GetPref("SceneSelectionToolbar.Scenes");

			scenes = serialized.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => new SceneInfo(s)).ToList();

			serialized = GetPref("SceneSelectionToolbar.LatestOpenedScene");

			if (!string.IsNullOrEmpty(serialized))
				SetOpenedScene(new SceneInfo(serialized));

			RefreshDisplayedOptions();
		}

		static void OpenScene (int index)
		{
			if (index < scenes.Count)
			{
				EditorSceneManager.OpenScene(scenes[index].path);
			}
		}

		static void SetPref(string name, string value) =>
			EditorPrefs.SetString($"{Application.productName}_{name}", value);

		static string GetPref(string name) => EditorPrefs.GetString($"{Application.productName}_{name}");

		[Serializable]
		class SceneInfo
		{
			public SceneInfo()
			{
			}

			public SceneInfo(Scene scene)
			{
				name = scene.name;
				path = scene.path;
			}

			public SceneInfo(string path)
			{
				name = System.IO.Path.GetFileNameWithoutExtension(path);
				this.path = path;
			}

			public string name;
			public string path;
		}
	}
}

#endif