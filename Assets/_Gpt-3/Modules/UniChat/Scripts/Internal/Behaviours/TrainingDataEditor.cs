using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using System;

using Modules.Utilities.External;
using Newtonsoft.Json;


namespace Modules.UniChat.Internal.Behaviours
{
	[Serializable]
	public class TrainingData
	{
		const int LabelCharacterLimit = 40;

		[JsonProperty("input")]
		public string Input => input;
		[JsonProperty("output")]
		public string Output => output;

		[FoldoutGroup("$Label"), TextArea(3,3), SerializeField]
		string input;
		[FoldoutGroup("$Label"), TextArea(8,8), SerializeField]
		string output;

		public TrainingData(string newInput, string newOutput)
		{
			input = newInput;
			output = newOutput;
		}

		[UsedImplicitly] string Label => Input.Length <= LabelCharacterLimit ? Input : Input[..LabelCharacterLimit] + "..." ;
	}

	[Serializable]
	public class TrainingSet
	{
		[JsonProperty("training_data")]
		public List<TrainingData> TrainingData
			=> trainingData;

		public void Add(TrainingData data)
			=> trainingData.Add(data);

		[SerializeField]
		List<TrainingData> trainingData;
	}

	[CreateAssetMenu(fileName = "new _trainingDataEditor", menuName = "Tailwind/Training/Data Editor")]
	public class TrainingDataEditor : ScriptableObject
	{
		[FoldoutGroup("Settings"), SerializeField]
		HexColor color;
		[FoldoutGroup("Settings"), SerializeField]
		string fileName;
		[FoldoutGroup("Settings"), FolderPath, SerializeField]
		string savePath;

		[FoldoutGroup("Edit"), TextArea(3,3), SerializeField]
		string input;
		[FoldoutGroup("Edit"), TextArea(8,8), SerializeField]
		string output;

		[FoldoutGroup("Training Set"), HideLabel, SerializeField]
		TrainingSet trainingSet;


		[FoldoutGroup("Edit"), ButtonGroup("Edit/Buttons"), Button(ButtonSizes.Medium)]
		void Add()
		{
			trainingSet.Add(new TrainingData(input, output));
			input = "";
			output = "";
		}

		[FoldoutGroup("Edit"), ButtonGroup("Edit/Buttons"), Button(ButtonSizes.Medium)]
		void Save ()
		{
			JsonUtilities.SaveAsJsonFile(savePath, fileName, trainingSet);
			AssetDatabase.Refresh();
		}
	}
}