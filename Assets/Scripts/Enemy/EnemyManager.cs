using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        public static  EnemyManager Instance { get; private set; }

        private List<Unit> _enemies = new ();
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SpawnEnemies()
        {
            var currentLevelData = Level.LevelManager.Instance.GetCurrentLevel();
            var aliveEnemies = currentLevelData.enemyUnits;
            // for (var i = 0; i < aliveEnemies.Count; i++)
            // {
            //     GridManager.Instance.PlaceUnit(new Vector2Int(i, i), aliveEnemies[i]);
            // }
            GridManager.Instance.PlaceUnit(new Vector2Int(3, 3), aliveEnemies[0]);
            GridManager.Instance.PlaceUnit(new Vector2Int(-3, 3), aliveEnemies[1]);
        }
    }
}