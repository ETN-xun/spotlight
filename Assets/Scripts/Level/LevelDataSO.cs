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
        public GridCell[,] gridCells;
        public List<Unit> playerUnits = new List<Unit>();
        public List<Unit> enemyUnits = new List<Unit>();
    }
}
