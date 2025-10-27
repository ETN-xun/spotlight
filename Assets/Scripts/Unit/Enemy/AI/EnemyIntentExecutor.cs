using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Enemy.AI
{
    public class EnemyIntentExecutor
    {
        public IEnumerator ShowMoveIntent(Unit enemy, EnemyIntent intent)
        {
            if (intent is not { type: EnemyIntentType.Move }) 
                yield break;
            
            if (enemy.data.unitType == UnitType.NullPointer)        // Special case for NullPointer: only highlight target cell
            {
                GridManager.Instance.Highlight(true, intent.moveTargetCell.Coordinate);
                yield return new WaitForSeconds(0.5f);
                yield break;
            }
            
            if (intent.movePath is null || intent.movePath.Count == 0)
                yield break;

            foreach (var pathCell in intent.movePath)
                GridManager.Instance.Highlight(true, pathCell.Coordinate);

            yield return new WaitForSeconds(0.5f);
        }
        
        public IEnumerator ShowAttackIntent(Unit enemy, EnemyIntent intent)
        {
            if (intent is not { type: EnemyIntentType.Attack }) 
                yield break;

            GridManager.Instance.Highlight(true, intent.attackTargetCell.Coordinate);
            yield return new WaitForSeconds(0.5f);
        }
        
        public IEnumerator ShowSpawnIntent(Unit enemy, EnemyIntent intent)
        {
            if (intent is not { type: EnemyIntentType.Spawn }) 
                yield break;

            GridManager.Instance.Highlight(true, intent.spawnTargetCell.Coordinate);
            yield return new WaitForSeconds(0.5f);
        }
        
        public IEnumerator ExecuteSpawnIntent(Unit enemy, EnemyIntent intent)
        {
            if (intent is not { type: EnemyIntentType.Spawn }) 
                yield break;

            if (intent.spawnTargetCell == null)
                yield break;

            Debug.Log($"Enemy {enemy.data.unitName} spawns {intent.spawnUnitType} at cell {intent.spawnTargetCell.Coordinate}");
            
            // Load the unit prefab based on spawn type
            Unit spawnedUnit = intent.spawnUnitType switch
            {
                UnitType.GarbledCrawler => Resources.Load<Unit>("Prefab/Unit/乱码爬虫"),
                UnitType.CrashUndead => Resources.Load<Unit>("Prefab/Unit/死机亡灵"),
                UnitType.NullPointer => Resources.Load<Unit>("Prefab/Unit/空指针"),
                _ => null
            };

            if (spawnedUnit != null && intent.spawnTargetCell.CurrentUnit == null)
            {
                GridManager.Instance.PlaceUnit(intent.spawnTargetCell.Coordinate, spawnedUnit);
                Debug.Log($"Successfully spawned {intent.spawnUnitType}!");
            }
            else
            {
                Debug.Log($"Failed to spawn unit - cell occupied or invalid unit type");
            }

            yield return new WaitForSeconds(0.3f);
        }
        
        public IEnumerator ExecuteMoveIntent(Unit enemy, EnemyIntent intent)
        {
            GridManager.Instance.ClearAllHighlights();
            if (intent is not { type: EnemyIntentType.Move }) 
                yield break;
            
            if (enemy.data.unitType == UnitType.NullPointer)        // Special case for NullPointer: no movement
            {
                enemy.MoveTo(intent.moveTargetCell);
                yield break;
            }
            
            if (intent.movePath == null || intent.movePath.Count == 0)
                yield break;

            yield return MovementSystem.Instance.MoveUnitByPathCoroutine(enemy, intent.movePath);
            yield return new WaitForSeconds(0.2f);
        }
        
        public IEnumerator ExecuteAttackIntent(Unit enemy, EnemyIntent intent)
        {
            if (intent is not { type: EnemyIntentType.Attack }) 
                yield break;
            
            if (intent.attackTargetCell == null)
                yield break;

            Debug.Log($"Enemy {enemy.data.unitName} attacks cell {intent.attackTargetCell.Coordinate}");
            var targetUnit = intent.attackTargetCell.CurrentUnit;
            if (targetUnit != null)
            {
                targetUnit.TakeDamage(enemy.data.baseDamage);
                if (!enemy.attackedUnits.TryAdd(targetUnit, 2))
                    enemy.attackedUnits[targetUnit] = 2; 
                Debug.Log($"{targetUnit.data.unitName} took {enemy.data.baseDamage} damage!");
            }
            else
            {
                Debug.Log("No target in that cell, attack wasted.");
            }

            yield return new WaitForSeconds(0.3f);
        }
        
        public IEnumerator ShowIntent(Unit enemy, EnemyIntent intent)
        {
            if (intent == null) yield break;

            switch (intent.type)
            {
                case EnemyIntentType.None:
                    yield break;

                case EnemyIntentType.Move:
                    if (enemy.data.unitType == UnitType.NullPointer)
                    {
                        GridManager.Instance.Highlight(true, intent.moveTargetCell.Coordinate);
                        yield return new WaitForSeconds(0.5f);
                        break;
                    }
                    foreach (var pathCell in intent.movePath)
                        GridManager.Instance.Highlight(true, pathCell.Coordinate);

                    yield return new WaitForSeconds(0.5f);
                    break;

                case EnemyIntentType.Attack:
                    GridManager.Instance.Highlight(true, intent.attackTargetCell.Coordinate);
                    yield return new WaitForSeconds(0.5f);
                    break;

                case EnemyIntentType.Spawn:
                    break;
            }

            yield return null;
        }
        
    }
}
