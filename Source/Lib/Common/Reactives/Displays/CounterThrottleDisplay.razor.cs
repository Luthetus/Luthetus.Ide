using Luthetus.Common.RazorLib.Reactives.Models.Internals;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Reactives.Displays;

public partial class CounterThrottleDisplay : ComponentBase
{
    /*
     * This scenario is a perfect example of the issue I'm facing (2024-05-10)
     * Goal: When creating a throttle type: how does one provide a Func<Task>
     *       that will immediately be invoked if the throttle delay is completed.
     * ==========================================================================
     * 
     * I previously had found that when running ServerSide,
     * '.ConfigureAwait(false)' being used or not, had no impact
     * on a 5 second throttle delay. (The UI was freezing for
     * 5 seconds each time).
     * 
     * Well, I just now set on 'IdeMainLayout.razor' a boolean to true,
     * which results in a development website being rendered,
     * rather than the IDE.
     * 
     * I believe the sequence of events was that I then changed from
     * Release to Debug, and then ran the website.
     * 
     * The IDE ended up rendering and I thought that was odd,
     * so I cleaned the solution, then rebuilt it,
     * after that I saw the development website.
     * 
     * I don't have proof that swapping between Release to Debug
     * was the cause of me originally not finding '.ConfigureAwait(false)'
     * to have any impact while using ServerSide,
     * but at the very least I need to make sure to clean the solution,
     * and rebuild anytime I change from Release mode to Debug mode or vice versa.
     * Because there seems to be some oddities going on when I do so.
     * 
     * In the file 'DragInitializer.razor.cs' I wrote out a checklist,
     * so I will copy and paste it here. I believe that a Counter component
     * which increments 'onclick' is going to be the simplest example to use.
     * 
     * I guess I want a checklist so I'm going to type out all 6 cases:
     *     [ ] WASM
     *          [ ] Return the Task (no async keyword)
     *          [ ] Await the Task (with async keyword)
     *          [ ] Await the ConfiguredTaskAwaitable (with async keyword)
     *     [ ] ServerSide
     *          [ ] Return the Task (no async keyword)
     *          [ ] Await the Task (with async keyword)
     *          [ ] Await the ConfiguredTaskAwaitable (with async keyword)
     *          
     * As for the Throttle implementation itself,
     * I'm going to make an interface for IThrottleAsync,
     * and an interface for IThrottle.
     * This allows me to write both an async, and synchronous version.
     * Furthermore it allows many implementations per async, or synchronous version.
     * I can even render the counter with a variety of implementations at the same
     * time if I create 'CounterThrottleAsyncDisplay' and 'CounterThrottleDisplay'.
     * Then each component can accept either an 'IThrottleAsync', or an 'IThrottle'.
     * ==========================================================================
     * 
     * Regarding invoking '.WaitAsync()' on a SemaphoreSlim:
     *     try
     *     {
     *         await _workItemSemaphore.WaitAsync().ConfigureAwait(false);
     *     
     *         _workItemStack.Push(workItem);
     *         if (_workItemStack.Count > 1)
     *             return;
     *     }
     *     finally
     *     {
     *         _workItemSemaphore.Release();
     *     }
     * 
     * I see that people put the '.WaitAsync()' prior to the try-finally.
     *     await _workItemSemaphore.WaitAsync().ConfigureAwait(false);
     *     
     *     try
     *     {
     *         _workItemStack.Push(workItem);
     *         if (_workItemStack.Count > 1)
     *             return;
     *     }
     *     finally
     *     {
     *         _workItemSemaphore.Release();
     *     }
     *     
     * I don't know which way is better, but my thought process is:
     * if one is invoking '.WaitAsync()', then immediately opening
     * a try-finally, to have the '.Release()' in the finally block,
     * then it makes no difference whether the '.WaitAsync()' is immediately prior
     * to the try-finally, or inside the try block.
     * 
     * An issue arises though if one wants to include some timeout logic.
     * One might timeout, cause a TaskCancelledException, then end up invoking '.Release()'
     * yet the semaphore was never entered in the first place.
     * 
     * But, since I don't have any timeout logic, putting the '.WaitAsync()'
     * immediately inside the try block allows me to easily track where the semaphore usage
     * starts and ends since now its all encompassed by that try-finally.
     * 
     * One worry would be that the '.WaitAsync()' could throw an exception.
     * But I believe a bad case exists for both immediately prior to the try-finally,
     * and immediately within the try block.
     * 
     * It comes down to whether '.WaitAsync()' could throw an exception,
     * and whether it would throw said exeption prior to taking a counter from the semaphore,
     * or after taking the counter.
     * 
     * Regardless, I'm presuming neither of those cases can occur?
     * 
     * If one could occur but not the another then there would be a definitive answer.
     * As if the '.WaitAsync()' could throw an exception prior to taking a counter from the semaphore,
     * then a finally block would could be jumped-to and then erroneously run '.Release()'.
     * 
     * And if '.WaitAsync()' could throw  an exception after taking a counter from the semaphore,
     * then one would want a finally block because it should run '.Release()'.
     */

    [Parameter, EditorRequired]
    public ICounterThrottle CounterThrottle { get; set; } = null!;

    private void FireThrottleOnClick()
    {

    }
}