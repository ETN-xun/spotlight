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

        private readonly EnemyIntentPlanner _intentPlanner = new();
        private readonly EnemyIntentExecutor _intentExecutor = new();

        private readonly List<Unit> _enemies = new();
        private readonly List<Unit> _aliveEnemies = new();

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
            var _递归幻影数量 = _currentLevelData.初始递归幻影数量;

            _aliveEnemies.Clear();

            if (_乱码爬虫数量 != 0)
            {
                var prefab = Resources.Load<Unit>("Prefab/Unit/乱码爬虫");
                foreach (var pos in _currentLevelData.乱码爬虫位置)
                {
                    var coord = pos;
                    Utils.Coordinate.Transform(ref coord);
                    GridManager.Instance.PlaceUnit(coord, prefab);
                }
            }

            if (_死机亡灵数量 != 0)
            {
                var prefab = Resources.Load<Unit>("Prefab/Unit/死机亡灵");
                foreach (var pos in _currentLevelData.死机亡灵位置)
                {
                    var coord = pos;
                    Utils.Coordinate.Transform(ref coord);
                    GridManager.Instance.PlaceUnit(coord, prefab);
                }
            }

            if (_空指针数量 != 0)
            {
                var prefab = Resources.Load<Unit>("Prefab/Unit/空指针");
                foreach (var pos in _currentLevelData.空指针位置)
                {
                    var coord = pos;
                    Utils.Coordinate.Transform(ref coord);
                    GridManager.Instance.PlaceUnit(coord, prefab);
                }
            }

            // Boss：递归幻影
            if (_递归幻影数量 != 0)
            {
                var prefab = Resources.Load<Unit>("Prefab/Unit/递归幻影");
                if (prefab == null)
                {
                    Debug.LogError("未找到递归幻影预制：Resources/Prefab/Unit/递归幻影");
                }
                else
                {
                    foreach (var pos in _currentLevelData.递归幻影位置)
                    {
                        var coord = pos;
                        Utils.Coordinate.Transform(ref coord);
                        GridManager.Instance.PlaceUnit(coord, prefab);
                    }
                }
            }
        }

        public Unit GetAliveEnemyByID(string unitID)
        {
            return _aliveEnemies.Find(e => e.data.unitID == unitID);
        }

        public void AddAliveEnemy(Unit enemy)
        {
            _aliveEnemies.Add(enemy);
            _enemies.Add(enemy);
        }

        public List<Unit> GetAliveEnemies()
        {
            return _aliveEnemies;
        }

        public void RemoveNullPointerAttackedUnits()
        {
            foreach (var enemy in _enemies.Where(e => e.data.unitType == UnitType.NullPointer))
            {
                var toRemove = new List<Unit>();
                foreach (var kvp in enemy.attackedUnits.ToList())
                {
                    enemy.attackedUnits[kvp.Key] = kvp.Value - 1;
                    if (enemy.attackedUnits[kvp.Key] <= 0)
                        toRemove.Add(kvp.Key);
                }
                foreach (var unit in toRemove)
                    enemy.attackedUnits.Remove(unit);
            }
        }

        private IEnumerator EnemyTurnFlow()
        {
            CurrentEnemyTurn++;
            EnemyIntentsExecuteFinished = false;
            EnemyIntentsShowFinished = false;

            if (CurrentEnemyTurn == 1)
            {
                yield return FirstTurnFlow();
            }
            else
            {
                yield return LaterTurnFlow();
            }
        }

        private IEnumerator FirstTurnFlow()
        {
            var intents = _intentPlanner.GetOrderedEnemyIntents();
            foreach (var pair in intents.Where(i => _aliveEnemies.Contains(i.Key)))
            {
                var unit = pair.Key;
                var unitIntents = pair.Value;

                foreach (var intent in unitIntents)
                {
                    yield return StartCoroutine(_intentExecutor.ShowIntent(unit, intent));
                    yield return new WaitForSeconds(0.5f);

                    if (intent.type == EnemyIntentType.Move)
                    {
                        yield return _intentExecutor.ExecuteMoveIntent(unit, intent);
                    }

                    if (intent.type == EnemyIntentType.Attack)
                    {
                        yield return StartCoroutine(_intentExecutor.ShowAttackIntent(unit, intent));
                    }
                }
            }

            GridManager.Instance.ClearAllHighlights();
            yield return new WaitForSeconds(1f);
            yield return GenerateNewIntents();
            EnemyIntentsShowFinished = true;
            yield break;
        }

        private IEnumerator LaterTurnFlow()
        {
            var intents = _intentPlanner.GetOrderedEnemyIntents();

            foreach (var pair in intents.Where(i => _aliveEnemies.Contains(i.Key)))
            {
                var unit = pair.Key;
                var unitIntents = pair.Value;

                foreach (var intent in unitIntents)
                {
                    if (intent.type == EnemyIntentType.Attack)
                        yield return _intentExecutor.ExecuteAttackIntent(unit, intent);
                    else if (intent.type == EnemyIntentType.Spawn)
                        yield return _intentExecutor.ExecuteSpawnIntent(unit, intent);
                }
            }

            yield return GenerateNewIntents();
            yield return ShowAndExecuteMoveIntents();
        }

        private IEnumerator ShowAndExecuteMoveIntents()
        {
            var intents = _intentPlanner.GetOrderedEnemyIntents();

            foreach (var pair in intents.Where(i => _aliveEnemies.Contains(i.Key)))
            {
                var unit = pair.Key;
                var unitIntents = pair.Value;

                foreach (var intent in unitIntents)
                {
                    if (intent.type == EnemyIntentType.Move)
                    {
                        yield return StartCoroutine(_intentExecutor.ShowIntent(unit, intent));
                        yield return new WaitForSeconds(0.5f);
                        yield return _intentExecutor.ExecuteMoveIntent(unit, intent);
                    }
                }
            }

            GridManager.Instance.ClearAllHighlights();
            EnemyIntentsExecuteFinished = true;
            // 标记本回合意图展示完成
            EnemyIntentsShowFinished = true;
            yield return null;
        }

        private IEnumerator GenerateNewIntents()
        {
            foreach (var enemy in _aliveEnemies)
                _intentPlanner.BuildIntent(enemy);

            _intentPlanner.BuildSpawnIntents(_aliveEnemies, _currentLevelData);
            yield return null;
        }

        private void OnEnemyUnitDied(object[] args)
        {
            if (args[0] is not Unit unit) return;
            if (_aliveEnemies.Contains(unit))
                _aliveEnemies.Remove(unit);

            if (_aliveEnemies.Count == 0)
            {
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
