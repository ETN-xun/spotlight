using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;
using System.Linq;

public class Unit : MonoBehaviour
{
    public GameObject bloodBar;
    public List<Sprite> bloodSprites;
    public UnitDataSO data_ori; // 静态数据
    public UnitDataSO data;
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
    
    /// <summary>
    /// 递归幻影无敌判定：场上存在除自身外的其他敌方单位时，无敌
    /// </summary>
    /// <returns>是否处于无敌状态</returns>
    private bool IsRecursivePhantomInvincible()
    {
        if (!data.isEnemy) return false;
        if (data.unitType != UnitType.RecursivePhantom) return false;

        var enemyMgr = Enemy.EnemyManager.Instance;
        if (enemyMgr == null) return false;

        var othersAlive = enemyMgr.GetAliveEnemies().Any(u => u != null && u != this && u.currentHP > 0);
        return othersAlive;
    }

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
        data=Instantiate(data_ori);
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
            if (!loop)
            {
                // 非循环动画播放完毕后回到idle
                skeletonAnimation.state.AddAnimation(0, "idle", true, 0);
            }
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
    /// 安全交换两个单位的位置：同时更新两者的 CurrentCell 与各自格子的 CurrentUnit，
    /// 并移动 Transform 到对应的世界坐标，避免顺序调用 PlaceAt 导致占用被清空。
    /// </summary>
    public static void SwapPositions(Unit unit1, Unit unit2)
    {
        if (unit1 == null || unit2 == null) return;
        if (unit1 == unit2) return;

        var cell1 = unit1.CurrentCell;
        var cell2 = unit2.CurrentCell;
        if (cell1 == null || cell2 == null) return;

        // 更新格子占用与单位当前格子（原子性地交换，不通过 PlaceAt 的序列化清空逻辑）
        cell1.CurrentUnit = unit2;
        cell2.CurrentUnit = unit1;

        unit1.CurrentCell = cell2;
        unit2.CurrentCell = cell1;

        // 同步位置到世界坐标
        var pos2 = GridManager.Instance.CellToWorld(cell2.Coordinate);
        unit1.transform.position = new Vector3(pos2.x, pos2.y, pos2.y);

        var pos1 = GridManager.Instance.CellToWorld(cell1.Coordinate);
        unit2.transform.position = new Vector3(pos1.x, pos1.y, pos1.y);
    }

    /// <summary>
    /// 单位承受伤害
    /// </summary>
    /// <param name="damage">伤害数值</param>
    public void TakeDamage(int damage)
    {
        if (currentHP<=0) return;

        // 递归幻影：场上仍有其他敌方单位时，免疫伤害
        if (IsRecursivePhantomInvincible())
        {
            Debug.Log($"{data.unitName} 当前无敌：场上还有其他敌方单位，伤害无效");
            return;
        }

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

        int incomingDamage = damage;
        if (StatusEffectManager != null)
        {
            incomingDamage = StatusEffectManager.GetModifiedIncomingDamage(incomingDamage);
        }
        int remainingDamage = incomingDamage;
        
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
        // 递归幻影：场上仍有其他敌方单位时，免疫秒杀效果
        if (IsRecursivePhantomInvincible())
        {
            Debug.Log($"{data.unitName} 当前无敌：场上还有其他敌方单位，秒杀无效");
            return;
        }
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
        int range;
        // 第二关：将零的移动范围锁定为 1
        if (data != null && data.unitType == UnitType.Zero && Level.LevelManager.Instance != null && Level.LevelManager.Instance.GetCurrentLevelIndex() == 2)
        {
            range = 1;
        }
        else
        {
            range = data.moveRange;
        }

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

    // 根据技能的目标设置（友方/敌方）与单位的 AttackRange 计算可选目标
    public List<GridCell> GetSkillTargetRange(GridCell centerCell, SkillDataSO skillData)
    {
        List<GridCell> result = new List<GridCell>();
        // 使用技能自带的范围（如有），否则回退到单位的攻击范围
        int range = skillData != null && skillData.range > 0 ? skillData.range : data.attackRange;
        var center = centerCell.Coordinate;

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                int dist = Mathf.Abs(dx) + Mathf.Abs(dy);
                if (dist > range) continue;

                var pos = center + new Vector2Int(dx, dy);
                var cell = GridManager.Instance.GetCell(pos);
                if (cell == null) continue;

                // 生成类技能（如地形投放）：允许选择空格子，或选择敌方单位所在格子
                if (skillData != null && skillData.skillType == SkillType.Spawn)
                {
                    if (cell.CurrentUnit == null)
                    {
                        result.Add(cell);
                        continue;
                    }

                    bool isTargetEnemySpawn = cell.CurrentUnit.data.isEnemy;
                    bool isCasterEnemySpawn = data.isEnemy;
                    if (isTargetEnemySpawn != isCasterEnemySpawn)
                    {
                        result.Add(cell);
                    }
                    continue;
                }

                // 非生成技能：仅允许选择符合敌我设定的单位格子
                if (cell.CurrentUnit == null) continue;

                bool isTargetEnemy = cell.CurrentUnit.data.isEnemy;
                bool isCasterEnemy = data.isEnemy;

                bool canTarget = false;
                if (skillData.canTargetEnemies && isTargetEnemy != isCasterEnemy)
                    canTarget = true;
                if (skillData.canTargetAllies && isTargetEnemy == isCasterEnemy)
                    canTarget = true;

                if (canTarget)
                    result.Add(cell);
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
