using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.Tests.Basis.Contexts.Models;

/// <summary>
/// <see cref="ContextRecord"/>
/// </summary>
public class ContextRecordTests
{
    /// <summary>
    /// <see cref="ContextRecord(Key{ContextRecord}, string, string, Keymap)"/>
    /// <br/>----<br/>
    /// <see cref="ContextRecord.ContextElementId"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var key = Key<ContextRecord>.NewKey();
        var displayNameFriendly = "Header";
        var contextNameInternal = "header";
        
        var contextRecord = new ContextRecord(
            key,
            displayNameFriendly,
            contextNameInternal,
            Keymap.Empty);

        Assert.Equal(key, contextRecord.ContextKey);
        Assert.Equal(displayNameFriendly, contextRecord.DisplayNameFriendly);
        Assert.Equal(contextNameInternal, contextRecord.ContextNameInternal);
        Assert.Equal(Keymap.Empty, contextRecord.Keymap);
        Assert.Equal($"luth_ide_context-{key.Guid}", contextRecord.ContextElementId);
    }
}
