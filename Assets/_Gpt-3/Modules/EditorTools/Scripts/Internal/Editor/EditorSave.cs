using System.Reflection;
using Modules.ModulesSystem.External.DataObjects;
using Modules.ModulesSystem.External.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Modules.EditorTools.Internal.Editor
{
	internal static class EditorSave
	{
		internal static void SaveAllAndClear()
		{
			ClearConsole();
			SaveAll();
		}

		internal static void ClearConsole()
		{
			var assembly    = Assembly.GetAssembly(typeof(SceneView));
			var type        = assembly.GetType("UnityEditor.LogEntries");
			var method      = type.GetMethod("Clear");

			method?.Invoke(new object(), null);
		}

		static void SaveAll()
		{
			var logColor = Random.ColorHSV().ToHex();
			if (Application.isPlaying)
			{
				Debug.Log($"<color={logColor}><b>Cannot save during Play mode.</b></color>");
				return;
			}

			var bakedModulesCount	= BakePrefabs("Modules/Parent");

			SaveScene();
			SaveProject();

			Debug.Log($"<color={logColor}><b>~~~Scene and project saved. {bakedModulesCount} modules baked.</b></color>");
		}

		static void SaveScene ()	=> EditorSceneManager.SaveOpenScenes();
		static void SaveProject ()	=> AssetDatabase.SaveAssets();

		static int BakePrefabs(string tag)
		{
			var modulesGameObjects = GameObject.FindGameObjectsWithTag(tag);
			var bakedModulesCount = 0;

			if (modulesGameObjects is { Length: > 0 })
			{
				foreach (var modulesGameObject in modulesGameObjects)
				{
					foreach (Transform module in modulesGameObject.transform)
					{
						if (PrefabUtility.IsPartOfAnyPrefab(module.gameObject) == false) continue;

						module.GetComponent<BaseModule>().OnBakePrefab();
						PrefabUtility.ApplyPrefabInstance(module.gameObject, InteractionMode.UserAction);
						bakedModulesCount++;
					}
				}
			}

			return bakedModulesCount;
		}
	}
}