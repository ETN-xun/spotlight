using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 递归召唤技能
/// 每三回合召唤一个乱码爬虫
/// </summary>
public class RecursiveSummonSkill : Skill
{
    [Header("递归召唤设置")]
    public int summonInterval = 3;          // 召唤间隔（回合数）
    public string summonUnitName = "乱码爬虫"; // 召唤的单位名称
    
    private int turnCounter = 0;            // 回合计数器
    private bool isActive = false;          // 技能是否激活

    public RecursiveSummonSkill(SkillDataSO data, Unit caster) : base(data, caster) {}

    public override void Execute(GridCell targetCell, GridManager gridManager)
    {
        if (caster == null)
        {
            Debug.LogWarning("递归召唤技能：施法者无效");
            return;
        }

        // 激活递归召唤
        if (!isActive)
        {
            ActivateRecursiveSummon();
            Debug.Log($"递归召唤技能已激活，将每{summonInterval}回合召唤一个{summonUnitName}");
        }
        else
        {
            Debug.Log("递归召唤技能已经激活");
        }
    }

    /// <summary>
    /// 激活递归召唤
    /// </summary>
    private void ActivateRecursiveSummon()
    {
        isActive = true;
        turnCounter = 0;

        // 注册回合事件监听
        RegisterTurnEvents();

        // 立即召唤第一个单位
        TrySummonUnit();
    }

    /// <summary>
    /// 注册回合事件监听
    /// </summary>
    private void RegisterTurnEvents()
    {
        // 如果有回合管理系统，在这里注册事件
        // 这里使用简单的方法，通过GameManager或其他系统来监听回合变化
        
        // 添加递归召唤组件到施法者身上，用于持续监听
        RecursiveSummonComponent component = caster.gameObject.GetComponent<RecursiveSummonComponent>();
        if (component == null)
        {
            component = caster.gameObject.AddComponent<RecursiveSummonComponent>();
        }
        
        component.Initialize(this);
    }

    /// <summary>
    /// 回合开始时调用
    /// </summary>
    public void OnTurnStart()
    {
        if (!isActive) return;

        turnCounter++;
        Debug.Log($"递归召唤：当前回合计数 {turnCounter}");

        if (turnCounter >= summonInterval)
        {
            TrySummonUnit();
            turnCounter = 0; // 重置计数器
        }
    }

    /// <summary>
    /// 尝试召唤单位
    /// </summary>
    private void TrySummonUnit()
    {
        if (caster == null || caster.CurrentCell == null)
        {
            Debug.LogWarning("递归召唤：施法者或其位置无效");
            return;
        }

        GridManager gridManager = GridManager.Instance;
        if (gridManager == null)
        {
            Debug.LogError("递归召唤：找不到GridManager");
            return;
        }

        // 查找召唤位置
        GridCell summonCell = FindSummonPosition(caster.CurrentCell, gridManager);
        if (summonCell == null)
        {
            Debug.LogWarning("递归召唤：没有可用的召唤位置");
            return;
        }

        // 获取乱码爬虫的单位数据
        UnitDataSO summonUnitData = GetSummonUnitData();
        if (summonUnitData == null)
        {
            Debug.LogError($"递归召唤：找不到{summonUnitName}的单位数据");
            return;
        }

        // 召唤单位
        Unit summonedUnit = SummonUnit(summonUnitData, summonCell, gridManager);
        if (summonedUnit != null)
        {
            Debug.Log($"递归召唤：成功召唤了{summonUnitName}在位置({summonCell.Coordinate.x}, {summonCell.Coordinate.y})");
            PlaySummonEffect(summonCell);
        }
    }

    /// <summary>
    /// 查找召唤位置
    /// </summary>
    private GridCell FindSummonPosition(GridCell centerCell, GridManager gridManager)
    {
        Vector2Int center = centerCell.Coordinate;
        
        // 优先在施法者周围寻找空位
        for (int range = 1; range <= 3; range++)
        {
            List<GridCell> candidateCells = new List<GridCell>();
            
            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    if (dx == 0 && dy == 0) continue; // 跳过中心位置
                    
                    Vector2Int pos = center + new Vector2Int(dx, dy);
                    
                    if (gridManager.IsValidPosition(pos))
                    {
                        GridCell cell = gridManager.GetCell(pos);
                        if (cell != null && cell.CurrentUnit == null)
                        {
                            candidateCells.Add(cell);
                        }
                    }
                }
            }
            
