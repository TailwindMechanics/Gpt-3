using UnityEngine;


namespace Tailwind.ModulesSystem.Internal.Behaviours
{
    public class Persistant : MonoBehaviour
    {
        void Awake() => DontDestroyOnLoad(gameObject);
    }
}