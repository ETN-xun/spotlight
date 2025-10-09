using UnityEngine;

namespace Level
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }
        
        [SerializeField]
        private LevelDataSO _currentLevelData;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public LevelDataSO GetLevelData()
        {
            return _currentLevelData;
        }
    }
}