using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;
using System.IO;
using System;

#if UNITY_EDITOR
using UnityEditor.Animations;
using UnityEditor;
#endif


namespace Modules.EditorTools.External.DataObjects
{
    [CreateAssetMenu(fileName = "AssetDependencyDuplicator", menuName = "Tailwind/Character Lab/Asset Dependency Duplicator")]
    public class AssetDependencyDuplicatorSo : ScriptableObject
    {
        [UsedImplicitly, HideInInspector, SerializeField]
        bool processing = false;
        [FoldoutGroup("Process All"), ButtonGroup("Process All/buttons"), Button(ButtonSizes.Large), PropertyOrder(-1)]
        void ClearAll ()
        {
            ClearDependencies();
            ClearOutput();
            processing = false;
        }
        [FoldoutGroup("Process All"), ButtonGroup("Process All/buttons"), DisableIf("$processing"), Button(ButtonSizes.Large), PropertyOrder(-1)]
        async void ProcessAll ()
        {
            processing = true;

            DoGetDependencies();
            await Task.Delay(TimeSpan.FromSeconds(.25f));
            DoDuplicateDependencies();
            await Task.Delay(TimeSpan.FromSeconds(.25f));
            DoFixOutputDependencies();
            await Task.Delay(TimeSpan.FromSeconds(.25f));
            DoCheckDependencies();
            await Task.Delay(TimeSpan.FromSeconds(.25f));

            if (notFixedDependencies.Count < 1)
            {
                ClearDependencies();
                ClearOutput();
            }

            processing = false;
        }

        [FoldoutGroup("Settings"), InlineButton(nameof(ToggleEdit), "$editButtonLabel"), OnValueChanged(nameof(SetFolderNameToTargetName)), SerializeField]
        Object target;
        [FoldoutGroup("Settings"), EnableIf("$canEdit"), FolderPath, SerializeField]
        string destinationFolder;
        [FoldoutGroup("Settings"), EnableIf("$canEdit"), SerializeField]
        string newFolderName;
        [FoldoutGroup("Settings"), EnableIf("$canEdit"), SerializeField]
        string albedoMapName = "_BaseMap";
        [FoldoutGroup("Settings"), EnableIf("$canEdit"), SerializeField]
        string emissionMapName = "_EmissionMap";
        [FoldoutGroup("Settings"), EnableIf("$canEdit"), SerializeField]
        string normalMapName = "_BumpMap";
        [FoldoutGroup("Settings"), EnableIf("$canEdit"), SerializeField]
        string detailNormalMapName = "_DetailNormalMap";
        [FoldoutGroup("Settings"), EnableIf("$canEdit"), SerializeField]
        string metallicMapName = "_MetallicGlossMap";
        [FoldoutGroup("Settings"), EnableIf("$canEdit"), SerializeField]
        ShadowCastingMode shadowCastingMode = ShadowCastingMode.TwoSided;

        [FoldoutGroup("Dependencies"), Button(ButtonSizes.Medium), ButtonGroup("Dependencies/buttons")]
        void ClearDependencies() => DoClearDependencies();
        [FoldoutGroup("Dependencies"), Button(ButtonSizes.Medium), ButtonGroup("Dependencies/buttons")]
        void GetDependencies() => DoGetDependencies();

        [UsedImplicitly]
        const bool disableOutput = true;

        [HideInInspector, SerializeField]
        List<Object> externalDependencies = new();

        [FoldoutGroup("Dependencies"), PropertyOrder(2), SerializeField]
        List<Object> ignoredAssets = new();
        [FoldoutGroup("Dependencies"), PropertyOrder(2), SerializeField]
        List<Material> externalMaterials = new();
        [FoldoutGroup("Dependencies"), PropertyOrder(2), SerializeField]
        List<Texture2D> externalTextures = new();
        [FoldoutGroup("Dependencies"), PropertyOrder(2), SerializeField]
        List<GameObject> externalModels = new();
        [FoldoutGroup("Dependencies"), PropertyOrder(2), SerializeField]
        List<GameObject> externalPrefabs = new();
        [FoldoutGroup("Dependencies"), PropertyOrder(2), SerializeField]
        List<AnimatorController> externalAnimatorControllers = new();
        [FoldoutGroup("Dependencies"), PropertyOrder(2), SerializeField]
        List<Object> externalMonoScripts = new();
        [FoldoutGroup("Dependencies"), PropertyOrder(2), SerializeField]
        List<Object> unrecognizedTypes = new();

        [HideInInspector, SerializeField]
        bool canEdit = true;
        [UsedImplicitly]
        string editButtonLabel => canEdit ? "\u2714" : "\u270E";


