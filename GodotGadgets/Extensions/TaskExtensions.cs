namespace GodotGadgets.Extensions;

public static class TaskExtensions
{
    public static async void Fire(this Task task, Action? onComplete = null, Action<Exception>? onError = null)
    {
        try
        {
            try
            {
                await task;
            }
            catch (OperationCanceledException e)
            {
                GD.Print("---------- Under Control -----------");
                GD.Print($"A task was canceled, token {e.CancellationToken}");
                GD.Print("---------- Under Control -----------");
            }
            catch (Exception e)
            {
                GD.PrintErr("something wrong during fire & forget: ");
                GD.PrintErr(e);
                onError?.Invoke(e);
            }

            onComplete?.Invoke();
        }
        catch (Exception e)
        {
            GD.PrintErr("something wrong on fire & forget complete : ");
            GD.PrintErr(e);
            onError?.Invoke(e);
        }
    }
}