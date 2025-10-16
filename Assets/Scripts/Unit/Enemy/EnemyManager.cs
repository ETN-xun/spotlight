using System;
using System.Collections.Generic;
using Common;
using Enemy.AI;
using UnityEngine;

namespace Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        // EnemyManager 来管理所有敌方单位的生成和行为，还有敌方AI的执行
        public static EnemyManager Instance { get; private set; }
        
        public int CurrentEnemyTurn { get; private set; }

        private readonly EnemyIntentPlanner _intentPlanner = new();
        
        private readonly List<Unit> _enemies = new ();
        private readonly List<Unit> _aliveEnemies = new ();
        
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
            MessageCenter.Subscribe(Defines.EnemyUnitDiedEvent, OnEnemyUnitDied);
        }

        private void OnDisable()
        {
            MessageCenter.Unsubscribe(Defines.EnemyUnitDiedEvent, OnEnemyUnitDied);
        }
        
        public void StartEnemyTurn()       // 改成不用事件触发
        {
            CurrentEnemyTurn++;
            foreach (var enemy in _aliveEnemies)
            {
                _intentPlanner.BuildIntent(enemy);
            }
        }
        
        public void EndEnemyTurn()
        {

        }
        
        private void OnEnemyUnitDied(object[] args)
        {
            if (args[0] is not Unit unit) return;
            if (_aliveEnemies.Contains(unit))
            {
                _aliveEnemies.Remove(unit);
            }
        }

        public void SpawnEnemies()
        {
            var currentLevelData = Level.LevelManager.Instance.GetCurrentLevel();
            var aliveEnemies = currentLevelData.enemyUnits;
            GridManager.Instance.PlaceUnit(new Vector2Int(3, 3), aliveEnemies[0]);
            GridManager.Instance.PlaceUnit(new Vector2Int(-3, 3), aliveEnemies[1]);
            _enemies.Add(aliveEnemies[0]);
            _enemies.Add(aliveEnemies[1]);
        }
        
        public void AddAliveEnemy(Unit ally)
        {
            _aliveEnemies.Add(ally);
            _enemies.Add(ally);
        }
        
        public List<Unit> GetAliveEnemies()
        {
            return _aliveEnemies;
        }
        
        public void ShowEnemyIntents()
        {
            // 展示意图，得先规划意图
            foreach (var enemy in _enemies)
            {
                
            }
        }
    }
}