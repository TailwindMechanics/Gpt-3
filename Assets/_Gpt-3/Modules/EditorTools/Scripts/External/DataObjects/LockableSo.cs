using JetBrains.Annotations;
using UnityEngine;

namespace Modules.EditorTools.External.DataObjects
{
	public abstract class LockableSo : ScriptableObject
	{
		// InlineButton(nameof(ToggleEdit), "$SmallButtonLabel")

		public void ToggleEdit () => CanEdit = !CanEdit;
		[HideInInspector, SerializeField]
		public bool CanEdit = true;
		[UsedImplicitly]
		public string SmallButtonLabel => CanEdit ? "\u2714" : "\u270E";
		[UsedImplicitly]
		public string LargeButtonLabel => CanEdit ? "\u2714 Lock" : "\u270E Unlock";
	}
}