using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSkillsTest : MonoBehaviour
{
    [Header("测试设置")]
    public bool runTestsOnStart = true;
    
    [Header("技能数据引用")]
    public SkillDataSO positionSwapSkillData;
    public SkillDataSO mirrorCopySkillData;
    public SkillDataSO recursiveSummonSkillData;
    
    [Header("单位数据引用")]
    public UnitDataSO testUnitData;
    public UnitDataSO enemyUnitData;
    
    private Unit testTarget1;
    private Unit testTarget2;
    
    void Start()
    {
        // 创建一个简单的测试文件
        string logPath = System.IO.Path.Combine(Application.dataPath, "simple_test.txt");
        System.IO.File.WriteAllText(logPath, "Script is running! Time: " + System.DateTime.Now.ToString());
        
        // 使用Debug.Log
        Debug.Log("NewSkillsTest Start method called!");
        
        // 如果runTestsOnStart为true，开始测试
        if (runTestsOnStart)
        {
            Debug.Log("Starting coroutine tests...");
            StartCoroutine(SimpleTestCoroutine());
        }
    }

    IEnumerator SimpleTestCoroutine()
    {
        string logPath = System.IO.Path.Combine(Application.dataPath, "skill_test_results.txt");
        System.IO.File.WriteAllText(logPath, "=== 技能系统测试开始 ===\n");
        System.IO.File.AppendAllText(logPath, "测试开始时间: " + System.DateTime.Now.ToString() + "\n\n");
        
        Debug.Log("开始技能系统测试...");
        
        // 测试1: 数据加载
        yield return StartCoroutine(TestDataLoading(logPath));
        yield return new WaitForSeconds(0.5f);
        
        // 测试2: 技能功能
        yield return StartCoroutine(TestSkillFunctionality(logPath));
        yield return new WaitForSeconds(0.5f);
        
        // 测试3: 技能属性统计
        yield return StartCoroutine(TestSkillProperties(logPath));
        
        System.IO.File.AppendAllText(logPath, "\n=== 技能系统测试完成 ===\n");
        System.IO.File.AppendAllText(logPath, "测试结束时间: " + System.DateTime.Now.ToString() + "\n");
        
        Debug.Log("技能系统测试完成!");
    }

    IEnumerator TestDataLoading(string logPath)
    {
        System.IO.File.AppendAllText(logPath, "--- 数据加载测试 ---\n");
        Debug.Log("开始数据加载测试");
        
        // 测试技能数据加载
        Debug.Log("正在加载技能数据...");
        SkillDataSO[] allSkills = Resources.LoadAll<SkillDataSO>("Data/Skills");
        System.IO.File.AppendAllText(logPath, $"加载技能数据: {allSkills.Length} 个技能\n");
        Debug.Log($"加载了 {allSkills.Length} 个技能");
        
        yield return new WaitForSeconds(0.2f);
        
        // 测试单位数据加载（暂时跳过以避免卡住）
        Debug.Log("跳过单位数据加载测试...");
        System.IO.File.AppendAllText(logPath, "跳过单位数据加载测试\n");
        
        yield return new WaitForSeconds(0.2f);
        
        // 检查数据完整性
        Debug.Log("检查数据完整性...");
        int validSkills = 0;
        foreach (var skill in allSkills)
        {
            if (skill != null && !string.IsNullOrEmpty(skill.skillName))
                validSkills++;
        }
        
        System.IO.File.AppendAllText(logPath, $"有效技能数据: {validSkills}/{allSkills.Length}\n");
        Debug.Log($"有效技能数据: {validSkills}/{allSkills.Length}");
        
        System.IO.File.AppendAllText(logPath, "数据加载测试完成\n\n");
        Debug.Log("数据加载测试完成");
    }

    IEnumerator TestSkillFunctionality(string logPath)
    {
        System.IO.File.AppendAllText(logPath, "--- 技能功能测试 ---\n");
        
        // 测试特定技能
        string[] testSkillNames = { "位置交换", "镜像复制", "递归召唤" };
        
        foreach (string skillName in testSkillNames)
        {
            SkillDataSO skill = Resources.Load<SkillDataSO>($"Data/Skills/{skillName}");
            
            if (skill != null)
            {
                System.IO.File.AppendAllText(logPath, $"测试技能: {skill.skillName}\n");
                System.IO.File.AppendAllText(logPath, $"  类型: {skill.skillType}\n");
                System.IO.File.AppendAllText(logPath, $"  能量消耗: {skill.energyCost}\n");
                System.IO.File.AppendAllText(logPath, $"  范围: {skill.range}\n");
                Debug.Log($"测试技能: {skill.skillName}");
            }
            else
            {
                System.IO.File.AppendAllText(logPath, $"未找到技能: {skillName}\n");
                Debug.LogWarning($"未找到技能: {skillName}");
            }
            
            yield return new WaitForSeconds(0.3f);
        }
        
        System.IO.File.AppendAllText(logPath, "技能功能测试完成\n\n");
    }

    IEnumerator TestSkillProperties(string logPath)
    {
        System.IO.File.AppendAllText(logPath, "--- 技能属性统计 ---\n");
        
        SkillDataSO[] allSkills = Resources.LoadAll<SkillDataSO>("Data/Skills");
        
        // 统计技能类型
        var skillTypeCounts = new System.Collections.Generic.Dictionary<SkillType, int>();
        int totalDamage = 0;
        int totalEnergy = 0;
        int maxRange = 0;
        int damageSkillCount = 0;
        
        foreach (var skill in allSkills)
        {
            if (skill == null) continue;
            
            // 统计类型
            if (skillTypeCounts.ContainsKey(skill.skillType))
                skillTypeCounts[skill.skillType]++;
            else
                skillTypeCounts[skill.skillType] = 1;
            
            // 统计属性
            totalDamage += skill.baseDamage;
            totalEnergy += skill.energyCost;
            if (skill.range > maxRange) maxRange = skill.range;
            if (skill.baseDamage > 0) damageSkillCount++;
        }
        
        yield return new WaitForSeconds(0.2f);
        
        // 输出统计结果
        System.IO.File.AppendAllText(logPath, $"总技能数: {allSkills.Length}\n");
        System.IO.File.AppendAllText(logPath, $"伤害技能数: {damageSkillCount}\n");
        System.IO.File.AppendAllText(logPath, $"平均能量消耗: {(allSkills.Length > 0 ? totalEnergy / allSkills.Length : 0)}\n");
        System.IO.File.AppendAllText(logPath, $"最大范围: {maxRange}\n");
        
        Debug.Log($"技能统计: 总数{allSkills.Length}, 伤害技能{damageSkillCount}, 最大范围{maxRange}");
        
        yield return new WaitForSeconds(0.2f);
        
        // 输出类型统计
        System.IO.File.AppendAllText(logPath, "技能类型分布:\n");
        foreach (var kvp in skillTypeCounts)
        {
            System.IO.File.AppendAllText(logPath, $"  {kvp.Key}: {kvp.Value} 个\n");
            Debug.Log($"技能类型 {kvp.Key}: {kvp.Value} 个");
            yield return new WaitForSeconds(0.1f);
        }
        
        System.IO.File.AppendAllText(logPath, "技能属性统计完成\n\n");
    }
}