        [FoldoutGroup("Output"), ButtonGroup("Output/buttons1"), Button(ButtonSizes.Medium), PropertyOrder(3)]
        void ClearOutput() => DoClearOutput();
        [FoldoutGroup("Output"), ButtonGroup("Output/buttons1"), Button(ButtonSizes.Medium), PropertyOrder(3)]
        void DuplicateDependencies() => DoDuplicateDependencies();
        [FoldoutGroup("Output"), ButtonGroup("Output/buttons2"), Button(ButtonSizes.Medium), PropertyOrder(3)]
        void FixOutputDependencies() => DoFixOutputDependencies();
        [FoldoutGroup("Output"), ButtonGroup("Output/buttons2"), Button(ButtonSizes.Medium), PropertyOrder(3)]
        void CheckDependencies() => DoCheckDependencies();

        [HideInInspector, SerializeField]
        List<Object> output = new();

        [FoldoutGroup("Output"), DisableIf("$disableOutput"), PropertyOrder(4), SerializeField]
        List<AssetDependencyVo> notFixedDependencies = new();
        [FoldoutGroup("Output"), DisableIf("$disableOutput"), PropertyOrder(4), SerializeField]
        List<Material> outputMaterials = new();
        [FoldoutGroup("Output"), DisableIf("$disableOutput"), PropertyOrder(4), SerializeField]
        List<Texture2D> outputTextures = new();
        [FoldoutGroup("Output"), DisableIf("$disableOutput"), PropertyOrder(4), SerializeField]
        List<GameObject> outputModels = new();
        [FoldoutGroup("Output"), DisableIf("$disableOutput"), PropertyOrder(4), SerializeField]
        List<GameObject> outputPrefabs = new();
        [FoldoutGroup("Output"), DisableIf("$disableOutput"), PropertyOrder(4), SerializeField]
        List<Mesh> outputMeshes = new();
        [FoldoutGroup("Output"), DisableIf("$disableOutput"), PropertyOrder(4), SerializeField]
        List<Avatar> outputAvatars = new();
        [FoldoutGroup("Output"), DisableIf("$disableOutput"), PropertyOrder(4), SerializeField]
        List<AnimatorController> outputAnimatorControllers = new();
        [FoldoutGroup("Output"), DisableIf("$disableOutput"), PropertyOrder(4), SerializeField]
        List<Object> outputMonoScripts = new();


        void DoGetDependencies ()
        {
        #if UNITY_EDITOR

            if (target == null)
            {
                Debug.Log("<color=orange><b>>>> Target field is null</b></color>");
                return;
            }

            DoClearDependencies();

            Selection.SetActiveObjectWithContext(target, this);
            EditorApplication.ExecuteMenuItem("Assets/Select Dependencies");

            foreach (var selected in Selection.objects)
            {
                if (ignoredAssets.Contains(selected)) continue;

                externalDependencies.Add(selected);
                AddToTypedList(selected);
            }

        #endif
        }

        void AddToTypedList (Object item)
        {
            if (item == null) return;

            var prefabType = PrefabUtility.GetPrefabAssetType(item);
            switch (prefabType)
            {
                case PrefabAssetType.Model:
                    externalModels.Add(item as GameObject);
                    return;
                case PrefabAssetType.Regular:
                    externalPrefabs.Add(item as GameObject);
                    return;
            }

            var itemType = item.GetType();
            if (itemType == externalMaterials.GetType().GetGenericArguments()[0])
            {
                externalMaterials.Add(item as Material);
                return;
            }
            if (itemType == externalTextures.GetType().GetGenericArguments()[0])
            {
                externalTextures.Add(item as Texture2D);
                return;
            }
            if (itemType == externalAnimatorControllers.GetType().GetGenericArguments()[0])
            {
                externalAnimatorControllers.Add(item as AnimatorController);
                return;
            }
            if (itemType == typeof(MonoScript))
            {
                externalMonoScripts.Add(item);
                return;
            }

            Debug.Log($"<color=yellow><b>>>> Type not found for: {item} of type {itemType}, {PrefabUtility.GetPrefabAssetType(item)}</b></color>");
            unrecognizedTypes.Add(item);
        }

