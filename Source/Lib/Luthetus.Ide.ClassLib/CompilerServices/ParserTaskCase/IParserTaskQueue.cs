namespace Luthetus.Ide.ClassLib.CompilerServices.ParserTaskCase;

public interface IParserTaskQueue
{
    public void QueueParserWorkItem(
        IParserTask backgroundTask);

    public Task<IParserTask?> DequeueAsync(
        CancellationToken cancellationToken);
}