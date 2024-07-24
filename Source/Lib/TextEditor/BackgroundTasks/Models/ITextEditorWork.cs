using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

/// <summary>
/// On the topic of "generic batching":
/// ==================================
/// I am leaving this comment here as a reminder to myself, of
/// what I did to try and achieve "generic batching", but the
/// massive flaws that existed as a result. The goal being,
/// if I ever revisit this topic, I don't just repeat the same mistake.
///
/// Every <see cref="ITextEditorWork"/> when finished executing,
/// will cause the text editor to render.
///
/// So, I started writing code to permit batching of <see cref="ITextEditorWork"/>,
/// in order to reduce the amount of rendering.
///
/// I started by having specific implementations of <see cref="ITextEditorWork"/>
/// be batchable amongst themselves.
///
/// For example, if the user types into the text editor, this results in
/// the construction of a <see cref="Events.Models.OnKeyDownLateBatching"/>.
///
/// Therefore, upon enqueueing an instance of <see cref="Events.Models.OnKeyDownLateBatching"/>,
/// it is found that the background task queue has in it a <see cref="Events.Models.OnKeyDownLateBatching"/>,
/// which came immediately prior (they are contiguous), then combine them into a
/// single <see cref="Events.Models.OnKeyDownLateBatching"/>.
///
/// A more concrete example would be a user typing the letter 'q', then the letter 'w'.
///
/// Instead of:
/// 	Insert("q");
///     Render();
/// 	Insert("w");
///     Render();
///
/// It might be preferable to:
/// 	Insert("qw");
///     Render();
///
/// <see cref="Events.Models.OnKeyDownLateBatching"/> seems useful,
/// but it seems to have an issue.
///
/// Take the example of a user HOLDING down the letter 'q'.
/// Sometimes the enqueueing and dequeueing seem to only batch,
/// and never get handled.
///
/// Then once the user lets go, its discovered that 'q' was batched 100 times
/// (or however many times the keyboard event occurred while being held).
///
/// I suppose I'm looking for it to batch, but eventually just handle the event,
/// and start the batch over again with the next event.
/// Instead of eternally batching under high load.
///
/// After I made an implementation of <see cref="ITextEditorWork"/>
/// for each of the UI events, I thought I would create an implementation
/// for general use.
///
/// So that any task could batch, but instead of the batch optimizing
/// the work (i.e.: 1 insert containing 2 characters instead of 2 inserts with each having 1 character)
/// the batching was just a 'foreach' loop, in order to avoid the render of the text editor.
///
/// So instead of:
/// 	Insert("q");
///     Render();
/// 	Insert("w");
///     Render();
///
/// Do:
/// 	Insert("q");
/// 	Insert("w");
///     Render();
///
/// But, this created an issue relating to when an exception was thrown,
/// by one of the tasks that were put into that foreach loop.
///
/// If one threw an exception, then any following tasks would not run.
///
/// Okay fine, I can put a 'try-catch' inside the 'foreach' loop,
/// around every invocation of an individual task.
///
/// But, the text editor is immutable. And in order to edit
/// the text editor, one is given a edit context.
///
/// In short the edit context is the exact same data as the immutable
/// version, except it is allowed to be changed.
/// That is, the immutable data gets copied into a mutable version of itself.
///
/// Well, if an individual task threw an exception during the 'foreach-batching',
/// is no longer as issue because of the 'try-catch'?
///
/// The outcome was that, the individual task likely was unable to make
/// the edit, that it sought out to make.
///
/// Maybe the task made part of their edit, prior to the exception.
///
/// Well, the metadata of the text editor (i.e.: position indices of the line endings, and etc...)
/// is calculated separately to that of the insertion of text.
///
/// So, the insertion of a '\n' character, actually calculates the new line ending positions
/// presuming it will be added without error, then lastly inserts the character.
///
/// If an exception occurs after calculating the metadata, but before inserting the character,
/// then the text editor will enter a "corrupt"/"unrecoverable" state.
///
/// And yet, if we were to have batched that task which threw an exception, with other tasks
/// following it.
/// We go on to pass along the corrupt text editor state to the tasks that follow.
///
/// So, generic batching can only be done with one further step.
/// Somehow, if a task throws an exception, the edit context needs to be reset to the state
/// it was in prior to the invocation of that task.
///
/// This is a pain because it was previously mentioned that the text editor,
/// after a batched task throws an exception, is likely in a "corrupt"/"unrecoverable" state.
///
/// It seems the only way is to create a new edit context, foreach of the tasks that are being
/// batched.
///
/// This way, if a task throws an exception, then the edit context it was given can be discarded.
/// Otherwise, take its edit context as the new edit context.
///
/// And if there is is a task following, then copy over the edit context and repeat until
/// all the tasks have ran.
///
/// Then, write out the edit context that survives the ordeal and render.
///
/// This way of foreach batching is essentially the same as my mistake, but with
/// the recreating of the edit context every time a task is ran.
/// So one can "rollback" if a "transaction" does not successfully complete.
///
/// But, I wonder if this batching I just described, even if it worked,
/// would it even be worthwhile?
///
/// That is, am I looking at batching too much, when I really need to be
/// optimizing the IBackgroundTask, such to remove the need to batch so much.
///
/// Something I think of a lot, is how I wrote the throttling code.
/// I used an 'await Task.Delay(...)'.
///
/// But in my mind, from a human perspective, starting a stopwatch,
/// then stopping it at the 60 millisecond mark, over and over and over...
/// this sounds like immense overhead.
///
/// I mention "from a human perspective" because thats a way I think
/// about tasks - that I'm asking someone to do something for me.
///
/// I don't know how to use the the 'Timer' class.
/// And sure, if I throttle at 60 milliseconds, the 'Timer'
/// is still starting and stopping every 60 milliseconds just the same.
///
/// But the overhead of "asking someone else" to start and stop every 60 milliseconds
/// disappears, in my head. I'm not sure if the overhead I'm imagining
/// actually would disappear but it feels like there would be less "middleman" logic.
///
/// In my head it feels as though a 'Timer' class is if I were to start and stop
/// every 60 milliseconds. As opposed to asking someone else to do it for me.
/// </summary>
public interface ITextEditorWork : IBackgroundTask
{
	public IEditContext EditContext { get; set; }
}
