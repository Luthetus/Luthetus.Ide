using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.ListExtensions;

namespace Luthetus.Ide.RazorLib.StartupControls.Models;

public class StartupControlService : IStartupControlService
{
    private readonly object _stateModificationLock = new();

    private StartupControlState _startupControlState = new();
	
	public event Action? StartupControlStateChanged;
	
	public StartupControlState GetStartupControlState() => _startupControlState;

	public void RegisterStartupControl(IStartupControlModel startupControl)
	{
		lock (_stateModificationLock)
		{
			var inState = GetStartupControlState();

			var indexOfStartupControl = inState.StartupControlList.FindIndex(
				x => x.Key == startupControl.Key);

			if (indexOfStartupControl != -1)
				goto finalize;

			var outStartupControlList = new List<IStartupControlModel>(inState.StartupControlList);
			outStartupControlList.Add(startupControl);

			_startupControlState = inState with
			{
				StartupControlList = outStartupControlList
			};

            goto finalize;
        }

		finalize:
        StartupControlStateChanged?.Invoke();
    }
	
	public void DisposeStartupControl(Key<IStartupControlModel> startupControlKey)
	{
		lock (_stateModificationLock)
		{
			var inState = GetStartupControlState();

			var indexOfStartupControl = inState.StartupControlList.FindIndex(
				x => x.Key == startupControlKey);

			if (indexOfStartupControl == -1)
                goto finalize;

            var outActiveStartupControlKey = inState.ActiveStartupControlKey;
			if (inState.ActiveStartupControlKey == startupControlKey)
				outActiveStartupControlKey = Key<IStartupControlModel>.Empty;

			var outStartupControlList = new List<IStartupControlModel>(inState.StartupControlList);
			outStartupControlList.RemoveAt(indexOfStartupControl);

			_startupControlState = inState with
			{
				StartupControlList = outStartupControlList,
				ActiveStartupControlKey = outActiveStartupControlKey
			};

            goto finalize;
        }

        finalize:
        StartupControlStateChanged?.Invoke();
    }
	
	public void SetActiveStartupControlKey(Key<IStartupControlModel> startupControlKey)
	{
		lock (_stateModificationLock)
		{
			var inState = GetStartupControlState();

			var startupControl = inState.StartupControlList.FirstOrDefault(
				x => x.Key == startupControlKey);

			if (startupControlKey == Key<IStartupControlModel>.Empty ||
				startupControl is null)
			{
				_startupControlState = inState with
				{
					ActiveStartupControlKey = Key<IStartupControlModel>.Empty
				};

                goto finalize;
            }

			_startupControlState = inState with
			{
				ActiveStartupControlKey = startupControl.Key
			};

            goto finalize;
        }

        finalize:
        StartupControlStateChanged?.Invoke();
    }
	
	public void StateChanged()
	{
		StartupControlStateChanged?.Invoke();
	}
}
