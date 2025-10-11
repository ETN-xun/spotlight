using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Level;

/// <summary>
/// 数据管理器
/// 负责加载和管理所有ScriptableObject配置数据
/// 提供按ID查找数据的接口
/// </summary>
public class DataManager : MonoBehaviour
{
    #region 单例模式
    private static DataManager _instance;
    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DataManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("DataManager");
                    _instance = go.AddComponent<DataManager>();
                    // DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    #endregion

    [Header("数据路径配置")]
    [Tooltip("单位数据资源路径")]
    public string unitDataPath = "Data/Units";
    
    [Tooltip("技能数据资源路径")]
    public string skillDataPath = "Data/Skills";
    
    [Tooltip("地形数据资源路径")]
    public string terrainDataPath = "Data/Terrains";
    
    [Tooltip("关卡数据资源路径")]
    public string levelDataPath = "Data/Levels";

    [Header("数据集合")]
    [Tooltip("所有单位数据")]
    public List<UnitDataSO> allUnitData = new List<UnitDataSO>();
    
    [Tooltip("所有技能数据")]
    public List<SkillDataSO> allSkillData = new List<SkillDataSO>();
    
    [Tooltip("所有地形数据")]
    public List<TerrainDataSO> allTerrainData = new List<TerrainDataSO>();
    
    [Tooltip("所有关卡数据")]
    public List<LevelDataSO> allLevelData = new List<LevelDataSO>();

    // 数据字典，用于快速查找
    private Dictionary<string, UnitDataSO> unitDataDict = new Dictionary<string, UnitDataSO>();
    private Dictionary<string, SkillDataSO> skillDataDict = new Dictionary<string, SkillDataSO>();
    private Dictionary<string, TerrainDataSO> terrainDataDict = new Dictionary<string, TerrainDataSO>();
    private Dictionary<string, LevelDataSO> levelDataDict = new Dictionary<string, LevelDataSO>();

