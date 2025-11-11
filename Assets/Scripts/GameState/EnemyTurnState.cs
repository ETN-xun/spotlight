using Common;
using Enemy;
using UnityEngine;
using Ally;
using Level;

/// <summary>
/// 敌人回合状态 - 敌人执行行动的阶段
/// </summary>
public class EnemyTurnState : GameStateBase
{
    private float _turnTimer;
    private const float MaxTurnTime = 5f; 
    private bool _didTeleportZeroAtLevel2 = false;

    public EnemyTurnState(GameManager gameManager) : base(gameManager)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        
        _turnTimer = 0f;
        MessageCenter.Publish(Defines.EnemyTurnStartEvent);

        // 第二关开局一次性：将“零”传送到左上角（x最小，y最大）
        // 仅在第一次敌人回合开始时执行一次
        int levelIndex = LevelManager.Instance != null ? LevelManager.Instance.GetCurrentLevelIndex() : -1;
        if (levelIndex == 2 && !_didTeleportZeroAtLevel2)
        {
            _didTeleportZeroAtLevel2 = true;
            var allies = AllyManager.Instance != null ? AllyManager.Instance.GetAliveAllies() : null;
            if (allies != null)
            {
                var zero = allies.Find(a => a != null && a.data.unitType == UnitType.Zero);
                if (zero != null && GridManager.Instance != null)
                {
                    int targetX = GridManager.Instance.GetMinX();
                    int targetY = GridManager.Instance.GetMaxY();
                    var targetCell = GridManager.Instance.GetCell(new Vector2Int(targetX, targetY));
                    if (targetCell != null)
                    {
                        // 若目标格子被占用且不是零，则与占用者交换位置；否则直接传送
                        var occupant = targetCell.CurrentUnit;
                        if (occupant != null && occupant != zero)
                        {
                            Unit.SwapPositions(zero, occupant);
                        }
                        else if (occupant == null)
                        {
                            zero.PlaceAt(targetCell);
                        }
                        // 若已经在目标位置则不作处理
                    }
                }
            }
        }

        EnemyManager.Instance.StartEnemyTurnFlow();
    }
    
    public override void Update()
    {
        // 更新计时器
        _turnTimer += Time.deltaTime;

        if (EnemyManager.Instance.CurrentEnemyTurn == 1)
        {
            if (EnemyManager.Instance.EnemyIntentsShowFinished)
            {
                gameManager.ChangeGameState(GameState.PlayerTurn);
                return;
            }
        }
        if (EnemyManager.Instance.EnemyIntentsExecuteFinished && EnemyManager.Instance.EnemyIntentsShowFinished)
            gameManager.ChangeGameState(GameState.PlayerTurn);
    }
    
    public override void Exit()
    {
        base.Exit();
        
        Debug.Log("敌人回合结束");
        GridManager.Instance.ClearAllHighlights();
        EnemyManager.Instance.RemoveNullPointerAttackedUnits();
        MessageCenter.Publish(Defines.EnemyTurnEndEvent);
    }
}
