using Random = UnityEngine.Random;
using UnityEditor.SceneManagement;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System;

using Tailwind.ModulesSystem.External.DataObjects;
using Tailwind.ModulesSystem.External.Utilities;


namespace Tailwind.EditorTools.Internal.Editor
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

		internal static async void SaveAll()
		{
			await Task.Delay(TimeSpan.FromSeconds(.01f));

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