using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelDataSO", order = 1)]
    public class LevelDataSO : ScriptableObject
    {
        [Tooltip("关卡唯一标识符，按示例来")]
        public string levelId;
        public LevelType levelType;
        public Sprite previewSprite;
        public string description;
        public int maxTurns;
        public int startingActionPoints;
        public int gridWidth;
        public int gridHeight;
        [Tooltip("最大能量值")]
        public int maxEnergy = 10;
        [Tooltip("基础能量值")]
        public int baseEnergy = 6;
        public List<Unit> allyUnits = new ();
        public List<Unit> enemyUnits = new ();
    }
}
