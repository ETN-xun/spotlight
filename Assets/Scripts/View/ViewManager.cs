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

        private string GetViewKey(ViewType viewType, string instanceKey = "")
        {
            return $"{viewType}_{instanceKey}";
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
        public void RemoveView(ViewType viewType, string instanceKey = "")
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
        public void OpenView(ViewType viewType, string instanceKey = "", params object[] args)
        {
            var key = GetViewKey(viewType, instanceKey);
            if (!_views.TryGetValue((int)viewType, out var viewInfo) || viewInfo == null)
            {
                Debug.LogError($"ViewManager.OpenView: ViewType {viewType} 未注册，无法打开。请先在 GameAppManager.RegisterViews 中注册。");
                return;
            }

            var view = GetView(viewType, instanceKey);
            
            if (view is null)
            {
                var prefabPath = $"Prefab/View/{viewInfo.PrefabName}";
                var prefab = Resources.Load<GameObject>(prefabPath);
                if (prefab is null)
                {
                    Debug.LogError($"Failed to load view prefab at Resources/{prefabPath}");
                    return;
                }
                var parent = viewInfo.ParentTransform != null ? viewInfo.ParentTransform : transform;
                var viewGo = Instantiate(prefab, parent);
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
        public void CloseView(ViewType viewType, string instanceKey = "", params object[] args)
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
        public void DestroyView(ViewType viewType, string instanceKey = "")
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
        private bool IsOpen(ViewType viewType, string instanceKey = "")
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
        public IBaseView GetView(ViewType viewType, string instanceKey = "")
        {
            var key = GetViewKey(viewType, instanceKey);
            if (_openViews.TryGetValue(key, out var view) || _cacheViews.TryGetValue(key, out view))
            {
                return view;
            }

            return null;
        }
        
        public T GetView<T>(ViewType viewType, string instanceKey = "") where T : class, IBaseView
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