namespace Luthetus.Ide.Tests.Adhoc;

/// <summary>
/// Personal tests of "Hunter Freeman" are in this file.
/// Probably delete this file long term.
///
/// Purpose of these tests:
/// I like to think that I understand the phrases:
///     -Synchronous
///     -Asynchronous
///     -Concurrency
///     -Parallel
///     -Single Threaded (should the word 'runtime' be added to the end of this?)
///     -Multi Threaded (should the word 'runtime' be added to the end of this?)
///     -Threads as a general concept
///     -Operating System task vs process 'things'
///     -Operating System level Task Scheduling
///     -.NET level Task Scheduling
///     -.NET to Operating System interop relating to tasks
///     -.NET SynchronizationContext
///     -.NET HostedService
///     -.NET BackgroundService
///
/// But, I think the reality is that I don't have clue what I'm doing.
/// 
/// So, I'd like to learn, and do better.
///
/// Hopefully I can write a test for every detail of how these things work,
/// and by doing so learn something.
/// </summary>
public class SynchronizationContextTests
{
    /// <summary>
    /// Idea: Assign a value, increment that value, then assert initialValue + 1.<br/><br/>
    /// 
    /// Reason: This code runs synchronously, therefore by the time the program reaches the assertion,
    ///         the increment is guaranteed to have occurred.
    /// </summary>
	[Fact]
	public void Synchronous()
	{
		var initialValue = 2;
		var expectedValue = 3;

		var x = initialValue;
		x++;

		Assert.Equal(expectedValue, x);
	}

    /// <summary>
    /// Idea: Assign a value, increment that value, then assert initialValue + 1.<br/><br/>
    /// 
    /// Reason: This code runs asynchronously, but the task is awaited, therefore by the time the program reaches the assertion,
    ///         the increment is guaranteed to have occurred.
    /// </summary>
    [Fact]
    public async Task AsynchronousWithAwait()
    {
        var initialValue = 2;
        var expectedValue = 3;

        var x = initialValue;
        await Task.Run(() => { x++; });

        Assert.Equal(expectedValue, x);
    }

    /// <summary>
    /// Idea: Assign a value, increment that value, then assert initialValue + 1.<br/><br/>
    /// 
    /// Reason: This code runs asynchronously, but the task is NOT awaited, therefore by the time the program reaches the assertion,
    ///         the increment is guaranteed to NOT have occurred.
    ///         
    /// Thoughts:
    ///     -Is it true that the increment is guaranteed to NOT have occurred?
    ///     -If I run this enough times could the scheduler for some reason decide to run it immediately?
    ///     -If I run this code on various operating systems, could the output be different, by nature of the
    ///          operating system's scheduler?
    ///     -Does the computer's architecture come into play with scheduling tasks?
    ///          -If it does, how much variance can I encounter by changing the computer's architecture?
    ///     -If I add enough code between the 'Task.Run' that isn't awaited, and the assertion,
    ///          could I manage to have the task complete prior to the assertion?
    ///          -I specifically say 'code' here rather than 'await Task.Delay(...)', because I
    ///               imagine the inclusion of the 'await' keyword could result in a different
    ///               outcome than if I just added something synchronous, but long enough timespan to compute,
    ///               that the task somehow finishes.
    /// </summary>
    [Fact]
    public async Task AsynchronousNoAwait()
    {
        var initialValue = 2;
        var expectedValue = 2;

        var x = initialValue;
        Task.Run(() => { x++; });

        Assert.Equal(expectedValue, x);
    }

    /// <summary>
    /// This test says the actual value is 3 for my current computer.
    /// But why did it decide to complete the task here?
    /// What if I have the for loop go up to a smaller upper limit.
    /// </summary>
    [Fact]
    public async Task AsynchronousNoAwait_TryToGetTheNotAwaitedTaskToComplete_ByExecutingSynchronousCodeBetweenItAndTheIncrementation_LargeForLoopUpperBound()
    {
        var initialValue = 2;
        var expectedValue = 3;

        var x = initialValue;
        Task.Run(() => { x++; });

        for (var i = 0; i < 1_000_000; i++)
        {
            // If I leave this empty could the compiler decide to 'optimize' it out,
            // on the basis that it has an empty code block?
        }

        Assert.Equal(expectedValue, x);
    }

