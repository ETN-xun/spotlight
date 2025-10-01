# DataManager 数据管理器模块

## 概述

DataManager是聚光灯项目的核心数据管理模块，负责管理游戏中所有的配置数据，包括单位数据、技能数据和地形数据。采用单例模式设计，提供统一的数据访问接口。

## 核心组件

### 1. 枚举类型 (DataTypes.cs)

- **SkillType**: 技能类型（伤害、位移、生成）
- **TerrainType**: 地形类型（平原、山脉、森林、熔岩、深渊、泉水、磁场、爆炸桶）
- **DisplacementDirection**: 位移方向（推开、拉拢）
- **EffectType**: 效果类型（伤害、位移、生成、治疗、护盾）

### 2. ScriptableObject 数据类

#### UnitDataSO (单位数据)
```csharp
// 基础属性
public string unitID;           // 唯一标识符
public string unitName;         // 单位名称
public string description;      // 详细描述
public Sprite unitSprite;       // 单位图标
public GameObject unitPrefab;   // 单位预制体

// 战斗属性
public int maxHP;              // 最大生命值
public int moveRange;          // 移动范围
public int attackRange;        // 攻击范围
public int movementEnergyCost; // 移动能量消耗
public int baseDamage;         // 基础伤害
public int collisionDamage;    // 碰撞伤害

// 技能系统
public SkillDataSO[] skills;   // 技能列表

// AI属性
public bool isEnemy;           // 是否为敌方
public int aiPriority;         // AI优先级
```

#### SkillDataSO (技能数据)
```csharp
// 基础信息
public string skillID;         // 技能ID
public string skillName;       // 技能名称
public string description;     // 技能描述
public Sprite skillIcon;       // 技能图标

// 技能属性
public SkillType skillType;    // 技能类型
public int energyCost;         // 能量消耗
public int range;              // 攻击范围
public bool requiresTarget;    // 是否需要目标

// 伤害属性
public int baseDamage;         // 基础伤害
public int collisionDamage;    // 碰撞伤害

// 位移属性
public DisplacementDirection displacementDirection; // 位移方向
public int displacementDistance;                    // 位移距离

// 生成属性
public GameObject spawnPrefab; // 生成预制体
public int spawnDuration;      // 持续时间
public int spawnHealth;        // 生成物生命值

// 范围效果
public int aoeRadius;          // 范围半径
public bool isLinearAOE;       // 是否线性范围

// 特殊效果
public EffectType[] effects;   // 效果列表
public int cooldown;           // 冷却时间
public bool isPassive;         // 是否被动技能
```

#### TerrainDataSO (地形数据)
```csharp
// 基础信息
public string terrainID;       // 地形ID
public string terrainName;     // 地形名称
public TerrainType terrainType;// 地形类型
public string description;     // 地形描述
public Sprite terrainIcon;     // 地形图标
public GameObject terrainPrefab; // 地形预制体

// 基础属性
public bool isWalkable;        // 是否可行走
public bool isDestructible;    // 是否可摧毁
public int destructionThreshold; // 摧毁阈值
public int durability;         // 耐久度

// 移动效果
public float movementReduction; // 移动力减少
public bool blocksLineOfSight; // 阻挡视线

// 伤害效果
public int enterDamage;        // 进入伤害
public int persistentDamage;   // 持续伤害
public bool instantKill;       // 即死效果

// 位移效果
public bool hasSpringEffect;   // 弹射效果
public int springDistance;     // 弹射距离
public bool enhancesPull;      // 增强拉拢

// 爆炸效果
public bool hasExplosionEffect; // 爆炸效果
public int explosionDamage;    // 爆炸伤害
public int explosionRadius;    // 爆炸范围
public int knockbackDistance;  // 击退距离

// 特殊效果
public bool providesShield;    // 提供护盾
public bool hasChainReaction;  // 连锁反应
```

### 3. DataManager (数据管理器)

#### 主要功能

1. **数据加载**: 从Resources文件夹自动加载所有配置数据
2. **数据查询**: 提供按ID和类型查询数据的方法
3. **数据验证**: 检查数据完整性和引用有效性
4. **缓存管理**: 使用字典缓存提高查询效率

#### 核心方法