            // 如果找到候选位置，随机选择一个
            if (candidateCells.Count > 0)
            {
                return candidateCells[Random.Range(0, candidateCells.Count)];
            }
        }
        
        return null;
    }

    /// <summary>
    /// 获取召唤单位的数据
    /// </summary>
    private UnitDataSO GetSummonUnitData()
    {
        // 尝试从DataManager获取单位数据
        DataManager dataManager = DataManager.Instance;
        if (dataManager != null)
        {
            // 假设DataManager有获取单位数据的方法
            // 这里需要根据实际的DataManager实现来调整
                        // 尝试通过名称查找单位数据
            var allUnits = dataManager.GetEnemyUnits();
            foreach (var unit in allUnits)
            {
                if (unit.unitName == summonUnitName)
                {
                    return unit;
                }
            }
            
            // 如果没找到，尝试通过ID查找
            return dataManager.GetUnitData(summonUnitName);
        }

        // 如果没有DataManager，尝试从Resources加载
        UnitDataSO unitData = Resources.Load<UnitDataSO>($"UnitData/{summonUnitName}");
        if (unitData != null)
        {
            return unitData;
        }

        // 创建默认的乱码爬虫数据
        return CreateDefaultCorruptedCrawlerData();
    }

    /// <summary>
    /// 创建默认的乱码爬虫数据
    /// </summary>
    private UnitDataSO CreateDefaultCorruptedCrawlerData()
    {
        UnitDataSO crawlerData = ScriptableObject.CreateInstance<UnitDataSO>();
        crawlerData.unitName = summonUnitName;
        crawlerData.maxHP = 30;
                crawlerData.baseDamage = 15;
        crawlerData.moveRange = 2;
        crawlerData.isEnemy = true; // 乱码爬虫是敌方单位
        
        Debug.Log("递归召唤：使用默认的乱码爬虫数据");
        return crawlerData;
    }

    /// <summary>
    /// 召唤单位
    /// </summary>
    private Unit SummonUnit(UnitDataSO unitData, GridCell targetCell, GridManager gridManager)
    {
        // 创建单位GameObject
        GameObject summonedObject;
        if (unitData.unitPrefab != null)
        {
            summonedObject = Object.Instantiate(unitData.unitPrefab);
        }
        else
        {
            // 创建基础GameObject
            summonedObject = new GameObject(unitData.unitName);
            summonedObject.AddComponent<Unit>();
        }

        // 获取Unit组件
        Unit summonedUnit = summonedObject.GetComponent<Unit>();
        if (summonedUnit == null)
        {
            summonedUnit = summonedObject.AddComponent<Unit>();
        }

        // 设置单位数据
        summonedUnit.data = unitData;
        summonedUnit.InitFromData();

        // 放置到目标位置
        summonedUnit.PlaceAt(targetCell);

        // 添加召唤标识
        RecursiveSummonTag summonTag = summonedObject.AddComponent<RecursiveSummonTag>();
        summonTag.summoner = caster;
        summonTag.summonTime = Time.time;

        return summonedUnit;
    }

    /// <summary>
    /// 播放召唤效果
    /// </summary>
    private void PlaySummonEffect(GridCell summonCell)
    {
        // TODO: 添加视觉效果
        Debug.Log($"播放递归召唤效果：在位置({summonCell.Coordinate.x}, {summonCell.Coordinate.y})");
    }

    /// <summary>
    /// 停用递归召唤
    /// </summary>
    public void DeactivateRecursiveSummon()
    {
        isActive = false;
        turnCounter = 0;
        Debug.Log("递归召唤技能已停用");
    }
}

/// <summary>
/// 递归召唤组件
/// 用于持续监听回合变化并触发召唤
/// </summary>
public class RecursiveSummonComponent : MonoBehaviour
{
    private RecursiveSummonSkill skill;

    public void Initialize(RecursiveSummonSkill recursiveSummonSkill)
    {
        skill = recursiveSummonSkill;
    }

    private void Start()
    {
        // 注册到回合管理系统（如果存在）
        // 这里需要根据实际的回合管理系统来实现
    }

    // 这个方法应该被回合管理系统调用
    public void OnTurnStart()
    {
        if (skill != null)
        {
            skill.OnTurnStart();
        }
    }

    private void OnDestroy()
    {
        // 清理事件监听
        if (skill != null)
        {
            skill.DeactivateRecursiveSummon();
        }
    }
}

/// <summary>
/// 递归召唤标签组件
/// 用于标识通过递归召唤创建的单位
/// </summary>
public class RecursiveSummonTag : MonoBehaviour
{
    [Header("召唤信息")]
    public Unit summoner;           // 召唤者
    public float summonTime;        // 召唤时间
    public bool isRecursiveSummon = true; // 是否为递归召唤的单位
}