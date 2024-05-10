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
     * and an interface for ThrottleAsync.
     * This allows me to write both an async, and synchronous version.
     * Furthermore it allows many implementations per async, or synchronous version.
     * I can even render the counter with a variety of implementations at the same
     * time if I create 'CounterThrottleAsyncDisplay' and 'CounterThrottleDisplay'.
     * Then each component can accept either a 'IThrottleAsync', or a 'ThrottleAsync'.
     */
}