    #region Unity生命周期
    private void Awake()
    {
        // 确保单例
        if (_instance == null)
        {
            _instance = this;
            // DontDestroyOnLoad(gameObject);
            InitializeData();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region 数据初始化
    /// <summary>
    /// 初始化所有数据
    /// </summary>
    public void InitializeData()
    {
        LoadAllData();
        BuildDataDictionaries();
        ValidateData();
    }

    /// <summary>
    /// 从Resources文件夹加载所有数据
    /// </summary>
    private void LoadAllData()
    {
        // 加载单位数据
        UnitDataSO[] unitDataArray = Resources.LoadAll<UnitDataSO>(unitDataPath);
        allUnitData = new List<UnitDataSO>(unitDataArray);
        Debug.Log($"加载了 {allUnitData.Count} 个单位数据");

        // 加载技能数据
        SkillDataSO[] skillDataArray = Resources.LoadAll<SkillDataSO>(skillDataPath);
        allSkillData = new List<SkillDataSO>(skillDataArray);
        Debug.Log($"加载了 {allSkillData.Count} 个技能数据");

        // 加载地形数据
        TerrainDataSO[] terrainDataArray = Resources.LoadAll<TerrainDataSO>(terrainDataPath);
        allTerrainData = new List<TerrainDataSO>(terrainDataArray);
        Debug.Log($"加载了 {allTerrainData.Count} 个地形数据");
        
        // 加载关卡数据
        LevelDataSO[] levelDataArray = Resources.LoadAll<LevelDataSO>(levelDataPath);
        allLevelData = new List<LevelDataSO>(levelDataArray);
    }

    /// <summary>
    /// 构建数据字典用于快速查找
    /// </summary>
    private void BuildDataDictionaries()
    {
        // 构建单位数据字典
        unitDataDict.Clear();
        foreach (var unitData in allUnitData)
        {
            if (!string.IsNullOrEmpty(unitData.unitID))
            {
                if (!unitDataDict.ContainsKey(unitData.unitID))
                {
                    unitDataDict.Add(unitData.unitID, unitData);
                }
                else
                {
                    Debug.LogWarning($"重复的单位ID: {unitData.unitID}");
                }
            }
        }

        // 构建技能数据字典
        skillDataDict.Clear();
        foreach (var skillData in allSkillData)
        {
            if (!string.IsNullOrEmpty(skillData.skillID))
            {
                if (!skillDataDict.ContainsKey(skillData.skillID))
                {
                    skillDataDict.Add(skillData.skillID, skillData);
                }
                else
                {
                    Debug.LogWarning($"重复的技能ID: {skillData.skillID}");
                }
            }
        }

        // 构建地形数据字典
        terrainDataDict.Clear();
        foreach (var terrainData in allTerrainData)
        {
            if (!string.IsNullOrEmpty(terrainData.terrainID))
            {
                if (!terrainDataDict.ContainsKey(terrainData.terrainID))
                {
                    terrainDataDict.Add(terrainData.terrainID, terrainData);
                }
                else
                {
                    Debug.LogWarning($"重复的地形ID: {terrainData.terrainID}");
                }
            }
        }

        // 构建关卡数据字典
        levelDataDict.Clear();
        foreach (var levelData in allLevelData)
        {
            if (!string.IsNullOrEmpty(levelData.levelId))
            {
                if (!levelDataDict.ContainsKey(levelData.levelId))
                {
                    levelDataDict.Add(levelData.levelId, levelData);
                }
                else
                {
                    Debug.LogWarning($"重复的关卡ID: {levelData.levelId}");
                }
            }
        }
    }

    /// <summary>
    /// 验证数据完整性
    /// </summary>
    private void ValidateData()
    {
        // 验证单位数据
        foreach (var unitData in allUnitData)
        {
            if (string.IsNullOrEmpty(unitData.unitID))
            {
                Debug.LogError($"单位数据缺少ID: {unitData.name}");
            }
            
            // 验证技能引用
            if (unitData.skills != null)
            {
                foreach (var skill in unitData.skills)
                {
                    if (skill == null)
                    {
                        Debug.LogWarning($"单位 {unitData.unitName} 包含空的技能引用");
                    }
                }
            }
        }

        // 验证技能数据
        foreach (var skillData in allSkillData)
        {
            if (string.IsNullOrEmpty(skillData.skillID))
            {
                Debug.LogError($"技能数据缺少ID: {skillData.name}");
            }
        }

        // 验证地形数据
        foreach (var terrainData in allTerrainData)
        {
            if (string.IsNullOrEmpty(terrainData.terrainID))
            {
                Debug.LogError($"地形数据缺少ID: {terrainData.name}");
            }
        }

        foreach (var levelData in allLevelData)
        {
            if (string.IsNullOrEmpty(levelData.levelId))
            {
                Debug.LogError($"关卡数据缺少ID: {levelData.name}");
            }
        }
    }
    #endregion

    #region 数据查询接口
    /// <summary>
    /// 根据ID获取单位数据
    /// </summary>
    /// <param name="unitID">单位ID</param>
    /// <returns>单位数据，如果未找到返回null</returns>
    public UnitDataSO GetUnitData(string unitID)
    {
        if (string.IsNullOrEmpty(unitID))
        {
            Debug.LogWarning("尝试获取单位数据时传入了空的ID");
            return null;
        }

        unitDataDict.TryGetValue(unitID, out UnitDataSO unitData);
        if (unitData == null)
        {
            Debug.LogWarning($"未找到ID为 {unitID} 的单位数据");
        }
        return unitData;
    }

    /// <summary>
    /// 根据ID获取技能数据
    /// </summary>
    /// <param name="skillID">技能ID</param>
    /// <returns>技能数据，如果未找到返回null</returns>
    public SkillDataSO GetSkillData(string skillID)
    {
        if (string.IsNullOrEmpty(skillID))
        {
            Debug.LogWarning("尝试获取技能数据时传入了空的ID");
            return null;
        }

        skillDataDict.TryGetValue(skillID, out SkillDataSO skillData);
        if (skillData == null)
        {
            Debug.LogWarning($"未找到ID为 {skillID} 的技能数据");
        }
        return skillData;
    }

    /// <summary>
    /// 根据ID获取地形数据
    /// </summary>
    /// <param name="terrainID">地形ID</param>
    /// <returns>地形数据，如果未找到返回null</returns>
    public TerrainDataSO GetTerrainData(string terrainID)
    {
        if (string.IsNullOrEmpty(terrainID))
        {
            Debug.LogWarning("尝试获取地形数据时传入了空的ID");
            return null;
        }

        terrainDataDict.TryGetValue(terrainID, out TerrainDataSO terrainData);
        if (terrainData == null)
        {
            Debug.LogWarning($"未找到ID为 {terrainID} 的地形数据");
        }
        return terrainData;
    }

    /// <summary>
    /// 根据地形类型获取地形数据
    /// </summary>
    /// <param name="terrainType">地形类型</param>
    /// <returns>匹配的地形数据列表</returns>
    public List<TerrainDataSO> GetTerrainDataByType(TerrainType terrainType)
    {
        return allTerrainData.Where(t => t.terrainType == terrainType).ToList();
    }

    /// <summary>
    /// 获取所有玩家单位数据
    /// </summary>
    /// <returns>玩家单位数据列表</returns>
    public List<UnitDataSO> GetPlayerUnits()
    {
        return allUnitData.Where(u => !u.isEnemy).ToList();
    }

    /// <summary>
    /// 获取所有敌方单位数据
    /// </summary>
    /// <returns>敌方单位数据列表</returns>
    public List<UnitDataSO> GetEnemyUnits()
    {
        return allUnitData.Where(u => u.isEnemy).ToList();
    }

    /// <summary>
    /// 根据技能类型获取技能数据
    /// </summary>
    /// <param name="skillType">技能类型</param>
    /// <returns>匹配的技能数据列表</returns>
    public List<SkillDataSO> GetSkillsByType(SkillType skillType)
    {
        return allSkillData.Where(s => s.skillType == skillType).ToList();
    }
    #endregion

    public LevelDataSO GetLevelData(string levelId)
    {
        if (string.IsNullOrEmpty(levelId))
        {
            Debug.LogWarning("尝试获取地形数据时传入了空的ID");
            return null;
        }

        levelDataDict.TryGetValue(levelId, out LevelDataSO levelDataSo);
        if (levelDataSo == null)
        {
            Debug.LogWarning($"未找到ID为 {levelId} 的地形数据");
        }
        return levelDataSo;
    }
    

    #region 数据重载
    /// <summary>
    /// 重新加载所有数据
    /// </summary>
    [ContextMenu("重新加载数据")]
    public void ReloadAllData()
    {
        InitializeData();
        Debug.Log("数据重新加载完成");
    }
    #endregion

    #region 调试信息
    /// <summary>
    /// 打印数据统计信息
    /// </summary>
    [ContextMenu("打印数据统计")]
    public void PrintDataStatistics()
    {
        Debug.Log($"=== 数据统计 ===");
        Debug.Log($"单位数据: {allUnitData.Count} 个");
        Debug.Log($"技能数据: {allSkillData.Count} 个");
        Debug.Log($"地形数据: {allTerrainData.Count} 个");
        Debug.Log($"玩家单位: {GetPlayerUnits().Count} 个");
        Debug.Log($"敌方单位: {GetEnemyUnits().Count} 个");
    }
    #endregion
}