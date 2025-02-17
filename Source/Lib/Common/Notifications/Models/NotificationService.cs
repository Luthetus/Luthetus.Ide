using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.ListExtensions;

namespace Luthetus.Common.RazorLib.Notifications.Models;

public class NotificationService : INotificationService
{
	private NotificationState _notificationState = new();
	
	public event Action? NotificationStateChanged;
	
	public NotificationState GetNotificationState() => _notificationState;

    public void ReduceRegisterAction(INotification notification)
    {
    	var inState = GetNotificationState();
    
        var outDefaultList = new List<INotification>(inState.DefaultList);
        outDefaultList.Add(notification);

        _notificationState = inState with { DefaultList = outDefaultList };
        
        NotificationStateChanged?.Invoke();
        return;
    }

    public void ReduceDisposeAction(Key<IDynamicViewModel> key)
    {
    	var inState = GetNotificationState();
    
        var indexNotification = inState.DefaultList.FindIndex(
            x => x.DynamicViewModelKey == key);

        if (indexNotification == -1)
        {
            NotificationStateChanged?.Invoke();
        	return;
        }
            
        var inNotification = inState.DefaultList[indexNotification];

        var outDefaultList = new List<INotification>(inState.DefaultList);
        outDefaultList.RemoveAt(indexNotification);

        _notificationState = inState with { DefaultList = outDefaultList };
        
        NotificationStateChanged?.Invoke();
        return;
    }

    public void ReduceMakeReadAction(Key<IDynamicViewModel> key)
    {
    	var inState = GetNotificationState();
    
        var inNotificationIndex = inState.DefaultList.FindIndex(
            x => x.DynamicViewModelKey == key);

        if (inNotificationIndex == -1)
        {
            NotificationStateChanged?.Invoke();
        	return;
        }

        var inNotification = inState.DefaultList[inNotificationIndex];

        var outDefaultList = new List<INotification>(inState.DefaultList);
        outDefaultList.RemoveAt(inNotificationIndex);
        
        var outReadList = new List<INotification>(inState.ReadList);
        outReadList.Add(inNotification);

        _notificationState = inState with
        {
            DefaultList = outDefaultList,
            ReadList = outReadList
        };
        
        NotificationStateChanged?.Invoke();
        return;
    }
    
    public void ReduceUndoMakeReadAction(Key<IDynamicViewModel> key)
    {
    	var inState = GetNotificationState();
    
        var inNotificationIndex = inState.ReadList.FindIndex(
            x => x.DynamicViewModelKey == key);

        if (inNotificationIndex == -1)
        {
            NotificationStateChanged?.Invoke();
        	return;
        }

        var inNotification = inState.ReadList[inNotificationIndex];

        var outReadList = new List<INotification>(inState.ReadList);
        outReadList.RemoveAt(inNotificationIndex);
        
        var outDefaultList = new List<INotification>(inState.DefaultList);
        outDefaultList.Add(inNotification);

        _notificationState = inState with
        {
            DefaultList = outDefaultList,
            ReadList = outReadList
        };
        
        NotificationStateChanged?.Invoke();
        return;
    }

    public void ReduceMakeDeletedAction(Key<IDynamicViewModel> key)
    {
    	var inState = GetNotificationState();
    
        var inNotificationIndex = inState.DefaultList.FindIndex(
            x => x.DynamicViewModelKey == key);

        if (inNotificationIndex == -1)
        {
            NotificationStateChanged?.Invoke();
        	return;
        }

        var inNotification = inState.DefaultList[inNotificationIndex];

        var outDefaultList = new List<INotification>(inState.DefaultList);
        outDefaultList.RemoveAt(inNotificationIndex);
        
        var outDeletedList = new List<INotification>(inState.DeletedList);
        outDeletedList.Add(inNotification);

        _notificationState = inState with
        {
            DefaultList = outDefaultList,
            DeletedList = outDeletedList
        };
        
        NotificationStateChanged?.Invoke();
        return;
    }

    public void ReduceUndoMakeDeletedAction(Key<IDynamicViewModel> key)
    {
    	var inState = GetNotificationState();
    
        var inNotificationIndex = inState.DeletedList.FindIndex(
            x => x.DynamicViewModelKey == key);

        if (inNotificationIndex == -1)
        {
            NotificationStateChanged?.Invoke();
       	 return;
        }

        var inNotification = inState.DeletedList[inNotificationIndex];

        var outDeletedList = new List<INotification>(inState.DeletedList);
        outDeletedList.RemoveAt(inNotificationIndex);

        var outDefaultList = new List<INotification>(inState.DefaultList);
        outDefaultList.Add(inNotification);

        _notificationState = inState with
        {
            DefaultList = outDefaultList,
            DeletedList = outDeletedList
        };
        
        NotificationStateChanged?.Invoke();
        return;
    }

    public void ReduceMakeArchivedAction(Key<IDynamicViewModel> key)
    {
    	var inState = GetNotificationState();
    
        var inNotificationIndex = inState.DefaultList.FindIndex(
            x => x.DynamicViewModelKey == key);

        if (inNotificationIndex == -1)
        {
            NotificationStateChanged?.Invoke();
        	return;
        }

        var inNotification = inState.DefaultList[inNotificationIndex];

        var outDefaultList = new List<INotification>(inState.DefaultList);
        outDefaultList.RemoveAt(inNotificationIndex);

        var outArchivedList = new List<INotification>(inState.ArchivedList);
        outArchivedList.Add(inNotification);

        _notificationState = inState with
        {
            DefaultList = outDefaultList,
            ArchivedList = outArchivedList
        };
        
        NotificationStateChanged?.Invoke();
        return;
    }
    
    public void ReduceUndoMakeArchivedAction(Key<IDynamicViewModel> key)
    {
    	var inState = GetNotificationState();
    
        var inNotificationIndex = inState.ArchivedList.FindIndex(
            x => x.DynamicViewModelKey == key);

        if (inNotificationIndex == -1)
        {
            NotificationStateChanged?.Invoke();
       	 return;
        }

        var inNotification = inState.ArchivedList[inNotificationIndex];

        var outArchivedList = new List<INotification>(inState.ArchivedList);
        outArchivedList.RemoveAt(inNotificationIndex);
        
        var outDefaultList = new List<INotification>(inState.DefaultList);
        outDefaultList.Add(inNotification);

        _notificationState = inState with
        {
            DefaultList = outDefaultList,
            ArchivedList = outArchivedList
        };
        
        NotificationStateChanged?.Invoke();
        return;
    }

    public void ReduceClearDefaultAction()
    {
    	var inState = GetNotificationState();
    
        _notificationState = inState with
        {
            DefaultList = new List<INotification>()
        };
        
        NotificationStateChanged?.Invoke();
        return;
    }
    
    public void ReduceClearReadAction()
    {
    	var inState = GetNotificationState();
    
        _notificationState = inState with
        {
            ReadList = new List<INotification>()
        };
        
        NotificationStateChanged?.Invoke();
        return;
    }
    
    public void ReduceClearDeletedAction()
    {
    	var inState = GetNotificationState();
    
        _notificationState = inState with
        {
            DeletedList = new List<INotification>()
        };
        
        NotificationStateChanged?.Invoke();
        return;
    }

    public void ReduceClearArchivedAction()
    {
    	var inState = GetNotificationState();
    
        _notificationState = inState with
        {
            ArchivedList = new List<INotification>()
        };
        
        NotificationStateChanged?.Invoke();
        return;
    }
}
