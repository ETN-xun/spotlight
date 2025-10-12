using Common;
using UnityEngine;

namespace System
{
    public class EnergySystem : MonoBehaviour
    {
        private int _maxEnergy;
        private int _baseEnergy;
        private int _currentEnergy;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _maxEnergy = Level.LevelManager.Instance.GetCurrentLevel().maxEnergy;
            _baseEnergy = Level.LevelManager.Instance.GetCurrentLevel().baseEnergy;
            _currentEnergy = _baseEnergy;
        }
        
        public int GetCurrentEnergy()
        {
            return _currentEnergy;
        }
        
        public void IncreaseEnergy(int amount)
        {
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
            if (_currentEnergy < amount) return false;
            DecreaseEnergy(amount);
            return true;
        }
    }
}