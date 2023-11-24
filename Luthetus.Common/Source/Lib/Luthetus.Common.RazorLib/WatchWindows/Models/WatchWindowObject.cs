namespace Luthetus.Common.RazorLib.WatchWindows.Models;

public class WatchWindowObject
{
    /// <summary>
    /// Cannot use reflection on a null item therefore, provide the item AND the item's type.
    /// </summary>
    public WatchWindowObject(
        object? item,
        Type itemType,
        string displayName,
        bool isPubliclyReadable)
    {
        Item = item;
        ItemType = itemType;
        DisplayName = displayName;
        IsPubliclyReadable = isPubliclyReadable;
    }

    public object? Item { get; set; }
    public Type ItemType { get; set; }
    public string DisplayName { get; set; }
    public bool IsPubliclyReadable { get; set; }
}