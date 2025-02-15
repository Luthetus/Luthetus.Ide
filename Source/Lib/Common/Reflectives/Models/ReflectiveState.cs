namespace Luthetus.Common.RazorLib.Reflectives.Models;

public record struct ReflectiveState
{
    public ReflectiveState()
    {
        ReflectiveModelList = new();
    }

    public List<ReflectiveModel> ReflectiveModelList { get; init; }
}