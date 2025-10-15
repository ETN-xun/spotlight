using UnityEngine;

public abstract class BaseInputState
{
    protected InputStateMachine stateMachine;
    protected readonly Camera mainCamera = Camera.main;
    protected Unit selectedUnit;

    protected BaseInputState(InputStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    
    
}