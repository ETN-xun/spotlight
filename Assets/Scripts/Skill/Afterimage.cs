using UnityEngine;

/// <summary>
/// 残影类
/// 闪回位移技能留下的临时对象
/// </summary>
public class Afterimage : MonoBehaviour, IObjectOnCell
{
    [Header("残影属性")]
    [Tooltip("残影持续回合数")]
    public int duration = 2;
    
    [Tooltip("残影当前剩余回合")]
    public int remainingTurns;
    
    [Tooltip("创建残影的原始单位")]
    public Unit originalUnit;
    
    [Tooltip("残影名称")]
    public string afterimageName;
    
    [Tooltip("残影所在坐标")]
    public Vector2Int coordinate;
    
    [Tooltip("残影是否可以被攻击")]
    public bool canBeAttacked = true;
    
    [Tooltip("残影生命值")]
    public int hits = 1;
    
    /// <summary>
    /// 初始化残影
    /// </summary>
    /// <param name="original">原始单位</param>
    /// <param name="durationTurns">持续回合数</param>
    public void Initialize(Unit original, int durationTurns = 2)
    {
        originalUnit = original;
        duration = durationTurns;
        remainingTurns = duration;
        afterimageName = $"{original.data.unitName}_残影";
        hits = 1; // 残影通常比较脆弱
        
        // 设置残影的视觉效果
        SetupVisualEffect();
        
        Debug.Log($"残影 {afterimageName} 已创建，持续 {duration} 回合");
    }
    
    /// <summary>
    /// 设置残影的视觉效果
    /// </summary>
    private void SetupVisualEffect()
    {
        // 复制原始单位的外观
        if (originalUnit != null)
        {
            SpriteRenderer originalRenderer = originalUnit.GetComponent<SpriteRenderer>();
            if (originalRenderer != null)
            {
                SpriteRenderer afterimageRenderer = gameObject.AddComponent<SpriteRenderer>();
                afterimageRenderer.sprite = originalRenderer.sprite;
                afterimageRenderer.color = new Color(1f, 1f, 1f, 0.5f); // 半透明效果
                afterimageRenderer.sortingOrder = originalRenderer.sortingOrder - 1;
            }
        }
        
        // 添加闪烁效果
        StartCoroutine(FlickerEffect());
    }
    
    /// <summary>
    /// 闪烁效果协程
    /// </summary>
    private System.Collections.IEnumerator FlickerEffect()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer == null) yield break;
        
        while (gameObject != null)
        {
            // 渐变透明度
            float alpha = Mathf.PingPong(Time.time * 2f, 0.3f) + 0.2f;
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    /// <summary>
    /// 回合结束时调用，减少剩余回合数
    /// </summary>
    public void OnTurnEnd()
    {
        remainingTurns--;
        Debug.Log($"残影 {afterimageName} 剩余回合: {remainingTurns}");
        
        if (remainingTurns <= 0)
        {
            Disappear();
        }
    }
    
    /// <summary>
    /// 残影消失
    /// </summary>
    public void Disappear()
    {
        Debug.Log($"残影 {afterimageName} 消失");
        
        // 播放消失效果
        PlayDisappearEffect();
        
        // 清理格子引用
        GridCell currentCell = GetCurrentCell();
        if (currentCell != null && (Object)currentCell.ObjectOnCell == this)
        {
            currentCell.ObjectOnCell = null;
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
        Debug.Log($"播放残影消失效果");
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
    
    public string Name => afterimageName;
    public Vector2Int Coordinate => coordinate;
    public int Hits => hits;
    
    public void TakeDamage(int damage)
    {
        hits -= damage;
        Debug.Log($"残影 {afterimageName} 受到 {damage} 点伤害，剩余生命: {hits}");
        
        if (hits <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        Debug.Log($"残影 {afterimageName} 被摧毁");
        Disappear();
    }
    
    #endregion
    
    /// <summary>
    /// 检查残影是否可以被指定单位穿过
    /// </summary>
    /// <param name="unit">要检查的单位</param>
    /// <returns>是否可以穿过</returns>
    public bool CanUnitPassThrough(Unit unit)
    {
        // 原始单位可以穿过自己的残影
        if (unit == originalUnit)
        {
            return true;
        }
        
        // 友军可以穿过残影
        if (originalUnit != null && unit.data.isEnemy == originalUnit.data.isEnemy)
        {
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// 检查残影是否可以受到伤害
    /// </summary>
    /// <returns>残影不能受到伤害，返回false</returns>
    public bool CanTakeDamage()
    {
        // 残影是虚幻的，不能受到伤害
        return false;
    }
    
    /// <summary>
    /// 残影被单位穿过时的效果
    /// </summary>
    /// <param name="unit">穿过的单位</param>
    public void OnUnitPassThrough(Unit unit)
    {
        Debug.Log($"{unit.data.unitName} 穿过了残影 {afterimageName}");
        
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
                Debug.Log($"{unit.data.unitName} 因穿过残影而受到数据腐蚀影响");
            }
        }
    }
}