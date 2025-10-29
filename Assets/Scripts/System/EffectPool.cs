using System.Collections.Generic;
using UnityEngine;

namespace System
{
    public class EffectPool
    {
        private readonly GameObject _prefab;
        private readonly Queue<GameObject> _pool = new Queue<GameObject>();
        private readonly Transform _root;

        public EffectPool(GameObject prefab, Transform root)
        {
            _prefab = prefab;
            _root = root ?? new GameObject($"EffectPool_{prefab.name}").transform;
            // keep pools organized in scene: parent will be set by caller
        }

        public GameObject Get()
        {
            GameObject inst;
            if (_pool.Count > 0)
            {
                inst = _pool.Dequeue();
            }
            else
            {
                inst = UnityEngine.Object.Instantiate(_prefab, _root);
                var ei = inst.GetComponent<EffectInstance>();
                if (ei == null) ei = inst.AddComponent<EffectInstance>();
                ei.Initialize(this);
            }
            inst.transform.SetParent(_root, false);
            inst.SetActive(true);
            return inst;
        }

        public void Return(GameObject obj)
        {
            if (obj == null) return;
            obj.transform.SetParent(_root, false);
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }

        public void Preload(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var go = UnityEngine.Object.Instantiate(_prefab, _root);
                var ei = go.GetComponent<EffectInstance>();
                if (ei == null) ei = go.AddComponent<EffectInstance>();
                ei.Initialize(this);
                go.SetActive(false);
                _pool.Enqueue(go);
            }
        }
    }
}
