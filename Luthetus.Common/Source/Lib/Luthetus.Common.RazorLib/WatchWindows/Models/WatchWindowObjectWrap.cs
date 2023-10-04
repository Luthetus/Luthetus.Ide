namespace Luthetus.Common.RazorLib.WatchWindows.Models;

public class WatchWindowObjectWrap
{
    /// <summary>
    /// Cannot use reflection on a null item therefore, provide the item AND the item's type.
    /// </summary>
    public WatchWindowObjectWrap(
        object? debugObjectItem,
        Type debugObjectItemType,
        string displayName,
        bool isPubliclyReadable)
    {
        DebugObjectItem = debugObjectItem;
        DebugObjectItemType = debugObjectItemType;
        DisplayName = displayName;
        IsPubliclyReadable = isPubliclyReadable;
    }

    public object? DebugObjectItem { get; set; }
    public Type DebugObjectItemType { get; set; }
    public string DisplayName { get; set; }
    public bool IsPubliclyReadable { get; set; }
}