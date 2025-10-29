using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Spine.Unity;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Serialization;
using System.Linq;

public class Unit : MonoBehaviour
{
    public GameObject bloodBar;
    public List<Sprite> bloodSprites;
    public UnitDataSO data; // 静态数据
    public int currentHP { get; private set; }
    
    [HideInInspector]
    public int currentTurnActionCount;  // 当前回合已执行的行动次数
    public GridCell CurrentCell { get; private set; }
    [HideInInspector]
    public bool ttIsApplied; // 热量节流是否激活

    public Dictionary<Unit, int> attackedUnits = new();
    
    /// <summary>
    /// 状态效果管理器
    /// </summary>
    public StatusEffectManager StatusEffectManager { get; private set; }

    public void Update()
    {
        // 添加边界检查，防止索引越界
        if (bloodSprites != null && bloodSprites.Count > 0 && currentHP >= 0 && currentHP < bloodSprites.Count)
        {
            bloodBar.GetComponent<SpriteRenderer>().sprite = bloodSprites[currentHP];
        }
        else if (bloodSprites != null && bloodSprites.Count > 0)
        {
            // 如果currentHP超出范围，使用最后一个sprite（通常代表0血量）
            int clampedIndex = Mathf.Clamp(currentHP, 0, bloodSprites.Count - 1);
            bloodBar.GetComponent<SpriteRenderer>().sprite = bloodSprites[clampedIndex];
        }
    }

    private void Start()
    {
        InitFromData();
        InitStatusEffectManager();
        GetComponentInChildren<SkeletonAnimation>().state.SetAnimation(0, "idle", true);
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
    
    public void PlayAnimation(string animationName, bool loop)
    {
        var skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        if (skeletonAnimation != null)
        {
            Debug.Log("Playing animation: " + animationName);
            skeletonAnimation.state.SetAnimation(0, animationName, loop);
        }
    }

    public void InitFromData()
    {
        if (data == null) return;
        currentHP = data.maxHP;
    }

    public void PlaceAt(GridCell cell)
    {
        // 检查对象是否已被销毁
        if (this == null || gameObject == null)
        {
            Debug.LogWarning("尝试移动已销毁的单位");
            return;
        }
        
        if (cell == null || cell.CurrentUnit != null) return;
        if (CurrentCell != null) CurrentCell.CurrentUnit = null;
        
        CurrentCell = cell;
        cell.CurrentUnit = this;
        
        transform.position = new Vector3(GridManager.Instance.CellToWorld(cell.Coordinate).x,GridManager.Instance.CellToWorld(cell.Coordinate).y,GridManager.Instance.CellToWorld(cell.Coordinate).y);
    }

    /// <summary>
    /// 检查单位是否拥有闪回位移技能
    /// </summary>
    /// <returns>如果拥有闪回位移技能返回true，否则返回false</returns>
    private bool HasFlashbackDisplacementSkill()
    {
        if (data == null || data.skills == null) return false;
        
        foreach (var skill in data.skills)
        {
            if (skill == null) continue;
            
            // 检查技能名称是否包含"闪回"或技能ID是否为闪回位移
            if (skill.skillName.Contains("闪回") || skill.skillID == "flashback_displacement_01")
            {
                return true;
            }
        }
        return false;
    }

    public void MoveTo(GridCell targetCell)
    {
        // 检查对象是否已被销毁
        if (this == null || gameObject == null)
        {
            Debug.LogWarning("尝试移动已销毁的单位");
            return;
        }
        
        if (targetCell == null || targetCell.CurrentUnit != null) return;
        if (targetCell.DestructibleObject != null || targetCell.ObjectOnCell != null) return;
        
        // 如果该单位拥有闪回位移技能，记录移动前的位置
        if (HasFlashbackDisplacementSkill() && CurrentCell != null)
        {
            Vector2Int oldPosition = CurrentCell.Coordinate;
            Debug.Log($"[闪回位移] {data.unitName} 移动前位置: ({oldPosition.x}, {oldPosition.y})");
            
            // 获取当前回合数并记录移动
            int currentTurn = GameManager.Instance != null ? GameManager.Instance.CurrentTurn : 1;
            FlashbackDisplacementSkill.RecordMovement(this, CurrentCell, currentTurn);
        }
        
        PlaceAt(targetCell);
    }

    /// <summary>
    /// 单位承受伤害
    /// </summary>
    /// <param name="damage">伤害数值</param>
    public void TakeDamage(int damage)
    {
        if (currentHP<=0) return;

        // 检查是否是友方非虚影角色
        if (!data.isEnemy && GetComponent<FlashbackCopyTag>() == null)
        {
            // 查找所有虚影角色
            var phantoms = FindObjectsOfType<FlashbackCopyTag>()
                .Select(tag => tag.GetComponent<Unit>())
                .Where(unit => unit != null && unit.currentHP>0 && !unit.data.isEnemy)
                .ToList();

            if (phantoms.Any())
            {
                // 随机选择一个虚影来承受伤害
                var randomPhantom = phantoms[UnityEngine.Random.Range(0, phantoms.Count)];
                Debug.Log($"伤害从 {data.unitName} 转移到虚影 {randomPhantom.data.unitName}");
                randomPhantom.TakeDamage(damage);
                return; // 伤害已转移，直接返回
            }
        }

        int remainingDamage = damage;
        
        // 先消耗护盾
        if (data.Hits > 0)
        {
            int shieldDamage = Mathf.Min(data.Hits, remainingDamage);
            data.Hits -= shieldDamage;
            remainingDamage -= shieldDamage;
            Debug.Log($"{data.unitName} 护盾吸收了 {shieldDamage} 点伤害，剩余护盾: {data.Hits}");
        }
        
        // 剩余伤害作用于生命值
        if (remainingDamage > 0)
        {
            currentHP -= remainingDamage;
            Debug.Log($"{data.unitName} 受到 {remainingDamage} 点生命值伤害，剩余生命值: {currentHP}");
        }
        
        MessageCenter.Publish(Defines.UnitTakeDamageEvent, data.unitID);
        if (currentHP <= 0) Die();
    }
    
    /// <summary>
    /// 设置生命值为0并清除护盾，用于秒杀效果
    /// </summary>
    public void SetToZeroHP()
    {
        currentHP = 0;
        data.Hits = 0;
        MessageCenter.Publish(Defines.UnitTakeDamageEvent, data.unitID);
        Die();
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
                    if (target != null && target.CurrentUnit == null && target.DestructibleObject == null && target.ObjectOnCell == null)
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