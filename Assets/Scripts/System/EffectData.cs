using UnityEngine;

namespace System
{
    [CreateAssetMenu(menuName = "Game/EffectData", fileName = "NewEffectData")]
    public class EffectData : ScriptableObject
    {
        [Tooltip("Unique key to identify this effect")]
        public string key;
        public GameObject prefab;
        [Tooltip("If 0, the system will try to infer duration from ParticleSystems on the prefab")]
        public float duration;
        [Tooltip("How many instances to preload into the pool")]
        public int preloadCount = 2;
    }
}
