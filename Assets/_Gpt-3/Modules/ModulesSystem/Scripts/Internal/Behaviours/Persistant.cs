using UnityEngine;

namespace Modules.ModulesSystem.Internal.Behaviours
{
    public class Persistant : MonoBehaviour
    {
        void Awake() => DontDestroyOnLoad(gameObject);
    }
}