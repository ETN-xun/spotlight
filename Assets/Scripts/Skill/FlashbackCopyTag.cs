using UnityEngine;
using Common;

/// <summary>
/// 闪回复制标签组件
/// 用于标识和管理闪回位移技能创建的复制体
/// </summary>
public class FlashbackCopyTag : MonoBehaviour
{
    [Header("复制信息")]
    [Tooltip("原始单位引用")]
    public Unit originalUnit;
    
    [Tooltip("创建时间")]
    public float creationTime;
    
    [Tooltip("持续回合数")]
    public int duration = 2;
    
    [Tooltip("剩余回合数")]
    public int remainingTurns;
    
    /// <summary>
    /// 初始化闪回复制
    /// </summary>
    /// <param name="original">原始单位</param>
    /// <param name="durationTurns">持续回合数</param>
    public void Initialize(Unit original, int durationTurns = 1)
    {
        originalUnit = original;
        duration = durationTurns;
        remainingTurns = duration;
        creationTime = Time.time;
        
        // 只监听敌人回合结束事件，这样复制会在敌人回合结束后减少计数
        // 这确保复制能够存活完整的回合周期
        MessageCenter.Subscribe(Defines.EnemyTurnEndEvent, OnTurnEnd);
        
        Debug.Log($"闪回复制 {gameObject.name} 已创建，持续 {duration} 回合");
    }
    
    /// <summary>
    /// 回合结束时调用，减少剩余回合数
    /// </summary>
    public void OnTurnEnd(object[] args)
    {
        DestroyCopy();
        /*
        remainingTurns--;
        Debug.Log($"闪回复制 {gameObject.name} 剩余回合: {remainingTurns}");
        
        if (remainingTurns <= 0)
        {
            DestroyCopy();
        }
        */
    }
    
    /// <summary>
    /// 销毁复制品
    /// </summary>
    public void DestroyCopy()
    {
        Debug.Log($"闪回复制 {gameObject.name} 消失");
        
        // 取消事件订阅
        MessageCenter.Unsubscribe(Defines.EnemyTurnEndEvent, OnTurnEnd);
        
        // 清理格子引用
        Unit unit = GetComponent<Unit>();
        if (unit != null && unit.CurrentCell != null)
        {
            unit.CurrentCell.CurrentUnit = null;
        }
        
        // 播放消失效果
        PlayDisappearEffect();
        
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
        Debug.Log($"播放闪回复制消失效果");
    }
    
    /// <summary>
    /// 当组件被销毁时清理事件订阅
    /// </summary>
    private void OnDestroy()
    {
        MessageCenter.Unsubscribe(Defines.EnemyTurnEndEvent, OnTurnEnd);
    }
}