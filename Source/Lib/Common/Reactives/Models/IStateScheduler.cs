namespace Luthetus.Common.RazorLib.Reactives.Models;

/// <summary>
/// Background and asynchronous state modification needs to be scheduled
/// to the IDispatcher.
///
/// Once state is given to the IDispatcher then it is synchronously,
/// and thread safely, written over the existing state.
///
/// Then, the next background and asynchronous state modification reads the new state,
/// and does its calculation, hands it off the the IDispatcher, repeat.
///
/// NOTE: I think 'Scheduler' is the wrong word to use here. I need to continue thinking this through.
///       and then revisit the name.
/// </summary>
public interface IStateScheduler
{
	
}
