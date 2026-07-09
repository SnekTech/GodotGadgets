namespace GodotGadgets.Tasks;

public static class TaskFireAndForgetExtensions
{
    extension(Task task)
    {
        public void Fire(Action<Exception>? onError = null)
            => task.Fire(onError, onComplete: null, onCompleteError: null, PrinterGD.Instance);

        async void Fire(Action<Exception>? onError, Action? onComplete, Action<Exception>? onCompleteError,
            ITaskFireStatusPrinter printer)
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
                return;
            }
            catch (Exception e)
            {
                printer.PrintErr("something wrong during fire & forget: ");
                printer.PrintErr(e);
                onError?.Invoke(e);
                return;
            }

            try
            {
                onComplete?.Invoke();
            }
            catch (Exception e)
            {
                printer.PrintErr("onComplete callback threw: ", e);
                onCompleteError?.Invoke(e);
            }
        }
    }
}

public interface ITaskFireStatusPrinter
{
    void Print(string what);
    void PrintErr(params object[] what);
}

public sealed class PrinterGD : ITaskFireStatusPrinter
{
    PrinterGD()
    {
    }

    public static readonly PrinterGD Instance = new();

    public void Print(string what) => GD.Print(what);

    public void PrintErr(params object[] what)
    {
        GD.PushError(what);
    }
}