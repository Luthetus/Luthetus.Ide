using Luthetus.Common.RazorLib.Commands.Models;

namespace Luthetus.Common.RazorLib.Keymaps.Models;

/// <summary>
/// The idea behind this class is entirely based on the 'event.key' vs 'event.code' for a given keyboard event.
/// It is desired to use the 'event.key' sometimes, whereas other times the 'event.code'.
/// So, these corresponding properties on this type were made nullable.
/// It still is incredibly clumsy because the goal is that 1 of them isn't null, yet both will forever be "nullable".
/// 
/// Most of this type is the copy and pasted source code of 'KeyboardEventArgs.cs'
/// https://github.com/dotnet/aspnetcore/blob/3f1acb59718cadf111a0a796681e3d3509bb3381/src/Components/Web/src/Web/KeyboardEventArgs.cs
/// 
/// The difference being that the copy and pasted code was changed to be nullable and immutable,
/// and some extra code was added.
/// </summary>
public readonly struct Keybind
{
    public Keybind(IKeymapArgs keymapArgs, CommandNoType commandNoType)
    {
        KeymapArgs = keymapArgs;
        CommandNoType = commandNoType;
    }

    public IKeymapArgs KeymapArgs { get; }
    public CommandNoType CommandNoType { get; }
}
