using Luthetus.Common.RazorLib.Exceptions;

namespace Luthetus.Common.RazorLib.Reactives.Models;

public class ProgressBarModel : IDisposable
{
	private readonly object _progressLock = new();

	public ProgressBarModel()
		: this(0, null)
	{
	}

	public ProgressBarModel(double decimalPercentProgress)
		: this(decimalPercentProgress, null)
	{
	}

	public ProgressBarModel(double decimalPercentProgress, string? message)
	{
		DecimalPercentProgress = decimalPercentProgress;
		Message = message;
	}

	public double DecimalPercentProgress { get; private set; }
	public string? Message { get; private set; }
	public bool IsDisposed { get; private set; }

	/// <summary>
	/// When <see cref="SetProgress(double)"/> is invoked, then this event is raised
	/// with the bool value of 'false'. In this scenario the <see cref="DecimalPercentProgress"/>
	/// is "implied" to have changed, and perhaps one should re-render the progress bar UI.
	/// 
	/// When <see cref="Dispose()"/> is invoked, then this event is raised with
	/// the bool value of 'true'. In this scenario, the subscriber should unsubscribe
	/// from this event, as <see cref="SetProgress(double)"/> is no longer allowed to be invoked.
	/// Note: if one has a reference to this object, they can still invoke <see cref="GetProgress()"/>,
	/// even if the <see cref="IsDisposed"/> property is set to a value of 'true'.
	/// </summary>
	public event Action<bool>? ProgressChanged;

	public void SetProgress(double decimalPercentProgress, string? message = null)
	{
		if (IsDisposed)
			throw new LuthetusCommonException($"The {nameof(ProgressBarModel)} has {nameof(IsDisposed)} set to true, therefore cannot be set anymore.");

		lock (_progressLock)
		{
			DecimalPercentProgress = decimalPercentProgress;
			Message = message;
		}

		ProgressChanged?.Invoke(false);
	}

	public double GetProgress()
	{
		// TODO: Is returning from inside of a lock equivalent to this capturing of the value logic in regards to thread safety/concurrency?
		double decimalPercentProgress;

		lock (_progressLock)
		{
			decimalPercentProgress = DecimalPercentProgress;
		}

		return decimalPercentProgress;
	}

	public void Dispose()
	{
		lock (_progressLock)
		{
			IsDisposed = true;
			ProgressChanged?.Invoke(true);
		}
		
	}
}
