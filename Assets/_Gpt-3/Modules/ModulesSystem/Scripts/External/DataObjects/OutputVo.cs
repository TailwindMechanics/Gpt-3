using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
using System;
using UniRx;


namespace Tailwind.ModulesSystem.External.DataObjects
{
	[Serializable, FoldoutGroup("Base Settings"), GUIColor("$GetModuleColor"), HideLabel]
	public class OutputVo<T>
	{
		public List<BaseModule> outputModules = new();
		public Type OutputType() => typeof(T);
		public List<T> outputs = new();
		BaseModule thisModule;

		public OutputVo (BaseModule module,  Action initializedCallback = null)
		{
			thisModule = module;
			thisModule.OutputType = OutputType();

			thisModule.onSceneLoaded
				.TakeUntilDestroy(thisModule)
				.Subscribe(_  => UpdateOutputs());

			thisModule.OtherModuleAddOutput
				.Where(component => component != null)
				.TakeUntilDestroy(thisModule)
				.Subscribe(AddOutput);

			UpdateOutputs(.01f, initializedCallback);
		}

		async void UpdateOutputs (float delay = 0f, Action initializedCallback = null)
		{
			if (delay > 0f) await Task.Delay(TimeSpan.FromSeconds(delay));

			var allModules = Object.FindObjectsOfType<BaseModule>().ToList();
			allModules.ForEach(UpdateIO);
			LogOutputs();

			initializedCallback?.Invoke();
		}

		void UpdateIO(BaseModule otherModule)
		{
			if (otherModule.OutputType != OutputType())
			{
				AddOutputForModule(otherModule, thisModule);
				AddOutputForModule(thisModule, otherModule);
			}
		}

		void AddOutputForModule (BaseModule queryModule, BaseModule moduleToAdd)
			=> moduleToAdd.OtherModuleAddOutput
				.OnNext(queryModule
					.GetComponent(moduleToAdd.OutputType));

		void AddOutput (Component component)
		{
			var outputComponent = (T)(object)component;
			if (outputs.Contains(outputComponent)) return;

			outputs.Add(outputComponent);
			outputModules.Add(component.GetComponent<BaseModule>());
		}

		void LogOutputs ()
		{
			if (outputModules.Count > 0)
			{
				thisModule.LogMessage("Total outputs: ", outputModules.Count.ToString());
			}
		}
	}
}