using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Enemy.AI;
using Level;
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

        private int _乱码爬虫数量;
        private int _死机亡灵数量;
        private int _空指针数量;
        
        private LevelDataSO _currentLevelData;

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

        public void InitEnemies()
        {
            _currentLevelData = LevelManager.Instance.GetCurrentLevel();
            if (_currentLevelData is null) return;
            _乱码爬虫数量 = _currentLevelData.初始乱码爬虫数量;
            _死机亡灵数量 = _currentLevelData.初始死机亡灵数量;
            _空指针数量 = _currentLevelData.初始空指针数量;
            
            _aliveEnemies.Clear();

            if (_乱码爬虫数量 != 0)
            {
                var 乱码爬虫 = Resources.Load<Unit>("Prefab/Unit/乱码爬虫");
                foreach (var pos in _currentLevelData.乱码爬虫位置)
                {
                    var coord = pos;
                    Utils.Coordinate.Transform(ref coord);
                    GridManager.Instance.PlaceUnit(coord, 乱码爬虫);
                }
            }
            if (_死机亡灵数量 != 0)
            {
                var 死机亡灵 = Resources.Load<Unit>("Prefab/Unit/死机亡灵");
                foreach (var pos in _currentLevelData.死机亡灵位置)
                {
                    var coord = pos;
                    Utils.Coordinate.Transform(ref coord);
                    GridManager.Instance.PlaceUnit(coord, 死机亡灵);
                }
            }
            if (_空指针数量 != 0)
            {
                var 空指针 = Resources.Load<Unit>("Prefab/Unit/空指针");
                foreach (var pos in _currentLevelData.空指针位置)
                {
                    var coord = pos;
                    Utils.Coordinate.Transform(ref coord);
                    GridManager.Instance.PlaceUnit(coord, 空指针);
                }
            }

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
        
        private IEnumerator EnemyTurnFlow( )
        {
            CurrentEnemyTurn++;
            EnemyIntentsExecuteFinished = false;
            EnemyIntentsShowFinished = false;
            Debug.Log("敌人回合第 " + CurrentEnemyTurn + " 轮开始");

            // 第一回合：执行部署时生成的意图，然后显示新意图
            if (CurrentEnemyTurn == 1)
            {
                yield return ExecuteEnemyIntents();
                yield return ShowEnemyIntents();
                yield return GenerateNewIntents();
                yield break;
            }

            // 之后的回合：执行意图 → 显示意图 → 生成新意图
            yield return ExecuteEnemyIntents();
            yield return ShowEnemyIntents();
            yield return GenerateNewIntents();
        }
        
        private IEnumerator ShowEnemyIntents()
        {
            var enemyIntents = _intentPlanner.GetOrderedEnemyIntents();
            foreach (var enemyIntent in enemyIntents.Where(intent => _aliveEnemies.Contains(intent.Key)))
            {
                var unit = enemyIntent.Key;
                var intents = enemyIntent.Value;
                
                // Process intents based on their count and types
                foreach (var intent in intents)
                {
                    if (intent.type == EnemyIntentType.Move)
                    {
                        yield return StartCoroutine(_intentExecutor.ShowIntent(unit, intent));
                        yield return new WaitForSeconds(1f);
                        yield return _intentExecutor.ShowMoveIntent(unit, intent);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (intent.type == EnemyIntentType.Attack)
                    {
                        yield return StartCoroutine(_intentExecutor.ShowIntent(unit, intent));
                        yield return new WaitForSeconds(1f);
                        yield return _intentExecutor.ShowAttackIntent(unit, intent);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (intent.type == EnemyIntentType.Spawn)
                    {
                        yield return StartCoroutine(_intentExecutor.ShowIntent(unit, intent));
                        yield return new WaitForSeconds(1f);
                        yield return _intentExecutor.ShowSpawnIntent(unit, intent);
                        yield return new WaitForSeconds(1f);
                    }
                }
            }

            GridManager.Instance.ClearAllHighlights();
            yield return new WaitForSeconds(1f);
            EnemyIntentsShowFinished = true;
        }

        private IEnumerator ExecuteEnemyIntents()
        {
            var enemyIntents = _intentPlanner.GetOrderedEnemyIntents();
            foreach (var enemyIntent in enemyIntents.Where(intent => _aliveEnemies.Contains(intent.Key)))
            {
                // Execute all intents in order (Move, Attack, Spawn)
                foreach (var intent in enemyIntent.Value)
                {
                    if (intent.type == EnemyIntentType.Move)
                    {
                        yield return _intentExecutor.ExecuteMoveIntent(enemyIntent.Key, intent);
                    }
                    else if (intent.type == EnemyIntentType.Attack)
                    {
                        yield return _intentExecutor.ExecuteAttackIntent(enemyIntent.Key, intent);
                    }
                    else if (intent.type == EnemyIntentType.Spawn)
                    {
                        yield return _intentExecutor.ExecuteSpawnIntent(enemyIntent.Key, intent);
                    }
                }
            }

            yield return new WaitForSeconds(1f);
        }

        private IEnumerator GenerateNewIntents()
        {
            foreach (var enemy in _aliveEnemies)
                _intentPlanner.BuildIntent(enemy);
            
            // 生成 Spawn 意图（根据 LevelData 的配置）
            _intentPlanner.BuildSpawnIntents(_aliveEnemies, _currentLevelData);
            
            EnemyIntentsExecuteFinished = true;
            yield return null;
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