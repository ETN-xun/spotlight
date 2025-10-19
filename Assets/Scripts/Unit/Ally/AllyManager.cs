using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Ally
{
    /// <summary>
    /// 管理所有友方单位
    /// </summary>
    public class AllyManager : MonoBehaviour
    {
        public static AllyManager Instance { get; private set; }
        
        public int CurrentPlayerTurn { get; private set; }
        
        private readonly List<Unit> _allies = new List<Unit>();
        private readonly List<Unit> _aliveAllies = new List<Unit>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            MessageCenter.Subscribe(Defines.PlayerTurnStartEvent, OnPlayerTurnStart);
            MessageCenter.Subscribe(Defines.PlayerTurnEndEvent, OnPlayerTurnEnd);
            MessageCenter.Subscribe(Defines.AllyUnitDiedEvent, OnAllyUnitDied);
        }

        private void OnDisable()
        {
            MessageCenter.Unsubscribe(Defines.PlayerTurnStartEvent, OnPlayerTurnStart);
            MessageCenter.Unsubscribe(Defines.PlayerTurnEndEvent, OnPlayerTurnEnd);
            MessageCenter.Unsubscribe(Defines.AllyUnitDiedEvent, OnAllyUnitDied);
        }

        public List<Unit> GetAliveAllies()
        {
            return _aliveAllies;
        }

        public void AddAliveAlly(Unit ally)
        {
            _aliveAllies.Add(ally);
            _allies.Add(ally);
        }

        private void OnPlayerTurnStart(object[] args)
        {
            CurrentPlayerTurn++;
        }

        private void OnPlayerTurnEnd(object[] args)
        {
            
        }
        
        private void OnAllyUnitDied(object[] args)
        {
            if (args[0] is not Unit unit) return;
            if (_aliveAllies.Contains(unit))
            {
                _aliveAllies.Remove(unit);
            }

            if (_aliveAllies.Count == 0)
            {
                GameManager.Instance.ChangeGameState(GameState.GameOver);
            }
        }
    }
}