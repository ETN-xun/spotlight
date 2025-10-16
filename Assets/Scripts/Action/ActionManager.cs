using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Action
{
    public class ActionManager : MonoBehaviour
    {
        public static ActionManager Instance { get; private set; }

        public static readonly EnergySystem EnergySystem = new();
        
        [SerializeField] private int recoverEnergyPerTurn = 1;

        private Unit _actorUnit;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            MessageCenter.Subscribe(Defines.PlayerTurnStartEvent, OnPlayerTurnStarted);
            MessageCenter.Subscribe(Defines.PlayerTurnEndEvent, OnPlayerTurnEnd);
            // MessageCenter.Subscribe(Defines.EnemyTurnEndEvent, OnEnemyTurnStart);
        }
        
        private void OnDisable()
        {
            MessageCenter.Unsubscribe(Defines.PlayerTurnStartEvent, OnPlayerTurnStarted);
            MessageCenter.Unsubscribe(Defines.PlayerTurnEndEvent, OnPlayerTurnEnd);
        }

        public void ExecuteSkillAction(Unit actor, Skill skill, GridCell targetCell)
        {
            if (actor == null || targetCell == null || skill == null) return;
            if (!EnergySystem.TrySpendEnergy(skill.data.energyCost))
            {
                Debug.Log("Not enough energy to perform the skill.");
                return;
            }
            _actorUnit = actor;
            skill.Execute(targetCell, GridManager.Instance);
            
            // play animation here
            _actorUnit.currentTurnActionCount++;
            DetectActionEnd();
        }
        
        public void ExecuteMoveAction(Unit actor, GridCell targetCell)
        {
            if (actor == null || targetCell == null) return;
            if (!EnergySystem.TrySpendEnergy(_actorUnit.data.movementEnergyCost))
            {
                Debug.Log("Not enough energy to move.");
                return;
            }
            MovementSystem.Instance.MoveUnit(actor, targetCell);
            // play animation here
        }
        
        public void ExecuteEnemyWaitAction(Unit actor)
        {
            if (actor == null) return;
        }
        
        public void ExecuteEnemyAttackAction(Unit actor, GridCell targetCell)
        {
            if (actor is null || targetCell is null) return;
            // play animation here
            var targetUnit = targetCell.CurrentUnit;
            if (targetUnit is null) return;
            targetUnit.TakeDamage(actor.data.baseDamage);
            Debug.Log($"{targetUnit.data.name} 收到伤害 {actor.data.baseDamage}");
            _actorUnit = actor;
            _actorUnit.currentTurnActionCount++;
            DetectActionEnd();
        }
        
        private void DetectActionEnd()
        {
            
        }
        
        private void OnPlayerTurnStarted(object[] args)
        {
            EnergySystem.IncreaseEnergy(recoverEnergyPerTurn);
        }

        private void OnPlayerTurnEnd(object[] args)
        {
            if (_actorUnit is null || _actorUnit.data.isEnemy) return;
            if (_actorUnit.currentTurnActionCount >= _actorUnit.data.overheatedActionsPerTurn)
            {
                // _actorUnit.StatusEffectManager.AddStatusEffect(StatusAbnormalType.ThermalThrottle, 1);
                if (!_actorUnit.ttIsApplied)
                    _actorUnit.ApplyTTEffect_temp();
            }
        }
    }
}