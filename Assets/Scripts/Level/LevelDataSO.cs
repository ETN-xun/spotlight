using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelDataSO", order = 1)]
    public class LevelDataSO : ScriptableObject
    {
        [Tooltip("关卡唯一标识符")]
        public string levelId;
        public Sprite previewSprite;
        public string levelTarget;
        [Tooltip("最大能量值")]
        public int maxEnergy = 10;
        [Tooltip("基础能量值")]
        public int baseEnergy = 6;
        [Tooltip("每回合增加的能量值")] public int energyPerTurn = 4;
        public List<Unit> allyUnits = new ();

        public int addedEnemyPerTurnCount;
        
        public int 初始乱码爬虫数量;
        public int 初始死机亡灵数量;
        public int 初始空指针数量;
        
        public int 最大乱码爬虫数量;
        public int 最大死机亡灵数量;
        public int 最大空指针数量;

        public List<Vector2Int> allyDeployPositions = new ();
        public List<Vector2Int> enemyDeployPositions = new ();
        
        public List<Vector2Int> 乱码爬虫位置 = new ();
        public List<Vector2Int> 死机亡灵位置 = new ();
        public List<Vector2Int> 空指针位置 = new ();
    }
}
