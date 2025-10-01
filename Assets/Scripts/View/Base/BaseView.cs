using System;
using System.Collections.Generic;
using UnityEngine;

namespace View.Base
{
    public abstract class BaseView : MonoBehaviour, IBaseView
    {
        public int ViewId { get; set; }
        public bool IsInit { get; set; }
        
        private Canvas _canvas;
        
        private readonly Dictionary<string, GameObject> _cacheGameObjects = new ();

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            OnAwake();
        }

        private void Start()
        {
            OnStart();
        }
        
        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }

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
            _canvas.enabled = visible;
        }

        public GameObject Find(string goName)
        {
            if (_cacheGameObjects.TryGetValue(goName, out var go))
            {
                return go;
            }
            _cacheGameObjects.Add(goName, transform.Find(goName).gameObject);
            return _cacheGameObjects[goName];
        }
        
        public T Find<T>(string goName) where T : Component
        {
            return Find(goName).GetComponent<T>();
        }
    }
}