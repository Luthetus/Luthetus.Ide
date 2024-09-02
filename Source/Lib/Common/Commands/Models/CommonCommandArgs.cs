using Luthetus.Common.RazorLib.Keymaps.Models;

namespace Luthetus.Common.RazorLib.Commands.Models;

/// <summary>
/// Any command args ought to be a struct? High turnover
/// </summary>
public struct CommonCommandArgs : ICommandArgs
{
    public CommonCommandArgs(KeymapArgs keymapArgs)
    {
        KeymapArgs = keymapArgs;
    }

    /// <summary>
    /// When the command is fired via a keyboard event,
    /// there is no mapping done for <see cref="KeymapArgs.Location"/>,
    /// and other miscelaneous information.
    /// 
    /// So, the <see cref="KeymapArgs"/> is provided here so one can check the
    /// miscelaneous information.
    /// </summary>
    public KeymapArgs KeymapArgs { get; }
}