    /// <summary>
    /// This test says the actual value is 2 on my computer.
    /// It is deeply unsettling that when the upper bound was '1_000_000' it did increment, yet here it didn't<br/><br/>
    /// 
    /// In terms of a "human feeling". It "feels" like it makes sense. But at the end of the day,
    /// I have no idea why it did this.<br/><br/>
    /// 
    /// Its horrifying to think that my computer is running, and I'm using it at this moment.
    /// Because there is an unfathomable amount of code being executed, and tasks communicating to one another.<br/><br/>
    /// 
    /// How is it possible for the internet to function? Perhaps I purchase something online.
    /// I then get charged for "some"-dollar amount, maybe $50 is the charge.
    /// I then attempt to pay $50, but somehow the "bytes" that are sent
    /// over the internet, are incorrectly saying that I am paying $5,000?<br/><br/>
    /// 
    /// Computers appear to be like glass to me. I feel like at any moment everything could shatter,
    /// and yet it often works.<br/><br/>
    /// 
    /// My perspective here is largely from a "human"-perspective.
    /// That is, what if my computer tries to add "4 + 4",
    /// but is feeling a bit tired that day, and tells me the result is 44 instead of 8?<br/><br/>
    /// 
    /// Wouldn't it only take 1 tiny error in my computer for the entire thing to crash?
    /// Suppossedly 1s and 0s are set/unset. How many times per second is this being done?
    /// Its nauseating to think of doing this as a human, and not making a mistake.<br/><br/>
    /// 
    /// Its like some odd anxiety that I could write a for loop in C# that writes "Hello World\n".
    /// But if I run it for enough loops it somehow writes out "Abc123"?<br/><br/>
    /// 
    /// I am struggling to wrap my head around the setting of 1s and 0s.
    /// I imagine a computer going along setting its bits, then lightning strikes,
    /// and this somehow makes the computer write out a 1 instead of a 0.
    /// Then the result of this singular bit being incorrectly written out
    /// results in some catastrophic event.<br/><br/>
    /// 
    /// Even worse, simply imagining a computer settings 1s and 0s, in my head, there is
    /// an extremely anxious thought that it goes to mess up, due to no external circumstances,
    /// and it will be solely inexplainable.<br/><br/>
    /// </summary>
    [Fact]
    public async Task AsynchronousNoAwait_TryToGetTheNotAwaitedTaskToComplete_ByExecutingSynchronousCodeBetweenItAndTheIncrementation_SmallForLoopUpperBound()
    {
        var initialValue = 2;
        var expectedValue = 2;

        var x = initialValue;
        Task.Run(() => { x++; });

        for (var i = 0; i < 10; i++)
        {
            // If I leave this empty could the compiler decide to 'optimize' it out,
            // on the basis that it has an empty code block?
        }

        Assert.Equal(expectedValue, x);
    }

    /// <summary>
    /// This test has 3 as the actual value on my computer.
    /// </summary>
    [Fact]
    public async Task AsynchronousNoAwait_TryToGetTheNotAwaitedTaskToComplete_ByAwaitingATaskBetweenItAndTheIncrementation_TaskDelayNonZero()
    {
        var initialValue = 2;
        var expectedValue = 3;

        var x = initialValue;
        Task.Run(() => { x++; });

        await Task.Delay(100);

        Assert.Equal(expectedValue, x);
    }

    /// <summary>
    /// This test has 2 as the actual value on my computer.
    /// </summary>
    [Fact]
    public async Task AsynchronousNoAwait_TryToGetTheNotAwaitedTaskToComplete_ByAwaitingATaskBetweenItAndTheIncrementationTaskDelayZero()
    {
        var initialValue = 2;
        var expectedValue = 2;

        var x = initialValue;
        Task.Run(() => { x++; });

        await Task.Delay(0);

        Assert.Equal(expectedValue, x);
    }

    /// <summary>
    /// This test has 2 as the actual value on my computer.<br/><br/>
    /// 
    /// Idea: I have a test for a 0ms delay, and a 1000ms delay.
    ///       The 0ms delay says the actual value is 2,
    ///       whereas the 1000ms delay says the actual value is 3.
    ///       |
    ///       I'm wondering if maybe the 0ms delay results in some sort of
    ///       short circuiting, as if the 'await' keyword isn't even there.
    ///       |
    ///       For that reasoning, I'm going to use a 1ms delay, to see if it ends up
    ///       with the actual value being 3.
    /// </summary>
    [Fact]
    public async Task AsynchronousNoAwait_TryToGetTheNotAwaitedTaskToComplete_ByAwaitingATaskBetweenItAndTheIncrementationTaskDelayOne()
    {
        var initialValue = 2;
        var expectedValue = 3;

        var x = initialValue;
        Task.Run(() => { x++; });

        await Task.Delay(1);

        Assert.Equal(expectedValue, x);
    }

    /// <summary>
    /// Can concurrency be performed with a single thread?
    ///     -An 'async-await' model allows this.
    /// Can concurrency be performed with a multiple threads?
    ///     -I'm struggling to respond to this, because would this
    ///          be parallelism?
    ///          -What is the difference between concurrency and parallelism.
    ///          -I suppose you could execute multiple threads concurrently,
    ///               by using an 'async-await' model.
    ///          -But that furthermore if one ran the threads on two separate computers,
    ///               then that would be parallelism (no longer concurrent)
    ///          -I found it easier to think of "two separate computers",
    ///               rather than one computer performing parallelism with
    ///               its own hardware.
    ///               -I think this clearly shows that I don't understand computer
    ///                    hardware, and that I should learn more about it.
    ///     -Is concurrency referring to running many tasks on a single processor
    ///          with an 'await-async' like model,
    ///          where as parallelism refers to having each task
    ///          run on a separate processor?
    ///          -Furthermore, what is a processor, is this the correct word?
    /// </summary>
    [Fact]
    public async Task Concurrency()
    {
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    [Fact]
    public void Parallel()
    {

    }
}
