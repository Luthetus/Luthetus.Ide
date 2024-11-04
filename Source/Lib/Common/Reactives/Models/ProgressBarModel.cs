using Luthetus.Common.RazorLib.Exceptions;

namespace Luthetus.Common.RazorLib.Reactives.Models;

public class ProgressBarModel : IDisposable
{
	private readonly object _progressLock = new();
	private readonly CancellationToken? _cancellationToken;
	
	public ProgressBarModel(CancellationToken? cancellationToken)
		: this(0, null, cancellationToken)
	{
	}

	public ProgressBarModel(double decimalPercentProgress, CancellationToken? cancellationToken)
		: this(decimalPercentProgress, null, cancellationToken)
	{
	}

	public ProgressBarModel(double decimalPercentProgress, string? message, CancellationToken? cancellationToken)
	{
		DecimalPercentProgress = decimalPercentProgress;
		Message = message;
		_cancellationToken = cancellationToken;
		
		if (_cancellationToken is not null)
		{
			_stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken.Value);
		}
	}
	
	private CancellationTokenSource? _stoppingCts;

	public double DecimalPercentProgress { get; private set; }
	public string? Message { get; private set; }
	public string? SecondaryMessage { get; private set; }
	public bool IsCancellable => _cancellationToken is not null;
	public bool IsCancelled => _cancellationToken.Value.IsCancellationRequested;
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

	/// <summary>
	/// If a parameter is null, then their corresponding property will not be changed.
	/// </summary>
	public void SetProgress(double? decimalPercentProgress, string? message = null, string? secondaryMessage = null)
	{
		if (IsDisposed)
			throw new LuthetusCommonException($"The {nameof(ProgressBarModel)} has {nameof(IsDisposed)} set to true, therefore cannot be set anymore.");

		lock (_progressLock)
		{
			if (decimalPercentProgress is not null)
				DecimalPercentProgress = decimalPercentProgress.Value;

			if (message is not null)
				Message = message;

			if (secondaryMessage is not null)
				SecondaryMessage = secondaryMessage;
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
	
	public void Cancel()
	{
		Console.WriteLine("ProgressBarModel.Cancel()");
	
		if (_stoppingCts is not null)
		{
			_stoppingCts.Cancel();
			ProgressChanged?.Invoke(false);
		}
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
