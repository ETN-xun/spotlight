using UnityEngine;
using Spine.Unity;
using Common;

/// <summary>
/// 单位复制类
/// 闪回位移技能留下的完全相同的实例复制
/// </summary>
public class UnitCopy : MonoBehaviour, IObjectOnCell
{
    [Header("复制单位属性")]
    [Tooltip("复制持续回合数")]
    public int duration = 2;
    
    [Tooltip("复制当前剩余回合")]
    public int remainingTurns;
    
    [Tooltip("创建复制的原始单位")]
    public Unit originalUnit;
    
    [Tooltip("复制单位名称")]
    public string copyName;
    
    [Tooltip("复制单位所在坐标")]
    public Vector2Int coordinate;
    
    [Tooltip("复制单位是否可以被攻击")]
    public bool canBeAttacked = true;
    
    [Tooltip("复制单位生命值")]
    public int hits;
    
    [Tooltip("复制单位的数据")]
    public UnitDataSO copyData;
    
    [Tooltip("复制单位的当前生命值")]
    public int currentHP;
    
    /// <summary>
    /// 初始化单位复制
    /// </summary>
    /// <param name="original">原始单位</param>
    /// <param name="durationTurns">持续回合数</param>
    public void Initialize(Unit original, int durationTurns = 2)
    {
        originalUnit = original;
        duration = durationTurns;
        remainingTurns = duration;
        copyName = $"{original.data.unitName}_复制";
        
        // 复制原始单位的所有数据
        CopyUnitData(original);
        
        // 设置复制的视觉效果
        SetupVisualEffect();
        
        // 注册回合事件监听
        MessageCenter.Subscribe(Defines.PlayerTurnEndEvent, OnTurnEnd);
        MessageCenter.Subscribe(Defines.EnemyTurnEndEvent, OnTurnEnd);
        
        Debug.Log($"单位复制 {copyName} 已创建，持续 {duration} 回合");
    }
    
    /// <summary>
    /// 复制原始单位的数据
    /// </summary>
    /// <param name="original">原始单位</param>
    private void CopyUnitData(Unit original)
    {
        // 创建数据副本
        copyData = ScriptableObject.CreateInstance<UnitDataSO>();
        
        // 复制所有基础属性
        copyData.unitID = original.data.unitID + "_copy";
        copyData.unitName = original.data.unitName;
        copyData.unitType = original.data.unitType;
        copyData.description = original.data.description;
        copyData.unitIcon = original.data.unitIcon;
        copyData.unitPrefab = original.data.unitPrefab;
        copyData.isEnemy = original.data.isEnemy;
        
        // 复制属性值
        copyData.maxHP = original.data.maxHP;
        copyData.Hits = original.data.Hits;
        copyData.moveRange = original.data.moveRange;
        copyData.attackRange = original.data.attackRange;
        copyData.movementEnergyCost = original.data.movementEnergyCost;
        copyData.Energy = original.data.Energy;
        copyData.RecoverEnergy = original.data.RecoverEnergy;
        copyData.baseDamage = original.data.baseDamage;
        copyData.canDestroy = original.data.canDestroy;
        copyData.collisionDamage = original.data.collisionDamage;
        
        // 复制技能列表
        if (original.data.skills != null)
        {
            copyData.skills = new SkillDataSO[original.data.skills.Length];
            for (int i = 0; i < original.data.skills.Length; i++)
            {
                copyData.skills[i] = original.data.skills[i];
            }
        }
        
        // 设置当前状态
        currentHP = original.currentHP;
        hits = original.data.Hits;
    }
    
    /// <summary>
    /// 设置复制的视觉效果
    /// </summary>
    private void SetupVisualEffect()
    {
        if (originalUnit == null) return;
        
        // 复制Spine动画组件
        SkeletonAnimation originalSkeleton = originalUnit.GetComponentInChildren<SkeletonAnimation>();
        if (originalSkeleton != null)
        {
            // 创建子对象来放置Spine动画
            GameObject skeletonObj = new GameObject("SkeletonAnimation");
            skeletonObj.transform.SetParent(transform);
            skeletonObj.transform.localPosition = Vector3.zero;
            
            SkeletonAnimation copySkeleton = skeletonObj.AddComponent<SkeletonAnimation>();
            copySkeleton.skeletonDataAsset = originalSkeleton.skeletonDataAsset;
            copySkeleton.initialSkinName = originalSkeleton.initialSkinName;
            copySkeleton.initialFlipX = originalSkeleton.initialFlipX;
            copySkeleton.initialFlipY = originalSkeleton.initialFlipY;
            
            // 初始化骨骼动画
            copySkeleton.Initialize(false);
            
            // 设置透明度为50%
            if (copySkeleton.skeleton != null)
            {
                copySkeleton.skeleton.A = 0.5f; // 设置透明度为50%
            }
            
            // 播放idle动画
            copySkeleton.state.SetAnimation(0, "idle", true);
        }
        
        // 复制血条
        if (originalUnit.bloodBar != null)
        {
            GameObject bloodBarCopy = Instantiate(originalUnit.bloodBar, transform);
            bloodBarCopy.name = "BloodBar_Copy";
            
            // 设置血条透明度
            SpriteRenderer bloodRenderer = bloodBarCopy.GetComponent<SpriteRenderer>();
            if (bloodRenderer != null)
            {
                Color bloodColor = bloodRenderer.color;
                bloodColor.a = 0.5f;
                bloodRenderer.color = bloodColor;
            }
        }
        
        Debug.Log($"复制单位视觉效果设置完成，透明度50%");
    }
    
