using System;
using System.Collections.Generic;

namespace Common
{
    public static class MessageCenter
    {
        // 存储所有事件名及其委托
        private static readonly Dictionary<string, Delegate> EventTable = new();

        /// <summary>
        /// 注册事件监听
        /// </summary>
        public static void Subscribe(string eventName, Action<object[]> handler)
        {
            if (string.IsNullOrEmpty(eventName) || handler == null) return;
            if (EventTable.TryGetValue(eventName, out var del))
            {
                EventTable[eventName] = Delegate.Combine(del, handler);
            }
            else
            {
                EventTable[eventName] = handler;
            }
        }

        /// <summary>
        /// 注销事件监听
        /// </summary>
        public static void Unsubscribe(string eventName, Action<object[]> handler)
        {
            if (string.IsNullOrEmpty(eventName) || handler == null) return;
            if (!EventTable.TryGetValue(eventName, out var del)) return;
            var currentDel = Delegate.Remove(del, handler);
            if (currentDel == null)
                EventTable.Remove(eventName);
            else
                EventTable[eventName] = currentDel;
        }

        /// <summary>
        /// 触发事件（参数可选，可为null或空）
        /// </summary>
        public static void Publish(string eventName, params object[] args)
        {
            if (string.IsNullOrEmpty(eventName)) return;
            if (!EventTable.TryGetValue(eventName, out var del)) return;
            if (del is Action<object[]> callback)
            {
                callback.Invoke(args);
            }
        }

        /// <summary>
        /// 清空所有事件
        /// </summary>
        public static void Clear()
        {
            EventTable.Clear();
        }
    }
}