        void DoDuplicateDependencies ()
        {
        #if UNITY_EDITOR

            if (externalDependencies.Count < 1)
            {
                Debug.Log("<color=orange><b>>>> Dependencies list is empty</b></color>");
                return;
            }

            DoClearOutput();

            var folderPath = $"{destinationFolder}/{newFolderName}";
            if (Directory.Exists(folderPath) == false)
            {
                Directory.CreateDirectory(folderPath);
            }

            externalMaterials.Select(dependency => DuplicateAsset(dependency, folderPath) as Material).ToList()
                .ForEach(item =>
                {
                    outputMaterials.Add(item);
                    output.Add(item);
                });
            externalTextures.Select(dependency => DuplicateAsset(dependency, folderPath) as Texture2D).ToList()
                .ForEach(item =>
                {
                    outputTextures.Add(item);
                    output.Add(item);
                });
            externalModels.Select(dependency => DuplicateAsset(dependency, folderPath) as GameObject).ToList()
                .ForEach(item =>
                {
                    var filters = item.transform.GetComponentsInChildren<MeshFilter>(true).ToList();
                    filters.ForEach(filter => outputMeshes.Add(filter.sharedMesh));
                    var skinned = item.transform.GetComponentsInChildren<SkinnedMeshRenderer>(true).ToList();
                    skinned.ForEach(skin => outputMeshes.Add(skin.sharedMesh));
                    var animators = item.transform.GetComponentsInChildren<Animator>(true).ToList();
                    animators.Where(animator => animator.avatar != null).ToList().ForEach(animator => outputAvatars.Add(animator.avatar));

                    outputModels.Add(item);
                    output.Add(item);
                });
            externalAnimatorControllers.Select(dependency => DuplicateAsset(dependency, folderPath) as AnimatorController).ToList()
                .ForEach(item =>
                {
                    outputAnimatorControllers.Add(item);
                    output.Add(item);
                });
            externalMonoScripts.ForEach(item =>
                {
                    var moved = MoveAsset(item, folderPath);
                    outputMonoScripts.Add(moved);
                    output.Add(moved);
                });
            externalPrefabs.Select(dependency => DuplicateAsset(dependency, folderPath) as GameObject).ToList()
                .ForEach(item =>
                {
                    outputPrefabs.Add(item);
                    output.Add(item);
                });

            AssetDatabase.Refresh();
        #endif
        }

        void DoCheckDependencies ()
        {
        #if UNITY_EDITOR

            if (output.Count < 1)
            {
                Debug.Log("<color=orange><b>>>> Output list is empty</b></color>");
                return;
            }

            var total = 0;
            notFixedDependencies.Clear();
            foreach (var outputObject in output)
            {
                Selection.SetActiveObjectWithContext(outputObject, this);
                EditorApplication.ExecuteMenuItem("Assets/Select Dependencies");

                foreach (var dependency in Selection.objects)
                {
                    if (ignoredAssets.Contains(dependency)) continue;

                    if (externalDependencies.Contains(dependency))
                    {
                        var assetDependencyVo = notFixedDependencies.FirstOrDefault(item => item.Asset == outputObject) ??
                                                new AssetDependencyVo(outputObject);

                        assetDependencyVo.Add(dependency);
                        notFixedDependencies.Add(assetDependencyVo);
                    }

                    total++;
                }
            }

            Debug.Log(notFixedDependencies.Count <= 0
                ? $"<color=green><b>>>> All dependencies resolved: {notFixedDependencies.Count}/{total}</b></color>"
                : $"<color=orange><b>>>> Dependencies Not resolved: {notFixedDependencies.Count}/{total}</b></color>");

        #endif
        }

        void DoFixOutputDependencies ()
        {
        #if UNITY_EDITOR

            FixMaterials();
            FixPrefabs();

            AssetDatabase.Refresh();

        #endif
        }

        void FixMaterials ()
        {
            foreach (var outputMaterial in outputMaterials)
            {
                var oldMaterial = externalMaterials.FirstOrDefault(mat => mat.name == outputMaterial.name);
                if (oldMaterial == null) continue;

                UpdateMap(oldMaterial, outputMaterial, albedoMapName);
                UpdateMap(oldMaterial, outputMaterial, emissionMapName);
                UpdateMap(oldMaterial, outputMaterial, normalMapName);
                UpdateMap(oldMaterial, outputMaterial, detailNormalMapName);
                UpdateMap(oldMaterial, outputMaterial, metallicMapName);
            }
        }

