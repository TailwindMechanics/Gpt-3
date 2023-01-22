using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Modules.ModulesSystem.External.Utilities;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Modules.ModulesSystem.External.DataObjects
{
	public abstract class BaseModule : MonoBehaviour
	{
		#region Fields
		[FoldoutGroup("Base Settings"), GUIColor("$GetModuleColor"), SerializeField]
		bool enableLogs = true;
		[FoldoutGroup("Base Settings"), GUIColor("$GetModuleColor"), SerializeField]
		string moduleName = "Module";
		[FoldoutGroup("Base Settings"), GUIColor("$GetModuleColor"), SerializeField]
		List<Color> moduleColors = new(){Color.white};

		[UsedImplicitly]
		protected Color GetModuleColor() => moduleColors[activeColourIndex];
		readonly int activeColourIndex = 0;
		#endregion Fields


		#region Output
		public abstract int OutputCount();
		public Type OutputType;
		public readonly ISubject<Scene> onSceneLoaded = new Subject<Scene>();
		public readonly ISubject<Component> OtherModuleAddOutput = new Subject<Component>();
		#endregion Output


		#region Unity Events
		void OnEnable()
			=> SceneManager.sceneLoaded += OnSceneLoaded;
		void OnDisable()
			=> SceneManager.sceneLoaded -= OnSceneLoaded;

		void OnSceneLoaded (Scene scene, LoadSceneMode loadSceneMode) => onSceneLoaded.OnNext(scene);
		#endregion Unity Events


		#region Editor
		public virtual void OnBakePrefab(){}
		public bool PlayingOffline => Application.isEditor && GameObject.Find("AppFlow") == null;
		#endregion Editor


		#region Logging
		public void LogMessage (string message, string colorMessage = "", int colorIndex = 0)
		{
			ModuleLog(message, colorMessage, GetLogColor(colorIndex));
		}

		public void LogAttention (string message, string colorMessage = "")
		{
			var hexColor = Color.red.ToHex();
			ModuleLog(message, colorMessage, hexColor);
		}

		public BaseModule LogMessage (string message, string colorMessage)
		{
			ModuleLog(message, colorMessage, GetLogColor(0));
			return this;
		}

		public BaseModule LogOutput (string message, string colorMessage)
		{
			ModuleLog($"{message}()", colorMessage, GetLogColor(0), ModuleLogType.OutputLog, OutputCount());
			return this;
		}

		public BaseModule Execute (Action output)
			{output.Invoke(); return this;}

		protected void LogEditor (string message, string colorMessage = "", int colorIndex = 0)
		{
			ModuleLog(message, colorMessage, GetLogColor(colorIndex), ModuleLogType.EditorLog);
		}

		void ModuleLog(string message, string colorMessage = "", string hexColor = "", ModuleLogType logType = ModuleLogType.RuntimeLog, int outputCount = 0)
		{
			if (enableLogs == false) return;

			if (string.IsNullOrWhiteSpace(message) == false
			&& string.IsNullOrWhiteSpace(colorMessage) == false)
			{
				message += ": ";
			}

			message = message?.Replace("\n", "");
			var log = logType switch
			{
				ModuleLogType.OutputLog when outputCount > 0 => $"<color={hexColor}><b>>>> {moduleName}:<color=white> {message}</color>{colorMessage}</b></color>",
				ModuleLogType.OutputLog => $"<color={hexColor}><b>>>> {moduleName}:<color=white> {message}</color> has no outputs.</b></color>",
				ModuleLogType.RuntimeLog => $"<color={hexColor}><b>>>> {moduleName}:<color=white> {message}</color>{colorMessage}</b></color>",
				ModuleLogType.EditorLog => $"<color={hexColor}><b>~~~ {moduleName}:<color=white> {message}</color>{colorMessage}</b></color>",
				_ => ""
			};

			Debug.Log(log);
		}

		string GetLogColor (int colourIndex)
		{
			return colourIndex >= moduleColors.Count
				? Color.white.ToHex()
				: moduleColors[colourIndex].ToHex();
		}
		#endregion Logging
	}
}