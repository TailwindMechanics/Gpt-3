#if UNITY_EDITOR

using Modules.Utilities.External;
using UnityEditor;
using UnityEngine;

namespace Modules.EditorTools.Internal.Editor
{
	[CustomPropertyDrawer(typeof(HexColor))]
	public class HexColorDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			var hex = property.FindPropertyRelative("hexCode").stringValue;
			var color = new HexColor("#" + hex).Color;

			EditorGUI.BeginChangeCheck();
			color = EditorGUI.ColorField(position, label, color, true, true, false);
			if (EditorGUI.EndChangeCheck())
			{
				hex = ColorUtility.ToHtmlStringRGB(color);
				property.FindPropertyRelative("hexCode").stringValue = hex;
			}

			EditorGUI.EndProperty();
		}
	}
}

#endif