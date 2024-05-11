using Luthetus.Common.RazorLib.Reactives.Models.Internals;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Reactives.Displays;

public partial class CounterThrottleAsyncDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public CTA_NoConfigureAwait CounterThrottleAsync { get; set; } = new(TimeSpan.FromMilliseconds(2_000));

    private int _count;
    private List<Func<Task>>? _emptyWorkItemList;

    private List<Func<Task>> EmptyWorkItemList => _emptyWorkItemList ??= new();

    private async Task FireThrottleOnClick()
    {
        await CounterThrottleAsync.PushEvent(() =>
        {
            _count++;
            return Task.CompletedTask;
        });
    }

    private bool TryGetWorkItemList(out List<Func<Task>> workItemList)
    {
        try
        {
            workItemList = CounterThrottleAsync._workItemStack.ToList();
            return true;
        }
        catch (Exception)
        {
            workItemList = EmptyWorkItemList;
            return false;
        }
    }
}