    /// <summary>
    /// 回合结束时调用，减少剩余回合数
    /// </summary>
    public void OnTurnEnd(object[] args)
    {
        remainingTurns--;
        Debug.Log($"单位复制 {copyName} 剩余回合: {remainingTurns}");
        
        if (remainingTurns <= 0)
        {
            Disappear();
        }
    }
    
    /// <summary>
    /// 复制消失
    /// </summary>
    public void Disappear()
    {
        Debug.Log($"单位复制 {copyName} 消失");
        
        // 取消事件订阅
        MessageCenter.Unsubscribe(Defines.PlayerTurnEndEvent, OnTurnEnd);
        MessageCenter.Unsubscribe(Defines.EnemyTurnEndEvent, OnTurnEnd);
        
        // 播放消失效果
        PlayDisappearEffect();
        
        // 清理格子引用
        GridCell currentCell = GetCurrentCell();
        if (currentCell != null && (Object)currentCell.ObjectOnCell == this)
        {
            currentCell.ObjectOnCell = null;
        }
        
        // 清理数据副本
        if (copyData != null)
        {
            DestroyImmediate(copyData);
        }
        
        // 销毁游戏对象
        Destroy(gameObject);
    }
    
    /// <summary>
    /// 播放消失效果
    /// </summary>
    private void PlayDisappearEffect()
    {
        // 这里可以添加消失的视觉效果
        // 例如粒子效果、渐隐动画等
        Debug.Log($"播放单位复制消失效果");
    }
    
    /// <summary>
    /// 获取当前所在格子
    /// </summary>
    /// <returns>当前格子</returns>
    private GridCell GetCurrentCell()
    {
        if (GridManager.Instance != null)
        {
            return GridManager.Instance.GetCell(coordinate);
        }
        return null;
    }
    
    #region IObjectOnCell 接口实现
    
    public string Name => copyName;
    public Vector2Int Coordinate => coordinate;
    public int Hits => hits;
    
    public void TakeDamage(int damage)
    {
        // 先消耗护盾
        if (hits > 0)
        {
            int shieldDamage = Mathf.Min(hits, damage);
            hits -= shieldDamage;
            damage -= shieldDamage;
            Debug.Log($"单位复制 {copyName} 护盾吸收了 {shieldDamage} 点伤害，剩余护盾: {hits}");
        }
        
        // 剩余伤害作用于生命值
        if (damage > 0)
        {
            currentHP -= damage;
            Debug.Log($"单位复制 {copyName} 受到 {damage} 点生命值伤害，剩余生命值: {currentHP}");
        }
        
        if (currentHP <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        Debug.Log($"单位复制 {copyName} 被摧毁");
        Disappear();
    }
    
    #endregion
    
    /// <summary>
    /// 检查复制是否可以被指定单位穿过
    /// </summary>
    /// <param name="unit">要检查的单位</param>
    /// <returns>是否可以穿过</returns>
    public bool CanUnitPassThrough(Unit unit)
    {
        // 原始单位可以穿过自己的复制
        if (unit == originalUnit)
        {
            return true;
        }
        
        // 友军可以穿过复制
        if (originalUnit != null && unit.data.isEnemy == originalUnit.data.isEnemy)
        {
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// 检查复制是否可以受到伤害
    /// </summary>
    /// <returns>复制可以受到伤害</returns>
    public bool CanTakeDamage()
    {
        return canBeAttacked;
    }
    
    /// <summary>
    /// 复制被单位穿过时的效果
    /// </summary>
    /// <param name="unit">穿过的单位</param>
    public void OnUnitPassThrough(Unit unit)
    {
        Debug.Log($"{unit.data.unitName} 穿过了单位复制 {copyName}");
        
        // 可以在这里添加特殊效果
        // 例如：给穿过的敌方单位施加状态异常
        if (originalUnit != null && unit.data.isEnemy != originalUnit.data.isEnemy)
        {
            // 对敌方单位施加轻微的状态异常
            if (unit.StatusEffectManager != null)
            {
                unit.StatusEffectManager.AddStatusEffect(
                    StatusAbnormalType.DataCorruption,
                    duration: 1,
                    intensity: 0.5f
                );
                Debug.Log($"{unit.data.unitName} 因穿过单位复制而受到数据腐蚀影响");
            }
        }
    }
}