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
            // var currentLevelData = Level.LevelManager.Instance.GetCurrentLevel();
            // var aliveEnemies = currentLevelData.enemyUnits;
            // for (var i = 0; i < aliveEnemies.Count; i++)
            // {
            //     GridManager.Instance.PlaceUnit(new Vector2Int(i, 3), aliveEnemies[i]);
            // }
        }
        
        public Unit GetAliveEnemyByID(string unitID)
        {
            return _aliveEnemies.Find(enemy => enemy.data.unitID == unitID);
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
        
        public void RemoveNullPointerAttackedUnits()
        {
            foreach (var enemy in _enemies.Where(enemy => enemy.data.unitType == UnitType.NullPointer))
            {
                var toRemove = new List<Unit>();
                foreach (var kvp in enemy.attackedUnits.ToList()) // 遍历副本，避免修改时出错
                {
                    enemy.attackedUnits[kvp.Key] = kvp.Value - 1;
                    if (enemy.attackedUnits[kvp.Key] <= 0)
                        toRemove.Add(kvp.Key);
                }
                foreach (var unit in toRemove)
                {
                    enemy.attackedUnits.Remove(unit);
                }
            }
        }
        
        private IEnumerator ShowEnemyIntents()
        {
            var enemyIntents = _intentPlanner.GetOrderedEnemyIntents();
            foreach (var enemyIntent in enemyIntents.Where(intent => _aliveEnemies.Contains(intent.Key)))
            {
                var unit = enemyIntent.Key;
                var intents = enemyIntent.Value;
                if (intents.Count == 1)
                {
                    var intent = intents[0];
                    if (intent.type == EnemyIntentType.Move)
                    {
                        yield return StartCoroutine(_intentExecutor.ShowIntent(unit, intent));
                        yield return new WaitForSeconds(1f);
                        yield return _intentExecutor.ShowMoveIntent(unit, intent);
                        yield return new WaitForSeconds(1f);
                        yield return _intentExecutor.ExecuteMoveIntent(unit, intent);
                    }
                    else if (intent.type == EnemyIntentType.Attack)
                    {
                        yield return StartCoroutine(_intentExecutor.ShowIntent(unit, intent));
                        yield return new WaitForSeconds(1f);
                        yield return _intentExecutor.ShowAttackIntent(unit, intent);
                    }
                }
                else if (intents.Count == 2)
                {
                    var moveIntent = intents[0];
                    var attackIntent = intents[1];
                    yield return StartCoroutine(_intentExecutor.ShowIntent(unit, moveIntent));
                    yield return new WaitForSeconds(1f);
                    yield return _intentExecutor.ShowMoveIntent(unit, moveIntent);
                    yield return new WaitForSeconds(1f);
                    yield return _intentExecutor.ExecuteMoveIntent(unit, moveIntent);
                    yield return new WaitForSeconds(1f);
                    yield return _intentExecutor.ShowAttackIntent(unit, attackIntent);
                }
            }

            yield return new WaitForSeconds(1f);
            EnemyIntentsShowFinished = true;
        }

        private IEnumerator ExecuteEnemyIntents()
        {
            var enemyIntents = _intentPlanner.GetOrderedEnemyIntents();
            foreach (var enemyIntent in enemyIntents.Where(intent => _aliveEnemies.Contains(intent.Key)))
            {
                if (enemyIntent.Value.Count == 1)
                {
                    if (enemyIntent.Value[0].type == EnemyIntentType.Attack)
                    {
                        yield return _intentExecutor.ExecuteAttackIntent(enemyIntent.Key, enemyIntent.Value[0]);
                    }
                }
                else if (enemyIntent.Value.Count == 2)
                {
                    yield return _intentExecutor.ExecuteAttackIntent(enemyIntent.Key, enemyIntent.Value[1]);
                }
            }

            yield return new WaitForSeconds(1f);
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