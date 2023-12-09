namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

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