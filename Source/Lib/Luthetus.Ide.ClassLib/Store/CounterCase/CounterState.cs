using Fluxor;

namespace Luthetus.Ide.ClassLib.Store.CounterCase;

[FeatureState]
public record CounterState(int Count)
{
    public CounterState() : this(0)
    {

    }
}