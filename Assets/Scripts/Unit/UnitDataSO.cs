using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Game/UnitData")]
public class UnitDataSO : ScriptableObject
{
    [Header("基础")]
    public string unitName;     //单位名称
    public Sprite unitSprite; //美术表现
    public bool isEnemy;      

    [Header("属性")]
    public int maxHP;
    public int moveRange;     //移动范围
    public int attackRange;   //攻击范围
    public int energyCost;    //消耗能量

    [Header("类型")] 
    public bool canDestroy;

    /* [Header("技能")]
     public SkillDataSO[] skills; //技能列表*/
}
