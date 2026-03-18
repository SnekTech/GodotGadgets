namespace GodotGadgets.FSM;

public interface IState
{
    Task OnEnterAsync(CancellationToken ct);
    Task OnExitAsync(CancellationToken ct);
}