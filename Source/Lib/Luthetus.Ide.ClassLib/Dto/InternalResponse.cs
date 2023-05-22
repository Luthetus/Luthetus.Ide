namespace Luthetus.Ide.ClassLib.Dto;

public class InternalResponse<T> : InternalResponseVoid
{
    public InternalResponse(
        string itemJson,
        string message)
        : base(message)
    {
        ItemJson = itemJson;
        Message = message;
    }

    public string ItemJson { get; }
    public string Message { get; }

    public Type ItemType => typeof(T);
}