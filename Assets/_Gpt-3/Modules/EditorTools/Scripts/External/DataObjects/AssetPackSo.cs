using Sirenix.OdinInspector;
using UnityEngine;


namespace Tailwind.EditorTools.External.DataObjects
{
	[CreateAssetMenu(fileName = "new _assetPack", menuName = "Tailwind/Assets/Pack")]
	public class AssetPackSo : LockableSo
	{
		[ValueDropdown(nameof(Authors)), InlineButton(nameof(ToggleEdit), "$SmallButtonLabel"), SerializeField]
		string Author;
		[EnableIf("$CanEdit"), SerializeField]
		string PackName;

		string[] Authors => AssetAuthors.Authors;
	}
}