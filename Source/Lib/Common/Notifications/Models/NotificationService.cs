using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.ListExtensions;

namespace Luthetus.Common.RazorLib.Notifications.Models;

public class NotificationService : INotificationService
{
	private readonly object _stateModificationLock = new();

	private NotificationState _notificationState = new();
	
	public event Action? NotificationStateChanged;
	
	public NotificationState GetNotificationState() => _notificationState;

    public void ReduceRegisterAction(INotification notification)
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetNotificationState();
	    
	        var outDefaultList = new List<INotification>(inState.DefaultList);
	        outDefaultList.Add(notification);
	
	        _notificationState = inState with { DefaultList = outDefaultList };
	        
	        goto finalize;
	    }
	    
	    finalize:
	    NotificationStateChanged?.Invoke();
    }

    public void ReduceDisposeAction(Key<IDynamicViewModel> key)
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetNotificationState();
	    
	        var indexNotification = inState.DefaultList.FindIndex(
	            x => x.DynamicViewModelKey == key);
	
	        if (indexNotification == -1)
	        	goto finalize;
	            
	        var inNotification = inState.DefaultList[indexNotification];
	
	        var outDefaultList = new List<INotification>(inState.DefaultList);
	        outDefaultList.RemoveAt(indexNotification);
	
	        _notificationState = inState with { DefaultList = outDefaultList };
	        
	        goto finalize;
	    }
	    
	    finalize:
	    NotificationStateChanged?.Invoke();
    }

    public void ReduceMakeReadAction(Key<IDynamicViewModel> key)
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetNotificationState();
	    
	        var inNotificationIndex = inState.DefaultList.FindIndex(
	            x => x.DynamicViewModelKey == key);
	
	        if (inNotificationIndex == -1)
	        	goto finalize;
	
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
	        
	        goto finalize;
	    }
	    
	    finalize:
	    NotificationStateChanged?.Invoke();
    }
    
    public void ReduceUndoMakeReadAction(Key<IDynamicViewModel> key)
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetNotificationState();
	    
	        var inNotificationIndex = inState.ReadList.FindIndex(
	            x => x.DynamicViewModelKey == key);
	
	        if (inNotificationIndex == -1)
	        	goto finalize;
	
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
	        
	        goto finalize;
	    }
	    
	    finalize:
	    NotificationStateChanged?.Invoke();
    }

    public void ReduceMakeDeletedAction(Key<IDynamicViewModel> key)
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetNotificationState();
	    
	        var inNotificationIndex = inState.DefaultList.FindIndex(
	            x => x.DynamicViewModelKey == key);
	
	        if (inNotificationIndex == -1)
	        	goto finalize;
	
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
	        
	        goto finalize;
	    }
	    
	    finalize:
	    NotificationStateChanged?.Invoke();
    }

    public void ReduceUndoMakeDeletedAction(Key<IDynamicViewModel> key)
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetNotificationState();
	    
	        var inNotificationIndex = inState.DeletedList.FindIndex(
	            x => x.DynamicViewModelKey == key);
	
	        if (inNotificationIndex == -1)
	        	goto finalize;
	
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
	        
	        goto finalize;
	    }
	    
	    finalize:
	    NotificationStateChanged?.Invoke();
    }

    public void ReduceMakeArchivedAction(Key<IDynamicViewModel> key)
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetNotificationState();
	    
	        var inNotificationIndex = inState.DefaultList.FindIndex(
	            x => x.DynamicViewModelKey == key);
	
	        if (inNotificationIndex == -1)
	        	goto finalize;
	
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
	        
	        goto finalize;
	    }
	    
	    finalize:
	    NotificationStateChanged?.Invoke();
    }
    
    public void ReduceUndoMakeArchivedAction(Key<IDynamicViewModel> key)
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetNotificationState();
	    
	        var inNotificationIndex = inState.ArchivedList.FindIndex(
	            x => x.DynamicViewModelKey == key);
	
	        if (inNotificationIndex == -1)
	        	goto finalize;
	
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
	        
	        goto finalize;
	    }
	    
	    finalize:
	    NotificationStateChanged?.Invoke();
    }

    public void ReduceClearDefaultAction()
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetNotificationState();
	    
	        _notificationState = inState with
	        {
	            DefaultList = new List<INotification>()
	        };
	        
	        goto finalize;
	    }
	    
	    finalize:
	    NotificationStateChanged?.Invoke();
    }
    
    public void ReduceClearReadAction()
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetNotificationState();
	    
	        _notificationState = inState with
	        {
	            ReadList = new List<INotification>()
	        };
	        
	        goto finalize;
	    }
	    
	    finalize:
	    NotificationStateChanged?.Invoke();
    }
    
    public void ReduceClearDeletedAction()
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetNotificationState();
	    
	        _notificationState = inState with
	        {
	            DeletedList = new List<INotification>()
	        };
	        
	        goto finalize;
	    }
	    
	    finalize:
	    NotificationStateChanged?.Invoke();
    }

    public void ReduceClearArchivedAction()
    {
    	lock (_stateModificationLock)
    	{
	    	var inState = GetNotificationState();
	    
	        _notificationState = inState with
	        {
	            ArchivedList = new List<INotification>()
	        };
	        
	        goto finalize;
	    }
	    
	    finalize:
	    NotificationStateChanged?.Invoke();
    }
}
