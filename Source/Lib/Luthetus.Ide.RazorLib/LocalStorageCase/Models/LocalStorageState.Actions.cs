namespace Luthetus.Ide.RazorLib.LocalStorageCase.Models;

public partial class LocalStorageState
{
    public record LocalStorageSetItemTask(LocalStorageSync Sync, string Key, string Value);
}
