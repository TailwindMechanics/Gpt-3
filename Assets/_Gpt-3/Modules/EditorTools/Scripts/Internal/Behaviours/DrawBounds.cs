using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Modules.EditorTools.Internal.Behaviours
{
	[ExecuteAlways, CreateAssetMenu(menuName = "Tailwind/Tools/DrawBounds")]
	public class DrawBounds : ScriptableObject
	{
		[FoldoutGroup("Settings"), SerializeField] bool update;
		[FoldoutGroup("Settings"), SerializeField] bool drawBounds;
		[FoldoutGroup("Settings"), SerializeField] Material lineMaterial;
		[FoldoutGroup("Settings"), SerializeField] GameObject locatorSphere;
		[FoldoutGroup("Settings"), SerializeField] Transform currentTransform;
		[FoldoutGroup("Settings"), SerializeField] GameObject visualizationParent;
		[FoldoutGroup("Settings"), SerializeField] List<GameObject> lines = new();
		[BoxGroup("Settings/Bounds"), HideLabel, SerializeField] Bounds currentBounds;

#if UNITY_EDITOR
		void OnEnable()
			=> EditorApplication.update += Update;
		void OnDisable()
			=> EditorApplication.update -= Update;
#endif
		[Button(ButtonSizes.Medium)]
		public void Toggle()
		{
			drawBounds = !drawBounds;

			if (drawBounds) Init();
			else
			{
				update = false;
				CleanupVisualization();
			}
		}

		void Init()
		{
#if UNITY_EDITOR
			Debug.Log("<color=cyan><b>>>> Init</b></color>");

			currentTransform = Selection.activeTransform;

			var renderers = new List<Renderer>();
			foreach (var selected in Selection.gameObjects)
			{
				renderers.AddRange(selected.GetComponentsInChildren<Renderer>());
			}

			if (renderers.Count <= 0)
			{
				drawBounds = false;
				update = false;
				return;
			}

			currentBounds = GetLocalBounds(renderers[0]);
			for (var i = 1; i < renderers.Count; i++)
			{
				var childLocalBounds = GetLocalBounds(renderers[i]);
				currentBounds.Encapsulate(childLocalBounds.min);
				currentBounds.Encapsulate(childLocalBounds.max);
			}

			update = true;
#endif
		}

		void Update()
		{
			if (!update) return;

			var corners = GetBoundingBoxCorners();
			if (locatorSphere)
			{
				locatorSphere.transform.position = currentTransform.TransformPoint(currentBounds.center);
			}

			if (lines.Count == 0)
			{
				for (var i = 0; i < 12; i++)
				{
					lines.Add(CreateLine(Vector3.zero, Vector3.zero));
				}
			}

			SetLinesBetweenCorners(corners);
		}

		Vector3[] GetBoundingBoxCorners()
		{
			return new[]
			{
				currentTransform.TransformPoint(currentBounds.center + new Vector3(-currentBounds.extents.x,
					-currentBounds.extents.y, -currentBounds.extents.z)),
				currentTransform.TransformPoint(currentBounds.center + new Vector3(currentBounds.extents.x,
					-currentBounds.extents.y, -currentBounds.extents.z)),
				currentTransform.TransformPoint(currentBounds.center + new Vector3(currentBounds.extents.x,
					-currentBounds.extents.y, currentBounds.extents.z)),
				currentTransform.TransformPoint(currentBounds.center + new Vector3(-currentBounds.extents.x,
					-currentBounds.extents.y, currentBounds.extents.z)),
				currentTransform.TransformPoint(currentBounds.center + new Vector3(-currentBounds.extents.x,
					currentBounds.extents.y, -currentBounds.extents.z)),
				currentTransform.TransformPoint(currentBounds.center + new Vector3(currentBounds.extents.x,
					currentBounds.extents.y, -currentBounds.extents.z)),
				currentTransform.TransformPoint(currentBounds.center + new Vector3(currentBounds.extents.x,
					currentBounds.extents.y, currentBounds.extents.z)),
				currentTransform.TransformPoint(currentBounds.center + new Vector3(-currentBounds.extents.x,
					currentBounds.extents.y, currentBounds.extents.z))
			};
		}

		void SetLinesBetweenCorners(Vector3[] corners)
		{
			SetLinePosition(lines[0], corners[0], corners[1]);
			SetLinePosition(lines[1], corners[1], corners[2]);
			SetLinePosition(lines[2], corners[2], corners[3]);
			SetLinePosition(lines[3], corners[3], corners[0]);
			SetLinePosition(lines[4], corners[4], corners[5]);
			SetLinePosition(lines[5], corners[5], corners[6]);
			SetLinePosition(lines[6], corners[6], corners[7]);
			SetLinePosition(lines[7], corners[7], corners[4]);
			SetLinePosition(lines[8], corners[0], corners[4]);
			SetLinePosition(lines[9], corners[1], corners[5]);
			SetLinePosition(lines[10], corners[2], corners[6]);
			SetLinePosition(lines[11], corners[3], corners[7]);
		}

		GameObject CreateLine(Vector3 start, Vector3 end)
		{
			if (visualizationParent == null)
			{
				visualizationParent = new GameObject("VisualizationParent");
			}

			var line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			line.GetComponent<Renderer>().sharedMaterial = lineMaterial;
			line.transform.position = start;
			SetLinePosition(line, start, end);
			line.name = "BoundsLine";
			line.transform.SetParent(visualizationParent.transform);
			return line;
		}

		void SetLinePosition(GameObject line, Vector3 start, Vector3 end)
		{
			var midpoint = (start + end) / 2;
			var length = Vector3.Distance(start, end);
			var direction = (end - start).normalized;
			var rotation = Quaternion.FromToRotation(Vector3.up, direction);

			line.transform.position = midpoint;
			line.transform.rotation = rotation;
			line.transform.localScale = new Vector3(0.05f, length / 2, 0.05f);
		}

		void CleanupVisualization()
		{
			if (visualizationParent)
			{
				DestroyImmediate(visualizationParent);
				locatorSphere = null;
				lines.Clear();
			}
		}

		Bounds GetLocalBounds(Renderer renderer)
		{
			var meshFilter = renderer.GetComponent<MeshFilter>();
			if (!meshFilter) return new Bounds();

			var mesh = meshFilter.sharedMesh;
			var bounds = mesh.bounds;
			bounds.size = Vector3.Scale(bounds.size, renderer.transform.localScale);
			return bounds;
		}
	}
}