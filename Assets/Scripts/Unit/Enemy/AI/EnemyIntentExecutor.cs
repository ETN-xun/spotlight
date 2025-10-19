using System;
using System.Collections;
using UnityEngine;

namespace Enemy.AI
{
    public class EnemyIntentExecutor
    {
        public IEnumerator ShowIntent(Unit enemy, EnemyIntent intent)
        {
            switch (intent.type)
            {
                case EnemyIntentType.None:
                    break;
                case EnemyIntentType.Move:
                    var path = MovementSystem.Instance.FindPath(enemy.CurrentCell, intent.moveTargetCell);
                    // TODO: 暂且是路径高亮
                    foreach (var pathCell in path)
                    {
                        GridManager.Instance.Highlight(true, pathCell.Coordinate);
                    }
                    yield return new WaitForSeconds(0.5f);
                    break;
                case EnemyIntentType.Attack:
                    // TODO: 暂且是目标格子高亮
                    GridManager.Instance.Highlight(true, intent.attackTargetCell.Coordinate);
                    break;
                case EnemyIntentType.Spawn:
                    break;
            }
            yield return null;
        }
        
        public IEnumerator ExecuteIntent(Unit enemy, EnemyIntent intent)
        {
            switch (intent.type)
            {
                case EnemyIntentType.None:
                    break;
                case EnemyIntentType.Move:
                    var path = MovementSystem.Instance.FindPath(enemy.CurrentCell, intent.moveTargetCell);
                    yield return MovementSystem.Instance.MoveUnitByPathCoroutine(enemy, path);
                    break;
                case EnemyIntentType.Attack:
                    break;
                case EnemyIntentType.Spawn:
                    break;
                default:
                    break;
            }
            yield return null;
        }
    }
}