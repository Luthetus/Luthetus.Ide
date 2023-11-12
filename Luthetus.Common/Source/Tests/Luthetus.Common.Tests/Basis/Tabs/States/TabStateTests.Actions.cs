using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Tabs.States;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Tabs.States;

/// <summary>
/// <see cref="TabState"/>
/// </summary>
public class TabStateActionsTests
{
    /// <summary>
    /// <see cref="TabState.RegisterTabGroupAction"/>
    /// </summary>
    [Fact]
    public void RegisterTabGroupAction()
    {
        //var tabGroup = new TabGroup();
        //var aaa = new TabState.RegisterTabGroupAction();

        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TabState.DisposeTabGroupAction"/>
    /// </summary>
    [Fact]
    public void DisposeTabGroupAction()
    {
	    throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TabState.SetTabEntryBagAction"/>
    /// </summary>
    [Fact]
    public void SetTabEntryBagAction()
    {
	    throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TabState.SetActiveTabEntryKeyAction"/>
    /// </summary>
    [Fact]
    public void SetActiveTabEntryKeyAction()
    {
        throw new NotImplementedException();
    }

    private void InitializeTabStateActionsTests(out TabGroup sampleTabGroup)
    {
        sampleTabGroup = new TabGroup(
            loadTabEntriesArgs =>
            {
                var tabEntry = new TabEntryWithType<bool>(
                    true,
                    _ => string.Empty,
                    _ => { });

                var aaa = new TabEntryNoType[] 
                {
                   tabEntry
                }.ToImmutableList();

                return Task.FromResult(new TabGroupLoadTabEntriesOutput(aaa));
            },
            Key<TabGroup>.NewKey());
    }
}
