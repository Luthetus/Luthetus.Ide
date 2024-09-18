using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Ide.RazorLib.CodeSearches.States;

public partial record CodeSearchState
{
    public record WithAction(Func<CodeSearchState, CodeSearchState> WithFunc);
    public record AddResultAction(string Result);
    public record ClearResultListAction;
    public record SearchEffect(CancellationToken CancellationToken = default);
    
    public record InitializeResizeHandleDimensionUnitAction(DimensionUnit DimensionUnit);
}
