using Fluxor;
using Luthetus.Ide.ClassLib.State;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.Store.TerminalCase;

/// <param name="EmptyTextHack">(2023-06-09) I added this property because I did a refactor to remove unused properties and then an injection of this state got removed. Fluxor however was using the injection due to fluxor component inheritance. So the code broke. I'm referencing this empty string so Visual Studio sees me using the property.</param>
[FeatureState]
public record TerminalSessionWasModifiedRegistry(ImmutableDictionary<TerminalSessionKey, StateKey> TerminalSessionWasModifiedMap, string EmptyTextHack)
{
    public TerminalSessionWasModifiedRegistry()
        : this(ImmutableDictionary<TerminalSessionKey, StateKey>.Empty, string.Empty)
    {
    }
}