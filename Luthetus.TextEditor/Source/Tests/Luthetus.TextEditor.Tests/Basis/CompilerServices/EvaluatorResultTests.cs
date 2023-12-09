namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class EvaluatorResultTests
{
    public EvaluatorResult(Type type, object result)
    {
        Type = type;
        Result = result;
    }

    public Type Type { get; }
    public object Result { get; }
}