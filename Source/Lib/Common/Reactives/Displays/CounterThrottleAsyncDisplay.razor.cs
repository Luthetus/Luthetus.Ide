using Luthetus.Common.RazorLib.Reactives.Models.Internals;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Reactives.Displays;

public partial class CounterThrottleAsyncDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public CounterThrottleAsync CounterThrottleAsync { get; set; } = new(TimeSpan.FromMilliseconds(2_000));

    private int _count;
    private List<Func<Task>>? _emptyWorkItemList;
    private List<CounterThrottleAsync.ExecutionKind>? _executionKindList;

    private List<Func<Task>> EmptyWorkItemList => _emptyWorkItemList ??= new();
    private List<CounterThrottleAsync.ExecutionKind> ExecutionKindList => _executionKindList ??= Enum.GetValues<CounterThrottleAsync.ExecutionKind>().ToList();

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

    private async Task OnExecutionKindChanged(ChangeEventArgs changeEventArgs)
    {
        if (changeEventArgs.Value is null)
            return;

        var executionKindString = (string)changeEventArgs.Value;

        if (Enum.TryParse<CounterThrottleAsync.ExecutionKind>(executionKindString, out var selectedExecutionKind))
            await CounterThrottleAsync.SetExecuteKind(selectedExecutionKind);
    }
}