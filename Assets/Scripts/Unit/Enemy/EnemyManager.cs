using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Enemy.AI;
using UnityEngine;

namespace Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance { get; private set; }
        public int CurrentEnemyTurn { get; private set; }

        private readonly EnemyIntentPlanner _intentPlanner  = new();
        private readonly EnemyIntentExecutor _intentExecutor = new();
        
        private readonly List<Unit> _enemies = new ();
        private readonly List<Unit> _aliveEnemies = new ();

        public bool EnemyIntentsShowFinished { get; private set; }
        public bool EnemyIntentsExecuteFinished { get; private set; }
        
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
            MessageCenter.Subscribe(Defines.DeploymentStateEndedEvent, OnDeploymentStateEnded);
        }

        private void OnDisable()
        {
            MessageCenter.Unsubscribe(Defines.EnemyUnitDiedEvent, OnEnemyUnitDied);
            MessageCenter.Unsubscribe(Defines.DeploymentStateEndedEvent, OnDeploymentStateEnded);
        }
        
        public void StartEnemyTurnFlow()
        {
            StartCoroutine(EnemyTurnFlow());
        }

        private IEnumerator EnemyTurnFlow( )
        {
            CurrentEnemyTurn++;
            EnemyIntentsExecuteFinished = false;
            EnemyIntentsShowFinished = false;
            Debug.Log("敌人回合第 " + CurrentEnemyTurn + " 轮开始");

            if (CurrentEnemyTurn != 1)
                yield return ExecuteEnemyIntents();

            yield return ShowEnemyIntents();
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
        
        private IEnumerator ShowEnemyIntents()
        {
            var enemyIntents = _intentPlanner.GetOrderedEnemyIntents();
            foreach (var enemyIntent in enemyIntents.Where(intent => _aliveEnemies.Contains(intent.Key)))
            {
                // 仅供测试使用，实际应显示所有意图
                yield return StartCoroutine(_intentExecutor.ShowIntent(enemyIntent.Key, enemyIntent.Value[0]));
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(1f);
            EnemyIntentsShowFinished = true;
        }

        private IEnumerator ExecuteEnemyIntents()
        {
            var enemyIntents = _intentPlanner.GetOrderedEnemyIntents();
            foreach (var enemyIntent in enemyIntents.Where(intent => _aliveEnemies.Contains(intent.Key)))
            {
                yield return _intentExecutor.ExecuteIntent(enemyIntent.Key, enemyIntent.Value);
            }

            yield return new WaitForSeconds(2f);
            foreach (var enemy in _aliveEnemies)
                _intentPlanner.BuildIntent(enemy);
            EnemyIntentsExecuteFinished = true;
        }
        
        private void OnEnemyUnitDied(object[] args)
        {
            if (args[0] is not Unit unit) return;
            if (_aliveEnemies.Contains(unit))
            {
                _aliveEnemies.Remove(unit);
            }
            
            if (_aliveEnemies.Count == 0)
            {
                Debug.Log("所有敌人已被消灭，玩家获胜！");
                GameManager.Instance.ChangeGameState(GameState.GameOver);
            }
        }

        private void OnDeploymentStateEnded(object[] args)
        {
            foreach (var enemy in _aliveEnemies)
                _intentPlanner.BuildIntent(enemy);
        }
    }
}