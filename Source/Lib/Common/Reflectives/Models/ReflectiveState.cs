namespace Luthetus.Common.RazorLib.Reflectives.Models;

public record struct ReflectiveState
{
	public ReflectiveState()
	{
	}

    public IReadOnlyList<ReflectiveModel> ReflectiveModelList { get; init; } = Array.Empty<ReflectiveModel>();
}