```csharp
// 单例访问
public static DataManager Instance { get; }

// 数据查询
public UnitDataSO GetUnitData(string unitID)
public SkillDataSO GetSkillData(string skillID)
public TerrainDataSO GetTerrainData(string terrainID)

// 分类查询
public List<UnitDataSO> GetPlayerUnits()
public List<UnitDataSO> GetEnemyUnits()
public List<SkillDataSO> GetSkillsByType(SkillType skillType)
public List<TerrainDataSO> GetTerrainDataByType(TerrainType terrainType)

// 数据管理
public void ReloadAllData()
public bool ValidateDataIntegrity()
```

## 使用方法

### 1. 基础查询

```csharp
// 获取单位数据
UnitDataSO playerUnit = DataManager.Instance.GetUnitData("unit_player_handi");
if (playerUnit != null)
{
    Debug.Log($"单位名称: {playerUnit.unitName}");
    Debug.Log($"生命值: {playerUnit.maxHP}");
}

// 获取技能数据
SkillDataSO skill = DataManager.Instance.GetSkillData("skill_heavy_hammer");
if (skill != null)
{
    Debug.Log($"技能: {skill.skillName}, 伤害: {skill.baseDamage}");
}

// 获取地形数据
TerrainDataSO terrain = DataManager.Instance.GetTerrainData("terrain_mountain");
if (terrain != null)
{
    Debug.Log($"地形: {terrain.terrainName}, 可行走: {terrain.isWalkable}");
}
```

### 2. 分类查询

```csharp
// 获取所有玩家单位
List<UnitDataSO> playerUnits = DataManager.Instance.GetPlayerUnits();
foreach (var unit in playerUnits)
{
    Debug.Log($"玩家单位: {unit.unitName}");
}

// 获取伤害类技能
List<SkillDataSO> damageSkills = DataManager.Instance.GetSkillsByType(SkillType.Damage);
foreach (var skill in damageSkills)
{
    Debug.Log($"伤害技能: {skill.skillName}");
}

// 获取山脉地形
List<TerrainDataSO> mountains = DataManager.Instance.GetTerrainDataByType(TerrainType.Mountain);
```

### 3. 数据验证

```csharp
// 验证数据完整性
if (DataManager.Instance.ValidateDataIntegrity())
{
    Debug.Log("数据验证通过");
}
else
{
    Debug.LogError("数据验证失败，请检查配置");
}
```

## 数据文件组织

建议在项目中创建以下文件夹结构来存放数据文件：

```
Assets/
├── Resources/
│   ├── Data/
│   │   ├── Units/          # 单位数据文件
│   │   │   ├── PlayerUnits/
│   │   │   └── EnemyUnits/
│   │   ├── Skills/         # 技能数据文件
│   │   │   ├── DamageSkills/
│   │   │   ├── DisplacementSkills/
│   │   │   └── SpawnSkills/
│   │   └── Terrains/       # 地形数据文件
│   │       ├── BasicTerrains/
│   │       ├── SpecialTerrains/
│   │       └── InteractiveTerrains/
```

## 注意事项

1. **ID唯一性**: 确保所有数据的ID在同类型中唯一
2. **引用完整性**: 技能引用的预制体和图标必须存在
3. **数据一致性**: 单位的技能列表中引用的技能必须存在
4. **性能考虑**: DataManager在游戏启动时加载所有数据，避免运行时频繁加载
5. **错误处理**: 使用前检查返回值是否为null

## 扩展建议

1. **数据热更新**: 可以添加运行时重新加载数据的功能
2. **数据编辑器**: 创建自定义编辑器窗口方便配置数据
3. **数据导入导出**: 支持从Excel或JSON文件导入数据
4. **数据版本控制**: 添加数据版本管理功能
5. **本地化支持**: 为多语言支持做准备

## 测试

使用 `DataManagerExample.cs` 脚本可以测试DataManager的各项功能：

1. 将脚本挂载到场景中的GameObject上
2. 运行游戏
3. 查看Console输出的测试结果
4. 或者在Inspector中点击"手动测试DataManager"按钮

这个模块为整个游戏系统提供了稳定可靠的数据基础，支持灵活的配置和高效的查询。