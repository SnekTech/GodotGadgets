namespace GodotGadgets.Extensions;

public static class TaskExtensions
{
    public static void Fire(this Task task, Action? onComplete = null, Action<Exception>? onError = null)
    {
        task.Fire(onComplete, onError, PrinterGD.Instance);
    }

    private static async void Fire(this Task task, Action? onComplete, Action<Exception>? onError,
        ITaskFireStatusPrinter printer)
    {
        try
        {
            try
            {
                await task;
            }
            catch (OperationCanceledException)
            {
                printer.Print("---------- Under Control -----------");
                printer.Print("A task was canceled:");
                printer.Print("---------- Under Control -----------");
            }
            catch (Exception e)
            {
                printer.PrintErr("something wrong during fire & forget: ");
                printer.PrintErr(e);
                onError?.Invoke(e);
            }

            onComplete?.Invoke();
        }
        catch (Exception e)
        {
            printer.PrintErr("something wrong on fire & forget complete : ");
            printer.PrintErr(e);
            onError?.Invoke(e);
        }
    }
}

public interface ITaskFireStatusPrinter
{
    void Print(string what);
    void PrintErr(params object[] what);
}

public class PrinterGD : ITaskFireStatusPrinter
{
    private PrinterGD()
    {
    }

    public static readonly PrinterGD Instance = new();

    public void Print(string what) => GD.Print(what);

    public void PrintErr(params object[] what)
    {
        GD.PushError(what);
    }
}