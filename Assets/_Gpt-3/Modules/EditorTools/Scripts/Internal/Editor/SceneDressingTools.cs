#if UNITY_EDITOR

using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Modules.EditorTools.Internal.Editor
{
    public class SceneDressingTools : MonoBehaviour
    {
        #region Mover
        [FoldoutGroup("Mover"), SerializeField] Transform moverParent;
        [FoldoutGroup("Mover"), Button(ButtonSizes.Medium)]
        void MoveToParent ()
        {
            if (moverParent == null)
            {
                LogMessage("Mover parent is null");
                return;
            }

            foreach (var selected in Selection.gameObjects)
            {
                selected.transform.parent = moverParent;
            }

            LogMessage("Moved", $"{Selection.gameObjects.Length} objects.");
        }
        #endregion Mover


        #region Duplicator
        [FoldoutGroup("Duplicator"), SerializeField] Transform duplicatorParent;
        [FoldoutGroup("Duplicator"), SerializeField] bool preservePlacement;
        [FoldoutGroup("Duplicator"), SerializeField] bool allowDuplicates;
        [FoldoutGroup("Duplicator"), Button(ButtonSizes.Medium)]
        void Duplicate ()
        {
            if (duplicatorParent == null)
            {
                LogMessage("Working parent is null");
                return;
            }

            var pattern = @" \((.*?)\)";
            var count = 0;
            foreach (var selected in Selection.gameObjects)
            {
                if (allowDuplicates == false && WorkingParentAlreadyHasThisChild(selected))
                    continue;

                count++;
                var duplicate = Instantiate(selected, duplicatorParent, preservePlacement);
                if (preservePlacement == false)
                {
                    var circlePos = Random.insideUnitCircle * 10;
                    var spawnPos = new Vector3(circlePos.x, 0f, circlePos.y);
                    duplicate.transform.localPosition = spawnPos;
                }

                duplicate.name = Regex
                    .Replace(duplicate.name, pattern, string.Empty)
                    .Replace("(Clone)", "")
                    .Replace(" ", "");
            }

            if (count < 1) return;

            LogMessage("Duplicated", $"{count} items.");
            Selection.SetActiveObjectWithContext(duplicatorParent, this);
        }

        bool WorkingParentAlreadyHasThisChild (GameObject query)
        {
            foreach (Transform child in duplicatorParent)
            {
                var mesh = child.GetComponent<MeshFilter>().sharedMesh;
                var queryMesh = query.GetComponent<MeshFilter>().sharedMesh;

                if (mesh != queryMesh) continue;

                LogMessage("Already exists", child.name);
                return true;
            }

            return false;
        }
        #endregion Duplicator


        #region Distributor
        [FoldoutGroup("Distributor"), SerializeField] Transform distributorParent;
        [FoldoutGroup("Distributor"), Range(0f, 10f), SerializeField] float itemSpacing = 1f;
        [FoldoutGroup("Distributor"), Range(0f, 15f), SerializeField] float rowSpacing = 1f;
        [FoldoutGroup("Distributor"), Range(1, 9), SerializeField] int distributorRowLength;
        [FoldoutGroup("Distributor"), Button(ButtonSizes.Medium)]
        void Distribute ()
        {
            if (distributorParent == null)
            {
                LogMessage("Working parent is null");
                return;
            }

            var itemCount = distributorParent.childCount;

            for (var i = 0; i < itemCount; i++)
            {
                var zPos = i / distributorRowLength;
                var xPos = i % distributorRowLength - distributorRowLength / 2;

                var targetPosition = new Vector3(xPos * itemSpacing, 0, zPos * -rowSpacing);
                distributorParent.GetChild(i).localPosition = targetPosition;
            }
        }
        #endregion Distributor


        #region Workers
        void LogMessage (string a, string b = "")
            => Debug.Log($"<color=yellow><b>>>> {a}, {b}</b></color>");
        #endregion Workers
    }
}

#endif