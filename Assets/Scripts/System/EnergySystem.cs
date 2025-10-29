using Common;
using UnityEngine;

namespace System
{
    public class EnergySystem 
    {
        private int _maxEnergy;
        private int _baseEnergy;
        private int _currentEnergy;

        public EnergySystem()
        {
            // 不在构造函数中初始化，延迟到需要时初始化
        }

        private void Init()
        {
            if (Level.LevelManager.Instance == null)
            {
                Debug.LogError("LevelManager.Instance is null when initializing EnergySystem");
                return;
            }
            
            var currentLevel = Level.LevelManager.Instance.GetCurrentLevel();
            if (currentLevel == null)
            {
                Debug.LogError("Current level is null when initializing EnergySystem");
                return;
            }
            
            _maxEnergy = currentLevel.maxEnergy;
            _baseEnergy = currentLevel.baseEnergy;
            _currentEnergy = _baseEnergy;
        }
        
        private bool _isInitialized = false;
        
        private void EnsureInitialized()
        {
            if (!_isInitialized && Level.LevelManager.Instance != null && Level.LevelManager.Instance.GetCurrentLevel() != null)
            {
                Init();
                _isInitialized = true;
            }
        }
        
        public int GetCurrentEnergy()
        {
            EnsureInitialized();
            return _currentEnergy;
        }
        
        public void IncreaseEnergy(int amount)
        {
            EnsureInitialized();
            if (_currentEnergy >= _maxEnergy) return;
            _currentEnergy += amount;
            if (_currentEnergy > _maxEnergy)
            {
                _currentEnergy = _maxEnergy;
            }
            MessageCenter.Publish(Defines.EnergyChangedEvent, _currentEnergy);
        }

        public void DecreaseEnergy(int amount)
        {
            EnsureInitialized();
            if (_currentEnergy <= 0) return;
            _currentEnergy -= amount;
            if (_currentEnergy < 0)
            {
                _currentEnergy = 0;
            }
            MessageCenter.Publish(Defines.EnergyChangedEvent, _currentEnergy);
        }
        
        public bool TrySpendEnergy(int amount)
        {
            EnsureInitialized();
            if (_currentEnergy < amount) return false;
            DecreaseEnergy(amount);
            return true;
        }
    }
}