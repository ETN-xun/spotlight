using System.Collections.Generic;
using UnityEngine;

namespace View.Base
{
    public abstract class BaseView : MonoBehaviour, IBaseView
    {
        public int ViewId { get; set; }
        public bool IsInit { get; set; }
        
        
        private readonly Dictionary<string, GameObject> _cacheGameObjects = new ();

        private void Awake()
        {
            OnAwake();
        }

        private void Start()
        {
            OnStart();
        }

        private void Update()
        {
            OnUpdate();
        }

        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }
        protected virtual void OnUpdate() { }

        public void Init()
        {
            if (IsInit) return;
            InitView();
            InitData();
            IsInit = true;
        }

        protected virtual void InitView()
        {
            
        }

        protected virtual void InitData()
        {
            
        }

        public virtual void Open(params object[] args)
        {
            
        }

        public virtual void Close(params object[] args)
        {
            SetVisible(false);
        }

        public virtual void Destroy()
        {
            Destroy(gameObject);
        }

        public virtual void SetVisible(bool visible)
        {
            try
            {
                if (this == null) return;
                var go = gameObject;
                if (go == null) return;
                if (go.activeSelf != visible)
                    go.SetActive(visible);
            }
            catch (MissingReferenceException)
            {
            }
        }


        private GameObject Find(string goName)
        {
            if (_cacheGameObjects.TryGetValue(goName, out var go))
                return go;

            var tf = transform.Find(goName);
            if (tf == null) return null;

            _cacheGameObjects[goName] = tf.gameObject;
            return tf.gameObject;
        }

        public T Find<T>(string goName) where T : Component
        {
            var go = Find(goName);
            if (go == null) return null;
            return go.GetComponent<T>();
        }
    }
}