        void FixPrefabs ()
        {
            outputPrefabs
                .SelectMany(prefab => prefab.GetComponentsInChildren<Renderer>(true)).ToList()
                .ForEach(renderer =>
                {
                    renderer.sharedMaterials = renderer.sharedMaterials.Select(sharedMaterial => GetNewAssetOfName<Material>(sharedMaterial.name)).ToArray();
                    renderer.shadowCastingMode = shadowCastingMode;
                });

            outputPrefabs.SelectMany(prefab => prefab.GetComponentsInChildren<SkinnedMeshRenderer>(true)).ToList()
                .ForEach(renderer =>
                {
                    renderer.sharedMesh = outputMeshes.FirstOrDefault(mesh => mesh.name == renderer.sharedMesh.name);
                });

            outputPrefabs.SelectMany(prefab => prefab.GetComponentsInChildren<MeshFilter>(true)).ToList()
                .ForEach(filter =>
                {
                    filter.sharedMesh = outputMeshes.FirstOrDefault(mesh => mesh.name == filter.sharedMesh.name);
                });

            outputPrefabs.SelectMany(prefab => prefab.GetComponentsInChildren<Animator>(true)).ToList()
                .ForEach(animator =>
                {
                    animator.avatar = outputAvatars.FirstOrDefault(avatar => avatar.name == animator.avatar.name);
                    animator.runtimeAnimatorController = outputAnimatorControllers.FirstOrDefault(controller => controller.name == animator.runtimeAnimatorController.name);
                });

            outputPrefabs.SelectMany(prefab => prefab.GetComponentsInChildren<Component>(true))
                .Where(component => component.GetType() == typeof(MonoScript)).ToList()
                .ForEach(oldComponent =>
                {
                    var newComponent = outputMonoScripts.FirstOrDefault(component => component.name == oldComponent.name);
                    if (newComponent == null) return;

                    oldComponent.gameObject.AddComponent(newComponent.GetType());
                    DestroyImmediate(oldComponent);
                });
        }

        #region Workers
        void ToggleEdit ()
            => canEdit = !canEdit;
        void SetFolderNameToTargetName ()
            => newFolderName = target == null ? "" : target.name;

        T GetNewAssetOfName<T> (string queryName) where T : Object
        {
            var folderPath      = $"{destinationFolder}/{newFolderName}";
            var type            = typeof(T).ToString().Replace("UnityEngine.", "");
            var guid            = AssetDatabase.FindAssets($"{queryName} t:{type}", new[] {folderPath})[0];
            var assetPath       = AssetDatabase.GUIDToAssetPath(guid);
            var result          = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            return result;
        }

        void DoClearDependencies ()
        {
            externalDependencies.Clear();
            externalModels.Clear();
            externalPrefabs.Clear();
            externalMaterials.Clear();
            externalTextures.Clear();
            externalAnimatorControllers.Clear();
            externalMonoScripts.Clear();
            unrecognizedTypes.Clear();
        }

        void DoClearOutput ()
        {
            output.Clear();
            outputMaterials.Clear();
            outputTextures.Clear();
            outputModels.Clear();
            outputPrefabs.Clear();
            outputMeshes.Clear();
            outputAvatars.Clear();
            outputMonoScripts.Clear();
            outputAnimatorControllers.Clear();
            notFixedDependencies.Clear();
        }

        Object MoveAsset (Object asset, string folderPath)
        {
            Object result = null;

        #if UNITY_EDITOR
            var oldPath     = AssetDatabase.GetAssetPath(asset);
            var extension   = Path.GetExtension(oldPath);
            var newPath     = $"{folderPath}/{asset.name}{extension}";
            var success     = AssetDatabase.MoveAsset(oldPath, newPath);

            if (string.IsNullOrWhiteSpace(success) == false)
            {
                result = AssetDatabase.LoadAssetAtPath<Object>(newPath);
            }
        #endif

            return result;
        }

        Object DuplicateAsset (Object asset, string folderPath)
        {
            Object result = null;

        #if UNITY_EDITOR
            var oldPath     = AssetDatabase.GetAssetPath(asset);
            var extension   = Path.GetExtension(oldPath);
            var newPath     = $"{folderPath}/{asset.name}{extension}";
            var success     = AssetDatabase.CopyAsset(oldPath, newPath);

            if (success)
            {
                result = AssetDatabase.LoadAssetAtPath<Object>(newPath);
            }

            // var logColor = success ? "green" : "orange";
            // Debug.Log($"<color={logColor}><b>>>> {asset}, {oldPath}, {newPath}, {extension}</b></color>");
        #endif

            return result;
        }

        void UpdateMap (Material oldMaterial, Material newMaterial, string mapName)
        {
            var oldMap = oldMaterial.GetTexture(mapName);
            if (oldMap == null) return;

            var newMap = GetNewAssetOfName<Texture2D>(oldMap.name);
            if (newMap == null) return;

            newMaterial.SetTexture(mapName, newMap);
        }
        #endregion Workers
    }
}