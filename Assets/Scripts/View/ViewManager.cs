using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using View.Base;

namespace View
{
    public sealed class ViewManager : MonoBehaviour
    {
        public static ViewManager Instance { get; private set; }
        
        private readonly Dictionary<int, ViewInfo> _views = new (); 
        // 支持多实例，key: viewType_instanceKey
        private readonly Dictionary<string, IBaseView> _openViews = new ();
        private readonly Dictionary<string, IBaseView> _cacheViews = new ();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
            
        }

        private void Start()
        {

        }

        private string GetViewKey(ViewType viewType, int instanceKey = 0)
        {
            return $"{(int)viewType}_{instanceKey}";
        }

        /// <summary>
        /// 注册 View
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="viewInfo"></param>
        public void RegisterView(ViewType viewType, ViewInfo viewInfo)
        {
            RegisterView((int)viewType, viewInfo);
        }

        /// <summary>
        /// 注释 View
        /// </summary>
        /// <param name="viewType"></param>
        public void UnregisterView(ViewType viewType)
        {
            _views.Remove((int)viewType);
        }

        /// <summary>
        /// 移除 View
        /// </summary>
        /// <param name="viewType"></param>
        public void RemoveView(ViewType viewType, int instanceKey = 0)
        {
            var key = GetViewKey(viewType, instanceKey);
            _views.Remove((int) viewType);
            _openViews.Remove(key);
            _cacheViews.Remove(key);
        }

        /// <summary>
        /// 显示 View
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="instanceKey"></param>
        /// <param name="args"></param>
        public void OpenView(ViewType viewType, int instanceKey = 0, params object[] args)
        {
            var key = GetViewKey(viewType, instanceKey);
            var viewInfo = _views[(int)viewType];
            var view = GetView(viewType, instanceKey);
            
            if (view is null)
            {
                var viewGo = Instantiate(Resources.Load<GameObject>($"Prefab/View/{viewInfo.PrefabName}"), viewInfo.ParentTransform);
                if (viewGo is null)
                {
                    Debug.LogError($"Failed to load view prefab: {viewInfo.PrefabName}");
                    return;
                }
                var canvas = viewGo.GetComponent<Canvas>() ?? viewGo.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingOrder = viewInfo.SortingOrder;
                view = viewGo.GetComponent<IBaseView>();
                if (view is null)
                {
                    Debug.LogError($"The prefab {viewInfo.PrefabName} does not have a component that implements IBaseView.");
                    Destroy(viewGo);
                    return;
                }
                view.ViewId = (int)viewType;
                _cacheViews.Add(key, view);
            }

            if (!_openViews.TryAdd(key, view))
            {
                return;
            }

            if (view.IsInit)
            {
                view.SetVisible(true);
            }
            else
            {
                view.Init();
            }

            view.Open(args);
        }

        /// <summary>
        /// 关闭 View
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="instanceKey"></param>
        /// <param name="args"></param>
        public void CloseView(ViewType viewType, int instanceKey = 0, params object[] args)
        {
            var key = GetViewKey(viewType, instanceKey);
            if (!IsOpen(viewType, instanceKey)) return;
            
            var view = GetView(viewType, instanceKey);
            if (view is null)
            {
                Debug.LogWarning($"View of type {viewType} (instanceKey={instanceKey}) is not open.");
                return;
            }

            view.Close(args);
            _openViews.Remove(key);
        }
        
        /// <summary>
        /// 关闭所有的 View
        /// </summary>
        public void CloseAllViews()
        {
            foreach (var view in _openViews.Values)
            {
                view.Close();
            }
            _openViews.Clear();
        }

        /// <summary>
        /// 销毁 View
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="instanceKey"></param>
        public void DestroyView(ViewType viewType, int instanceKey = 0)
        {
            var key = GetViewKey(viewType, instanceKey);
            var view = GetView(viewType, instanceKey);
            if (view is null)
            {
                Debug.LogWarning($"View of type {viewType} (instanceKey={instanceKey}) does not exist.");
                return;
            }

            view.Destroy();
            _cacheViews.Remove(key);
        }

        /// <summary>
        /// View 是否打开
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        private bool IsOpen(ViewType viewType, int instanceKey = 0)
        {
            var key = GetViewKey(viewType, instanceKey);
            return _openViews.ContainsKey(key);
        }

        /// <summary>
        /// 得到 View
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public IBaseView GetView(ViewType viewType, int instanceKey = 0)
        {
            var key = GetViewKey(viewType, instanceKey);
            if (_openViews.TryGetValue(key, out var view) || _cacheViews.TryGetValue(key, out view))
            {
                return view;
            }

            return null;
        }
        
        public T GetView<T>(ViewType viewType, int instanceKey = 0) where T : class, IBaseView
        {
            var view = GetView(viewType, instanceKey);
            return view as T;
        }

        private void RegisterView(int viewType, ViewInfo viewInfo)
        {
            _views.TryAdd(viewType, viewInfo);
        }
    }
}