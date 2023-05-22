namespace Luthetus.Ide.ClassLib.Dto;

public class InternalResponse<T> : InternalResponseVoid
{
    public InternalResponse(
        string itemJson,
        string message)
        : base(message)
    {
        ItemJson = itemJson;
    }

    public string ItemJson { get; }

    public Type ItemType => typeof(T);
}