using System;
using System.Collections.Generic;
using System.Linq;
using Action;
using Ally;
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
