using JetBrains.Annotations;
using UnityEngine;


namespace Tailwind.EditorTools.External.DataObjects
{
	public abstract class Lockable : MonoBehaviour
	{
		// InlineButton(nameof(ToggleEdit), "$SmallButtonLabel")

		public void ToggleEdit () => CanEdit = !CanEdit;
		[HideInInspector, SerializeField]
		public bool CanEdit = true;
		[UsedImplicitly]
		public string SmallButtonLabel => CanEdit ? "\u2714" : "\u270E";
		[UsedImplicitly]
		public string LargeButtonLabel => CanEdit ? "Lock" : "Unlock";
	}
}