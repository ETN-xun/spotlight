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
        
        public int initGarbledCrawlerCount;
        public int initCrashUndeadCount;
        public int initRecursiveCount;
        
        public int maxGarbledCrawlerCount;
        public int maxCrashUndeadCount;
        public int maxRecursiveCount;

        public List<Vector2Int> allyDeployPositions = new ();
        public List<Vector2Int> enemyDeployPositions = new ();
        
        public List<Vector2Int> garbledCrawlerPositions = new ();
        public List<Vector2Int> crashUndeadPositions = new ();
        public List<Vector2Int> recursivePositions = new ();
    }
}
