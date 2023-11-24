using Luthetus.Common.RazorLib.Options.States;

namespace Luthetus.Common.Tests.Basis.Options.States;

/// <summary>
/// <see cref="AppOptionsState"/>
/// </summary>
public class AppOptionsStateActionTests
{
    /// <summary>
    /// <see cref="AppOptionsState.WithAction"/>
    /// </summary>
    [Fact]
    public void WithAction()
    {
        var withFunc = new Func<AppOptionsState, AppOptionsState>(
            inAppOptionsState => inAppOptionsState);

        var withAction = new AppOptionsState.WithAction(withFunc);

        Assert.Equal(withFunc, withAction.WithFunc);
    }
}