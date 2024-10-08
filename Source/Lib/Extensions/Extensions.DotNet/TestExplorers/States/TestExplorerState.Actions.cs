using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.States;

public partial record TestExplorerState
{
    public record WithAction(Func<TestExplorerState, TestExplorerState> WithFunc);

    /// <summary>
    /// When the user interface for the test explorer is rendered,
    /// then dispatch this in order to start a task that will discover unit tests.
    /// </summary>
    public record UserInterfaceWasInitializedEffect;

    public record ShouldInitializeEffect;
    
    public record InitializeResizeHandleDimensionUnitAction(DimensionUnit DimensionUnit);
}