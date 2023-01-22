using Sirenix.OdinInspector;
using UnityEngine;


namespace Tailwind.EditorTools.Internal.Behaviours
{
    public class Dashboard : MonoBehaviour
    {
        [InlineEditor, SerializeField]
        ScriptableObject tool;
    }
}