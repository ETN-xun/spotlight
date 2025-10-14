public class InputStateMachine
{
    public InputState CurrentState { get; private set; }
    private BaseInputState _currentStateInstance;
    private readonly BaseInputState _idleStateInstance;
    private readonly BaseInputState _selectPlayerUnitStateInstance;
    private readonly BaseInputState _selectEnemyUnitStateInstance;
    private readonly BaseInputState _selectNoUnitStateInstance;
    
    
    public InputStateMachine()
    {
        _idleStateInstance = new IdleState(this);
        _selectEnemyUnitStateInstance = new SelectEnemyUnitState(this);
        _selectPlayerUnitStateInstance = new SelectPlayerUnitState(this);
        _selectNoUnitStateInstance = new SelectNoUnitState(this);
        
        CurrentState = InputState.IdleState;
        _currentStateInstance = _idleStateInstance;
    }
    
    public void ChangeState(InputState newState)
    {
        _currentStateInstance?.Exit();
        CurrentState = newState;
        _currentStateInstance = GetStateInstance(CurrentState);
        _currentStateInstance?.Enter();
    }
        
    public void Update()
    {
        _currentStateInstance?.Update();
    }
    
    private BaseInputState GetStateInstance(InputState state)
    {
        return state switch
        {
            InputState.IdleState => _idleStateInstance,
            InputState.SelectPlayerUnitState => _selectPlayerUnitStateInstance,
            InputState.SelectEnemyUnitState => _selectEnemyUnitStateInstance,
            InputState.SelectNoUnitState => _selectNoUnitStateInstance,
            _ => null
        };
    }
}