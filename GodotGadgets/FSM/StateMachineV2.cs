namespace GodotGadgets.FSM;

public abstract class StateMachineV2<TState>
    where TState : class, IState
{
    protected TState? CurrentState { get; private set; }
    bool HasInitialized => CurrentState is not null;
    bool _isTransitioning;

    public async Task SetInitStateAsync(TState initState, CancellationToken ct = default)
    {
        if (HasInitialized) return;

        _isTransitioning = true;
        CurrentState = initState;
        await CurrentState.OnEnterAsync(ct);
        _isTransitioning = false;
    }

    public async Task ChangeStateAsync(TState newState, CancellationToken ct = default)
    {
        if (CurrentState == null)
            throw new InvalidOperationException("cannot change state from a null state");
        if (_isTransitioning)
            return;

        if (newState.GetType() == CurrentState.GetType())
            return;

        _isTransitioning = true;
        await CurrentState.OnExitAsync(ct);
        CurrentState = newState;
        await CurrentState.OnEnterAsync(ct);
        _isTransitioning = false;
    }
}