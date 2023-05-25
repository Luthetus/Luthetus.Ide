namespace Luthetus.Ide.ClassLib.CompilerServices.Common.EvaluatorCase;

public class EvaluatorResult
{
    public EvaluatorResult(
        Type type,
        object result)
    {
        Type = type;
        Result = result;
    }

    public Type Type { get; }
    public object Result { get; }
}
