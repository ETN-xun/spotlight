using System.Collections.Generic;
using UnityEngine;

namespace System
{
    public class EffectSystem : MonoBehaviour
    {
        public static EffectSystem Instance { get; private set; }

        [Tooltip("可在 Inspector 填入 EffectData ScriptableObjects")]
        public List<EffectData> effectConfigs = new List<EffectData>();

        private readonly Dictionary<string, EffectData> _configMap = new Dictionary<string, EffectData>();
        private readonly Dictionary<string, EffectPool> _pools = new Dictionary<string, EffectPool>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            // 确保设置为根对象再标记为常驻，避免因父对象卸载导致失效
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
            RegisterConfigs(effectConfigs);
        }

        public void RegisterConfigs(IEnumerable<EffectData> configs)
        {
            if (configs == null) return;
            foreach (var cfg in configs)
            {
                if (cfg == null || string.IsNullOrEmpty(cfg.key) || cfg.prefab == null) continue;
                _configMap[cfg.key] = cfg;
                if (!_pools.ContainsKey(cfg.key))
                {
                    var root = new GameObject($"EffectRoot_{cfg.key}").transform;
                    root.SetParent(transform, false);
                    var pool = new EffectPool(cfg.prefab, root);
                    _pools[cfg.key] = pool;
                    if (cfg.preloadCount > 0) pool.Preload(cfg.preloadCount);
                }
            }
        }

        public GameObject Play(string key, Vector3 position, Quaternion rotation = default, Transform parent = null)
        {
            if (!_configMap.TryGetValue(key, out var cfg)) return null;
            if (!_pools.TryGetValue(key, out var pool))
            {
                var root = new GameObject($"EffectRoot_{key}").transform;
                root.SetParent(transform, false);
                pool = new EffectPool(cfg.prefab, root);
                _pools[key] = pool;
            }

            var inst = pool.Get();
            if (parent != null) inst.transform.SetParent(parent, false);
            else inst.transform.SetParent(null);
            inst.transform.position = position;
            inst.transform.rotation = rotation == default ? Quaternion.identity : rotation;

            float duration = cfg.duration;
            var ei = inst.GetComponent<EffectInstance>();
            if (ei != null) ei.Play(duration);
            return inst;
        }

        public void Stop(GameObject instance)
        {
            if (instance == null) return;
            var ei = instance.GetComponent<EffectInstance>();
            if (ei != null) ei.ReturnToPool();
            else instance.SetActive(false);
        }

        public void Preload(string key, int count)
        {
            if (!_configMap.TryGetValue(key, out var cfg)) return;
            if (!_pools.TryGetValue(key, out var pool))
            {
                var root = new GameObject($"EffectRoot_{key}").transform;
                root.SetParent(transform, false);
                pool = new EffectPool(cfg.prefab, root);
                _pools[key] = pool;
            }
            pool.Preload(count);
        }
    }
}
