using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public enum LevelType
    {
        Tutorial,
        Normal,
        Boss
    }
    
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }
        
        [SerializeField]
        private LevelDataSO _currentLevel;
        
        private List<LevelDataSO> _levels;
        
        // private 
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            // 缓存所有关卡数据，便于索引查找
            LoadAllLevelData();
        }

        public void SetCurrentLevel(LevelDataSO levelData)
        {
            _currentLevel = levelData;
        }

        public LevelDataSO GetCurrentLevel()
        {
            return _currentLevel;
        }

        private void LoadAllLevelData()
        {
            _levels = DataManager.Instance.allLevelData;
        }

        /// <summary>
        /// 获取当前关卡在关卡列表中的 1 基索引（找不到时返回 1）。
        /// </summary>
        public int GetCurrentLevelIndex()
        {
            if (_currentLevel == null)
            {
                return 1;
            }
            if (_levels == null || _levels.Count == 0)
            {
                LoadAllLevelData();
            }
            if (_levels != null && _levels.Count > 0)
            {
                int idx = _levels.IndexOf(_currentLevel);
                if (idx >= 0)
                {
                    return idx + 1;
                }
            }
            // 兜底：尝试从字符串 ID 解析
            int parsed;
            if (int.TryParse(_currentLevel.levelId, out parsed) && parsed > 0)
            {
                return parsed;
            }
            return 1;
        }

        /// <summary>
        /// 通过 1 基索引获取关卡数据（越界返回 null）。
        /// </summary>
        public LevelDataSO GetLevelDataByIndex(int index)
        {
            if (_levels == null || _levels.Count == 0)
            {
                LoadAllLevelData();
            }
            if (index <= 0 || _levels == null || index > _levels.Count)
            {
                return null;
            }
            return _levels[index - 1];
        }
    }
}
