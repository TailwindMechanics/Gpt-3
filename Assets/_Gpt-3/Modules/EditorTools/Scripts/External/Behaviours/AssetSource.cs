using Modules.EditorTools.External.DataObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.EditorTools.External.Behaviours
{
    public class AssetSource : Lockable
    {
        [Button(ButtonSizes.Small), PropertyOrder(-1), LabelText("$LargeButtonLabel")]
        void ToggleLocked() => ToggleEdit();
        [EnableIf("$CanEdit"), SerializeField]
        AssetPackSo AssetPack;
        [EnableIf("$CanEdit"), SerializeField]
        string OriginalName;
    }
}