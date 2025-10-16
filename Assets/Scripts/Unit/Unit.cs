using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.Serialization;

public class Unit : MonoBehaviour
{
    public UnitDataSO data; // 静态数据
    public int currentHP { get; private set; }
    
    public int currentTurnActionCount;  // 当前回合已执行的行动次数
    public bool hasMoved;   // 当前回合是否已移动
    public GridCell CurrentCell { get; private set; }
    
    public bool ttIsApplied; // 热量节流是否激活
    
    /// <summary>
    /// 状态效果管理器
    /// </summary>
    public StatusEffectManager StatusEffectManager { get; private set; }

    private void Start()
    {
        InitFromData();
        InitStatusEffectManager();
    }

    private void OnEnable()
    {
        MessageCenter.Subscribe(Defines.PlayerTurnStartEvent, OnTurnStart);
        MessageCenter.Subscribe(Defines.PlayerTurnEndEvent, OnTurnEnd);
    }

    private void OnDisable()
    {
        MessageCenter.Unsubscribe(Defines.PlayerTurnStartEvent, OnTurnStart);
        MessageCenter.Unsubscribe(Defines.PlayerTurnEndEvent, OnTurnEnd);
    }

    /// <summary>
    /// 初始化状态效果管理器
    /// </summary>
    private void InitStatusEffectManager()
    {
        StatusEffectManager = GetComponent<StatusEffectManager>();
        if (StatusEffectManager == null)
        {
            StatusEffectManager = gameObject.AddComponent<StatusEffectManager>();
        }
    }

    public void InitFromData()
    {
        if (data == null) return;
        currentHP = data.maxHP;
        GetComponent<SpriteRenderer>().sprite = data.unitSprite;
    }

    public void PlaceAt(GridCell cell)
    {
        if (cell == null || cell.CurrentUnit != null) return;
        if (CurrentCell != null) CurrentCell.CurrentUnit = null;
        
        CurrentCell = cell;
        cell.CurrentUnit = this;
        
        transform.position = GridManager.Instance.CellToWorld(cell.Coordinate);
    }

    public void MoveTo(GridCell targetCell)
    {
        if (targetCell == null || targetCell.CurrentUnit != null) return;
        PlaceAt(targetCell);
    }

    public void TakeDamage(int dmg)
    {
        if(data.Hits>0)
            data.Hits--;
        else
            currentHP -= dmg;
        Debug.Log($"{data.unitName} 受到 {dmg} 点伤害，剩余生命值: {currentHP}");
        if (currentHP <= 0) Die();
    }
    
    /// <summary>
    /// 获取修正后的攻击力（考虑状态效果）
    /// </summary>
    /// <returns>修正后的攻击力</returns>
    public int GetModifiedDamage()
    {
        int baseDamage = data.baseDamage;
        if (StatusEffectManager != null)
        {
            return StatusEffectManager.GetModifiedDamage(baseDamage);
        }
        return baseDamage;
    }
    
    /// <summary>
    /// 回合开始时调用，应用状态效果
    /// </summary>
    public void OnTurnStart(object[] args)
    {
        if (StatusEffectManager != null)
        {
            StatusEffectManager.ApplyAllEffects();
        }

        hasMoved = false;
        currentTurnActionCount = 0;
    }
    
    /// <summary>
    /// 回合结束时调用，更新状态效果持续时间
    /// </summary>
    public void OnTurnEnd(object[] args)
    {
        if (StatusEffectManager != null)
        {
            StatusEffectManager.UpdateEffectDurations();
        }
    }

    private void Die()
    {
        // 检查是否有死亡感染技能
        CheckAndTriggerDeathInfection();
        MessageCenter.Publish(data.isEnemy ? Defines.EnemyUnitDiedEvent : Defines.AllyUnitDiedEvent, this);
        if (CurrentCell != null) CurrentCell.CurrentUnit = null;
        Destroy(gameObject);
    }
    
    /// <summary>
    /// 检查并触发死亡感染技能
    /// </summary>
    private void CheckAndTriggerDeathInfection()
    {
        if (data.skills == null || CurrentCell == null) return;
        
        foreach (var skillData in data.skills)
        {
            // 检查是否有死亡感染类型的技能
            if (skillData.skillType == SkillType.StatusAbnormal && 
                skillData.skillName.Contains("死亡感染") || skillData.skillName.Contains("DeathInfection"))
            {
                DeathInfectionSkill deathInfection = new DeathInfectionSkill(skillData, this);
                deathInfection.ExecuteDeathInfection(CurrentCell, GridManager.Instance);
                break; // 只触发第一个死亡感染技能
            }
        }
    }
    
    public List<GridCell> GetMoveRange()
    {
        List<GridCell> result = new List<GridCell>();
        int range = data.moveRange;

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                int dist = Mathf.Abs(dx) + Mathf.Abs(dy); 
                if (dist <= range)
                {
                    var target = GridManager.Instance.GetCell(CurrentCell.Coordinate + new Vector2Int(dx, dy));
                    if (target != null && target.CurrentUnit == null)
                    {
                        result.Add(target);
                    }
                }
            }
        }

        return result;
    }
    
    public List<GridCell> GetAttackRange(GridCell targetCell)
    {
        List<GridCell> result = new List<GridCell>();
        int range = data.attackRange;

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                int dist = Mathf.Abs(dx) + Mathf.Abs(dy); 
                if (dist <= range)
                {
                    var target = GridManager.Instance.GetCell(targetCell.Coordinate + new Vector2Int(dx, dy));
                    if (target != null && target.CurrentUnit != null && target.CurrentUnit.data.isEnemy != data.isEnemy)
                    {
                        result.Add(target);
                    }
                }
            }
        }

        return result;
    }
    
    
    public void ApplyTTEffect_temp()
    {
        if (data.isEnemy) return;
        data.moveRange -= 1;
        if (data.moveRange <= 0)
            data.moveRange = 0;
        data.baseDamage -= 1;
        if (data.baseDamage <= 0)
            data.baseDamage = 0;
        ttIsApplied = true;
        Debug.Log("" + data.unitName + " 受到热节流影响，移动力和攻击力各减少1");
    }

    public void CancelTTEffect_temp()
    {
        if (data.isEnemy) return;
        data.moveRange += 1;
        data.baseDamage += 1;
        ttIsApplied = false;
        Debug.Log("" + data.unitName + " 热节流效果结束，移动力和攻击力各恢复1");
    }
}
