using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using View.Base;

namespace View
{
    public sealed class ViewManager : MonoBehaviour
    {
        // 1. 初始化各种 View
        // 2. 供外界拿到 View 的接口
        public static ViewManager Instance { get; private set; }
        
        /// <summary>
        /// 所有的 View 信息
        /// </summary>
        private readonly Dictionary<int, ViewInfo> _views = new (); 
        /// <summary>
        /// 所有在场景中显示的 View
        /// </summary>
        private readonly Dictionary<int, IBaseView> _openViews = new ();
        /// <summary>
        /// 所有已加载的 View (包含打开和关闭的)
        /// </summary>
        private readonly Dictionary<int, IBaseView> _cacheViews = new ();
        

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            
            RegisterView(ViewType.TestView, new ViewInfo()
            {
                PrefabName = "TestView",
                SortingOrder = 0,
                ParentTransform = transform
            });

            RegisterView(ViewType.UnitView, new ViewInfo()
            {
                PrefabName = "UnitView",
                SortingOrder = 1,
                ParentTransform = transform
            });
        }

        private void Start()
        {
            
        }

        /// <summary>
        /// 注册 View
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="viewInfo"></param>
        private void RegisterView(ViewType viewType, ViewInfo viewInfo)
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
        public void RemoveView(ViewType viewType)
        {
            _views.Remove((int)viewType);
            _openViews.Remove((int)viewType);
            _cacheViews.Remove((int)viewType);
        }

        /// <summary>
        /// 显示 View
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="args"></param>
        public void OpenView(ViewType viewType, params object[] args)
        {
            var view = GetView(viewType);
            var viewInfo = _views[(int)viewType];
            
            if (view is null)
            {
                var viewGo = Instantiate(Resources.Load<GameObject>($"View/{viewInfo.PrefabName}"), viewInfo.ParentTransform);
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
                _cacheViews.Add((int)viewType, view);
                
            }

            if (!_openViews.TryAdd((int)viewType, view))
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
        /// <param name="args"></param>
        public void CloseView(ViewType viewType, params object[] args)
        {
            if (!IsOpen(viewType)) return;
            
            var view = GetView(viewType);
            if (view is null)
            {
                Debug.LogWarning($"View of type {viewType} is not open.");
                return;
            }

            view.Close(args);
            
            _openViews.Remove((int)viewType);
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
        public void DestroyView(ViewType viewType)
        {
            var view = GetView(viewType);
            if (view is null)
            {
                Debug.LogWarning($"View of type {viewType} does not exist.");
                return;
            }

            view.Destroy();
            _cacheViews.Remove((int)viewType);
            UnregisterView(viewType);
        }

        /// <summary>
        /// View 是否打开
        /// </summary>
        /// <param name="viewType"></param>
        /// <returns></returns>
        private bool IsOpen(ViewType viewType)
        {
            return _openViews.ContainsKey((int)viewType);
        }

        /// <summary>
        /// 得到 View
        /// </summary>
        /// <param name="viewType"></param>
        /// <returns></returns>
        private IBaseView GetView(ViewType viewType)
        {
            if (_openViews.TryGetValue((int)viewType, out var view) || _cacheViews.TryGetValue((int)viewType, out view))
            {
                return view;
            }

            return null;
        }

        private void RegisterView(int viewType, ViewInfo viewInfo)
        {
            _views.TryAdd(viewType, viewInfo);
        }
    }
}