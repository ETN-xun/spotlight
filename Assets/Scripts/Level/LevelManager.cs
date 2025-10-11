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
    }
}