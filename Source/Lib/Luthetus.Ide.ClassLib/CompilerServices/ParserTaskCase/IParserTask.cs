using Fluxor;

namespace Luthetus.Ide.ClassLib.CompilerServices.ParserTaskCase;

public interface IParserTask
{
    public ParserTaskKey ParserTaskKey { get; }
    public string Name { get; }
    public string Description { get; }
    public bool ShouldNotifyWhenStartingWorkItem { get; }
    public Task? WorkProgress { get; }
    public Func<CancellationToken, Task> CancelFunc { get; }
    public IDispatcher? Dispatcher { get; }

    public Task InvokeWorkItem(CancellationToken cancellationToken);
}