using UnityEngine;

/// <summary>
/// DataManager使用示例
/// 展示如何使用数据管理器获取和使用配置数据
/// </summary>
public class DataManagerExample : MonoBehaviour
{
    [Header("测试用ID")]
    public string testUnitID = "unit_player_handi";
    public string testSkillID = "skill_heavy_hammer";
    public string testTerrainID = "terrain_mountain";

    private void Start()
    {
        // 等待DataManager初始化完成
        Invoke(nameof(TestDataManager), 0.1f);
    }

    /// <summary>
    /// 测试DataManager功能
    /// </summary>
    private void TestDataManager()
    {
        Debug.Log("=== DataManager 功能测试 ===");

        // 测试获取单位数据
        TestUnitData();
        
        // 测试获取技能数据
        TestSkillData();
        
        // 测试获取地形数据
        TestTerrainData();
        
        // 测试分类查询
        TestCategoryQueries();
    }

    /// <summary>
    /// 测试单位数据获取
    /// </summary>
    private void TestUnitData()
    {
        Debug.Log("--- 测试单位数据 ---");
        
        UnitDataSO unitData = DataManager.Instance.GetUnitData(testUnitID);
        if (unitData != null)
        {
            Debug.Log($"找到单位: {unitData.unitName}");
            Debug.Log($"生命值: {unitData.maxHP}");
            Debug.Log($"移动范围: {unitData.moveRange}");
            Debug.Log($"是否为敌方: {unitData.isEnemy}");
            
            if (unitData.skills != null && unitData.skills.Length > 0)
            {
                Debug.Log($"拥有 {unitData.skills.Length} 个技能");
                foreach (var skill in unitData.skills)
                {
                    if (skill != null)
                    {
                        Debug.Log($"  - 技能: {skill.skillName}");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning($"未找到ID为 {testUnitID} 的单位数据");
        }
    }

    /// <summary>
    /// 测试技能数据获取
    /// </summary>
    private void TestSkillData()
    {
        Debug.Log("--- 测试技能数据 ---");
        
        SkillDataSO skillData = DataManager.Instance.GetSkillData(testSkillID);
        if (skillData != null)
        {
            Debug.Log($"找到技能: {skillData.skillName}");
            Debug.Log($"技能类型: {skillData.skillType}");
            Debug.Log($"能量消耗: {skillData.energyCost}");
            Debug.Log($"攻击范围: {skillData.range}");
            Debug.Log($"基础伤害: {skillData.baseDamage}");
            
            if (skillData.displacementDistance > 0)
            {
                Debug.Log($"位移距离: {skillData.displacementDistance}");
                Debug.Log($"位移方向: {skillData.displacementDirection}");
            }
        }
        else
        {
            Debug.LogWarning($"未找到ID为 {testSkillID} 的技能数据");
        }
    }

    /// <summary>
    /// 测试地形数据获取
    /// </summary>
    private void TestTerrainData()
    {
        Debug.Log("--- 测试地形数据 ---");
        
        TerrainDataSO terrainData = DataManager.Instance.GetTerrainData(testTerrainID);
        if (terrainData != null)
        {
            Debug.Log($"找到地形: {terrainData.terrainName}");
            Debug.Log($"地形类型: {terrainData.terrainType}");
            Debug.Log($"是否可行走: {terrainData.isWalkable}");
            Debug.Log($"是否可摧毁: {terrainData.isDestructible}");
            
            if (terrainData.enterDamage > 0)
            {
                Debug.Log($"进入伤害: {terrainData.enterDamage}");
            }
            
            if (terrainData.hasSpringEffect)
            {
                Debug.Log($"弹射距离: {terrainData.springDistance}");
            }
            
            if (terrainData.hasExplosionEffect)
            {
                Debug.Log($"爆炸伤害: {terrainData.explosionDamage}");
                Debug.Log($"爆炸范围: {terrainData.explosionRadius}");
            }
        }
        else
        {
            Debug.LogWarning($"未找到ID为 {testTerrainID} 的地形数据");
        }
    }

    /// <summary>
    /// 测试分类查询功能
    /// </summary>
    private void TestCategoryQueries()
    {
        Debug.Log("--- 测试分类查询 ---");
        
        // 获取所有玩家单位
        var playerUnits = DataManager.Instance.GetPlayerUnits();
        Debug.Log($"玩家单位数量: {playerUnits.Count}");
        
        // 获取所有敌方单位
        var enemyUnits = DataManager.Instance.GetEnemyUnits();
        Debug.Log($"敌方单位数量: {enemyUnits.Count}");
        
        // 获取伤害类技能
        var damageSkills = DataManager.Instance.GetSkillsByType(SkillType.Damage);
        Debug.Log($"伤害类技能数量: {damageSkills.Count}");
        
        // 获取位移类技能
        var displacementSkills = DataManager.Instance.GetSkillsByType(SkillType.Displacement);
        Debug.Log($"位移类技能数量: {displacementSkills.Count}");
        
        // 获取山脉地形
        var mountainTerrains = DataManager.Instance.GetTerrainDataByType(TerrainType.Mountain);
        Debug.Log($"山脉地形数量: {mountainTerrains.Count}");
    }

    /// <summary>
    /// 在编辑器中手动测试
    /// </summary>
    [ContextMenu("手动测试DataManager")]
    public void ManualTest()
    {
        if (Application.isPlaying)
        {
            TestDataManager();
        }
        else
        {
            Debug.LogWarning("请在运行时测试DataManager");
        }
    }
}