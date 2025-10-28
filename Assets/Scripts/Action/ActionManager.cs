using System;
using System.Collections;
using System.Collections.Generic;
using Ally;
using Common;
using UnityEngine;

namespace Action
{
    public class ActionManager : MonoBehaviour
    {
        public static ActionManager Instance { get; private set; }

        public static readonly EnergySystem EnergySystem = new();
        
        [SerializeField] private int recoverEnergyPerTurn = 5;

        private Unit _actorUnit;
        private readonly List<Unit> _overheatedUnits = new();
        private readonly List<Unit> _pendingRemoveOverheatedUnits = new();

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            MessageCenter.Subscribe(Defines.PlayerTurnStartEvent, OnPlayerTurnStarted);
            MessageCenter.Subscribe(Defines.PlayerTurnEndEvent, OnPlayerTurnEnd);
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
            Debug.Log("execute skill action");
            // play animation here
            DetectActionEnd();
        }
        
        public void ExecuteMoveAction(Unit actor, GridCell targetCell)
        {
            if (actor == null || targetCell == null) return;
            _actorUnit = actor;
            if (!EnergySystem.TrySpendEnergy(_actorUnit.data.movementEnergyCost))
            {
                Debug.Log("Not enough energy to move.");
                return;
            }
            MovementSystem.Instance.TryMoveToCell(actor, targetCell);
            DetectActionEnd();
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
            _actorUnit.currentTurnActionCount++;
            if (_actorUnit.currentTurnActionCount >= _actorUnit.data.overheatedActionsPerTurn)
            {
                if (!_actorUnit.ttIsApplied)
                {
                    _overheatedUnits.Add(_actorUnit);
                }
            }
        }
        
        private void OnPlayerTurnStarted(object[] args)
        {
            if (AllyManager.Instance.CurrentPlayerTurn == 1) return;
            EnergySystem.IncreaseEnergy(recoverEnergyPerTurn);
        }

        private void OnPlayerTurnEnd(object[] args)
        {
            foreach (var unit in _pendingRemoveOverheatedUnits)
            {
                unit.CancelTTEffect_temp();
            }
            _pendingRemoveOverheatedUnits.Clear();
            
            foreach (var unit in _overheatedUnits)
            {
                unit.ApplyTTEffect_temp();
                _pendingRemoveOverheatedUnits.Add(unit);
            }
            _overheatedUnits.Clear();
        }
    }
}