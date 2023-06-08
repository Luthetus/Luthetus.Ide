namespace Luthetus.Ide.ClassLib.CompilerServices.ParserTaskCase;

public interface IParserTaskMonitor
{
    public IParserTask? ExecutingParserTask { get; }

    public event Action? ExecutingParserTaskChanged;

    public void SetExecutingParserTask(IParserTask? parserTask);
}