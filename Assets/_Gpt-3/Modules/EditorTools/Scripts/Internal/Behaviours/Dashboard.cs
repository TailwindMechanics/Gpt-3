using Sirenix.OdinInspector;
using UnityEngine;

namespace Modules.EditorTools.Internal.Behaviours
{
    public class Dashboard : MonoBehaviour
    {
        [InlineEditor, SerializeField]
        ScriptableObject tool;
    }
}