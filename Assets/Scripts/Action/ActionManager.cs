using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Action
{
    public class ActionManager : MonoBehaviour
    {
        private readonly Queue<IAction> _actionQueue = new Queue<IAction>();
        private bool _isExecuting;

        public static ActionManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void EnqueueAction(IAction action)
        {
            _actionQueue.Enqueue(action);
            TryExecuteNext();
        }

        private void TryExecuteNext()
        {
            if (_isExecuting || _actionQueue.Count == 0)
                return;

            StartCoroutine(ExecuteNext());
        }

        private IEnumerator ExecuteNext()
        {
            _isExecuting = true;

            while (_actionQueue.Count > 0)
            {
                var action = _actionQueue.Dequeue();
                yield return action.Execute(new ActionContext(null, null, this));
            }

            _isExecuting = false;
            OnAllActionsCompleted();
        }

        private void OnAllActionsCompleted()
        {
            Debug.Log("所有动作执行完毕");
        }

        public void ClearActions()
        {
            _actionQueue.Clear();
        }
    }
}