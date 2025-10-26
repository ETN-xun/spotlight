using System;
using System.Collections.Generic;
using System.Linq;
using Action;
using Ally;
using Level;
using UnityEngine;

namespace Enemy.AI
{
    public class EnemyIntentPlanner
    {
        private readonly Dictionary<Unit, List<EnemyIntent>> _enemyIntents = new();
        private readonly IEnemyStrategy _garbledCrawlerStrategy = new GarbledCrawlerStrategy();
        private readonly IEnemyStrategy _crashUndeadStrategy = new CrashUndeadStrategy();
        private readonly IEnemyStrategy _nullPointerStrategy = new NullPointerStrategy();

        public void BuildIntent(Unit enemy)
        {
            var intents = new List<EnemyIntent>();
            switch (enemy.data.unitType)
            {
                case UnitType.GarbledCrawler:
                    intents = _garbledCrawlerStrategy.BuildIntent(enemy);
                    break;
                case UnitType.CrashUndead:
                    intents = _crashUndeadStrategy.BuildIntent(enemy);
                    break;
                case UnitType.NullPointer:
                    intents = _nullPointerStrategy.BuildIntent(enemy);
                    break;
                case UnitType.RecursivePhantom:
                    break;

            }
            _enemyIntents[enemy] = intents;
        }

        public void BuildSpawnIntents(List<Unit> aliveEnemies, LevelDataSO levelData)
        {
            if (levelData == null || levelData.addedEnemyPerTurnCount <= 0)
                return;

            int spawnCount = 0;
            int enemyIndex = 0;
            
            // 只生成指定数量的 Spawn 意图
            while (spawnCount < levelData.addedEnemyPerTurnCount && enemyIndex < aliveEnemies.Count)
            {
                var enemy = aliveEnemies[enemyIndex];
                var spawnLocation = FindSpawnLocation(enemy);
                
                if (spawnLocation != null)
                {
                    var spawnIntent = new EnemyIntent
                    {
                        type = EnemyIntentType.Spawn,
                        priority = enemy.data.aiPriority,
                        spawnUnitType = DetermineSpawnUnitType(levelData),
                        spawnTargetCell = spawnLocation
                    };
                    
                    // 添加到该敌人的意图列表
                    if (_enemyIntents.ContainsKey(enemy))
                    {
                        _enemyIntents[enemy].Add(spawnIntent);
                    }
                    else
                    {
                        _enemyIntents[enemy] = new List<EnemyIntent> { spawnIntent };
                    }
                    
                    spawnCount++;
                }
                
                enemyIndex++;
            }
        }

        private UnitType DetermineSpawnUnitType(LevelDataSO levelData)
        {
            // 根据当前数量和最大限制来决定生成哪种敌人
            int currentGarbledCrawlers = CountEnemyType(UnitType.GarbledCrawler);
            int currentCrashUndead = CountEnemyType(UnitType.CrashUndead);
            int currentNullPointer = CountEnemyType(UnitType.NullPointer);

            // 优先生成数量不足的敌人
            if (currentGarbledCrawlers < levelData.最大乱码爬虫数量)
                return UnitType.GarbledCrawler;
            if (currentCrashUndead < levelData.最大死机亡灵数量)
                return UnitType.CrashUndead;
            if (currentNullPointer < levelData.最大空指针数量)
                return UnitType.NullPointer;

            // 默认返回乱码爬虫
            return UnitType.GarbledCrawler;
        }

        private int CountEnemyType(UnitType unitType)
        {
            return _enemyIntents.Keys.Count(enemy => enemy.data.unitType == unitType);
        }

        private GridCell FindSpawnLocation(Unit enemy)
        {
            // 在敌人周围找一个空闲的格子来生成新敌人
            var attackRange = enemy.GetAttackRange(enemy.CurrentCell);
            
            foreach (var cell in attackRange)
            {
                if (cell.CurrentUnit == null)
                    return cell;
            }

            // 如果攻击范围内没有空位，试试移动范围
            var moveRange = enemy.GetMoveRange();
            foreach (var cell in moveRange)
            {
                if (cell.CurrentUnit == null)
                    return cell;
            }

            return null;
        }

        public Dictionary<Unit, List<EnemyIntent>> GetOrderedEnemyIntents()
        {
            return _enemyIntents
                .Where(kv => kv.Value != null && kv.Value.Count > 0)
                .OrderByDescending(kv => kv.Value.First().priority)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public void ClearIntents()
        {
            _enemyIntents.Clear();
        }
    }
}
