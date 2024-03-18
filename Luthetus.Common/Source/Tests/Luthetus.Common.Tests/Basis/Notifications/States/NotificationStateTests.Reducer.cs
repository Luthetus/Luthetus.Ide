using Fluxor;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Notifications.States;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Common.Tests.Basis.Notifications.States;

/// <summary>
/// <see cref="NotificationState.Reducer"/>
/// </summary>
public class NotificationStateReducerTests
{
    /// <summary>
    /// <see cref="NotificationState.Reducer.ReduceRegisterAction(NotificationState, NotificationState.RegisterAction)"/>
    /// </summary>
    [Fact]
    public void ReduceRegisterAction()
    {
        InitializeNotificationStateReducerTests(
            out var serviceProvider,
            out var notificationStateWrap,
            out var dispatcher,
            out var notificationRecord);

        Assert.Empty(notificationStateWrap.Value.DefaultList);

        dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));

        Assert.Single(notificationStateWrap.Value.DefaultList);
        Assert.Contains(notificationStateWrap.Value.DefaultList, x => x == notificationRecord);
    }

    /// <summary>
    /// <see cref="NotificationState.Reducer.ReduceDisposeAction(NotificationState, NotificationState.DisposeAction)"/>
    /// </summary>
    [Fact]
    public void ReduceDisposeAction()
    {
        InitializeNotificationStateReducerTests(
            out var serviceProvider,
            out var notificationStateWrap,
            out var dispatcher,
            out var notificationRecord);

        dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));

        Assert.Single(notificationStateWrap.Value.DefaultList);
        Assert.Contains(notificationStateWrap.Value.DefaultList, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.DisposeAction(notificationRecord.DynamicViewModelKey));
        
        Assert.Empty(notificationStateWrap.Value.DefaultList);
    }

    /// <summary>
    /// <see cref="NotificationState.Reducer.ReduceMakeReadAction(NotificationState, NotificationState.MakeReadAction)"/>
    /// </summary>
    [Fact]
    public void ReduceMakeReadAction()
    {
        InitializeNotificationStateReducerTests(
            out var serviceProvider,
            out var notificationStateWrap,
            out var dispatcher,
            out var notificationRecord);

        dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));

        Assert.Single(notificationStateWrap.Value.DefaultList);
        Assert.Contains(notificationStateWrap.Value.DefaultList, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.MakeReadAction(notificationRecord.DynamicViewModelKey));
        
        Assert.Empty(notificationStateWrap.Value.DefaultList);

        Assert.Single(notificationStateWrap.Value.ReadList);
        Assert.Contains(notificationStateWrap.Value.ReadList, x => x == notificationRecord);
    }

    /// <summary>
    /// <see cref="NotificationState.Reducer.ReduceUndoMakeReadAction(NotificationState, NotificationState.UndoMakeReadAction)"/>
    /// </summary>
    [Fact]
    public void ReduceUndoMakeReadAction()
    {
        InitializeNotificationStateReducerTests(
            out var serviceProvider,
            out var notificationStateWrap,
            out var dispatcher,
            out var notificationRecord);

        dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));

        Assert.Single(notificationStateWrap.Value.DefaultList);
        Assert.Contains(notificationStateWrap.Value.DefaultList, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.MakeReadAction(notificationRecord.DynamicViewModelKey));

        Assert.Empty(notificationStateWrap.Value.DefaultList);

        Assert.Single(notificationStateWrap.Value.ReadList);
        Assert.Contains(notificationStateWrap.Value.ReadList, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.UndoMakeReadAction(notificationRecord.DynamicViewModelKey));

        Assert.Empty(notificationStateWrap.Value.ReadList);

        Assert.Single(notificationStateWrap.Value.DefaultList);
        Assert.Contains(notificationStateWrap.Value.DefaultList, x => x == notificationRecord);
    }

    /// <summary>
    /// <see cref="NotificationState.Reducer.ReduceMakeDeletedAction(NotificationState, NotificationState.MakeDeletedAction)"/>
    /// </summary>
    [Fact]
    public void ReduceMakeDeletedAction()
    {
        InitializeNotificationStateReducerTests(
            out var serviceProvider,
            out var notificationStateWrap,
            out var dispatcher,
            out var notificationRecord);

        dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));

        Assert.Single(notificationStateWrap.Value.DefaultList);
        Assert.Contains(notificationStateWrap.Value.DefaultList, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.MakeDeletedAction(notificationRecord.DynamicViewModelKey));

        Assert.Empty(notificationStateWrap.Value.DefaultList);

        Assert.Single(notificationStateWrap.Value.DeletedList);
        Assert.Contains(notificationStateWrap.Value.DeletedList, x => x == notificationRecord);
    }

    /// <summary>
    /// <see cref="NotificationState.Reducer.ReduceUndoMakeDeletedAction(NotificationState, NotificationState.UndoMakeDeletedAction)"/>
    /// </summary>
    [Fact]
    public void ReduceUndoMakeDeletedAction()
    {
        InitializeNotificationStateReducerTests(
            out var serviceProvider,
            out var notificationStateWrap,
            out var dispatcher,
            out var notificationRecord);

        dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));

        Assert.Single(notificationStateWrap.Value.DefaultList);
        Assert.Contains(notificationStateWrap.Value.DefaultList, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.MakeDeletedAction(notificationRecord.DynamicViewModelKey));

        Assert.Empty(notificationStateWrap.Value.DefaultList);

        Assert.Single(notificationStateWrap.Value.DeletedList);
        Assert.Contains(notificationStateWrap.Value.DeletedList, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.UndoMakeDeletedAction(notificationRecord.DynamicViewModelKey));

        Assert.Empty(notificationStateWrap.Value.DeletedList);

        Assert.Single(notificationStateWrap.Value.DefaultList);
        Assert.Contains(notificationStateWrap.Value.DefaultList, x => x == notificationRecord);
    }

    /// <summary>
    /// <see cref="NotificationState.Reducer.ReduceMakeArchivedAction(NotificationState, NotificationState.MakeArchivedAction)"/>
    /// </summary>
    [Fact]
    public void ReduceMakeArchivedAction()
    {
        InitializeNotificationStateReducerTests(
            out var serviceProvider,
            out var notificationStateWrap,
            out var dispatcher,
            out var notificationRecord);

        dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));

        Assert.Single(notificationStateWrap.Value.DefaultList);
        Assert.Contains(notificationStateWrap.Value.DefaultList, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.MakeArchivedAction(notificationRecord.DynamicViewModelKey));

        Assert.Empty(notificationStateWrap.Value.DefaultList);

        Assert.Single(notificationStateWrap.Value.ArchivedList);
        Assert.Contains(notificationStateWrap.Value.ArchivedList, x => x == notificationRecord);
    }

    /// <summary>
    /// <see cref="NotificationState.Reducer.ReduceUndoMakeArchivedAction(NotificationState, NotificationState.UndoMakeArchivedAction)"/>
    /// </summary>
    [Fact]
    public void ReduceUndoMakeArchivedAction()
    {
        InitializeNotificationStateReducerTests(
            out var serviceProvider,
            out var notificationStateWrap,
            out var dispatcher,
            out var notificationRecord);

        dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));

        Assert.Single(notificationStateWrap.Value.DefaultList);
        Assert.Contains(notificationStateWrap.Value.DefaultList, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.MakeArchivedAction(notificationRecord.DynamicViewModelKey));

        Assert.Empty(notificationStateWrap.Value.DefaultList);

        Assert.Single(notificationStateWrap.Value.ArchivedList);
        Assert.Contains(notificationStateWrap.Value.ArchivedList, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.UndoMakeArchivedAction(notificationRecord.DynamicViewModelKey));

        Assert.Empty(notificationStateWrap.Value.ArchivedList);

        Assert.Single(notificationStateWrap.Value.DefaultList);
        Assert.Contains(notificationStateWrap.Value.DefaultList, x => x == notificationRecord);
    }

    /// <summary>
    /// <see cref="NotificationState.Reducer.ReduceClearDefaultAction(NotificationState)"/>
    /// </summary>
    [Fact]
    public void ReduceClearDefaultAction()
    {
        InitializeNotificationStateReducerTests(
            out var serviceProvider,
            out var notificationStateWrap,
            out var dispatcher,
            out var notificationRecord);

        // 0 items
        {
            Assert.Empty(notificationStateWrap.Value.DefaultList);
            
            dispatcher.Dispatch(new NotificationState.ClearDefaultAction());
            Assert.Empty(notificationStateWrap.Value.DefaultList);
        }

        // 1 item
        {
            Assert.Empty(notificationStateWrap.Value.DefaultList);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            Assert.Single(notificationStateWrap.Value.DefaultList);
            Assert.Contains(notificationStateWrap.Value.DefaultList, x => x == notificationRecord);
            
            dispatcher.Dispatch(new NotificationState.ClearDefaultAction());
            Assert.Empty(notificationStateWrap.Value.DefaultList);
        }

        // 2 items
        {
            Assert.Empty(notificationStateWrap.Value.DefaultList);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            Assert.Single(notificationStateWrap.Value.DefaultList);
            Assert.Contains(notificationStateWrap.Value.DefaultList, x => x == notificationRecord);

            // Add second item
            {
                var sampleNotificationRecord = new NotificationViewModel(
                    Key<IDynamicViewModel>.NewKey(),
                    "Test title",
                    typeof(CommonInformativeNotificationDisplay),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(CommonInformativeNotificationDisplay.Message),
                            "Test message"
                        }
                    },
                    null,
                    true,
                    null);

                dispatcher.Dispatch(new NotificationState.RegisterAction(sampleNotificationRecord));
                Assert.Equal(2, notificationStateWrap.Value.DefaultList.Count);
                Assert.Contains(notificationStateWrap.Value.DefaultList, x => x == sampleNotificationRecord);
            }

            dispatcher.Dispatch(new NotificationState.ClearDefaultAction());
            Assert.Empty(notificationStateWrap.Value.DefaultList);
        }
        
        // 3 items
        {
            Assert.Empty(notificationStateWrap.Value.DefaultList);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            Assert.Single(notificationStateWrap.Value.DefaultList);
            Assert.Contains(notificationStateWrap.Value.DefaultList, x => x == notificationRecord);

            // Add second item
            {
                var sampleNotificationRecord = new NotificationViewModel(
                    Key<IDynamicViewModel>.NewKey(),
                    "Test title",
                    typeof(CommonInformativeNotificationDisplay),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(CommonInformativeNotificationDisplay.Message),
                            "Test message"
                        }
                    },
                    null,
                    true,
                    null);

                dispatcher.Dispatch(new NotificationState.RegisterAction(sampleNotificationRecord));
                Assert.Equal(2, notificationStateWrap.Value.DefaultList.Count);
                Assert.Contains(notificationStateWrap.Value.DefaultList, x => x == sampleNotificationRecord);
            }
            
            // Add third item
            {
                var sampleNotificationRecord = new NotificationViewModel(
                    Key<IDynamicViewModel>.NewKey(),
                    "Test title",
                    typeof(CommonInformativeNotificationDisplay),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(CommonInformativeNotificationDisplay.Message),
                            "Test message"
                        }
                    },
                    null,
                    true,
                    null);

                dispatcher.Dispatch(new NotificationState.RegisterAction(sampleNotificationRecord));
                Assert.Equal(3, notificationStateWrap.Value.DefaultList.Count);
                Assert.Contains(notificationStateWrap.Value.DefaultList, x => x == sampleNotificationRecord);
            }

            dispatcher.Dispatch(new NotificationState.ClearDefaultAction());
            Assert.Empty(notificationStateWrap.Value.DefaultList);
        }
    }

    /// <summary>
    /// <see cref="NotificationState.Reducer.ReduceClearReadAction(NotificationState)"/>
    /// </summary>
    [Fact]
    public void ReduceClearReadAction()
    {
        InitializeNotificationStateReducerTests(
            out var serviceProvider,
            out var notificationStateWrap,
            out var dispatcher,
            out var notificationRecord);

        // 0 items
        {
            Assert.Empty(notificationStateWrap.Value.ReadList);

            dispatcher.Dispatch(new NotificationState.ClearReadAction());
            Assert.Empty(notificationStateWrap.Value.ReadList);
        }

        // 1 item
        {
            Assert.Empty(notificationStateWrap.Value.ReadList);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeReadAction(notificationRecord.DynamicViewModelKey));
            Assert.Single(notificationStateWrap.Value.ReadList);
            Assert.Contains(notificationStateWrap.Value.ReadList, x => x == notificationRecord);

            dispatcher.Dispatch(new NotificationState.ClearReadAction());
            Assert.Empty(notificationStateWrap.Value.ReadList);
        }

        // 2 items
        {
            Assert.Empty(notificationStateWrap.Value.ReadList);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeReadAction(notificationRecord.DynamicViewModelKey));
            Assert.Single(notificationStateWrap.Value.ReadList);
            Assert.Contains(notificationStateWrap.Value.ReadList, x => x == notificationRecord);

            // Add second item
            {
                var sampleNotificationRecord = new NotificationViewModel(
                    Key<IDynamicViewModel>.NewKey(),
                    "Test title",
                    typeof(CommonInformativeNotificationDisplay),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(CommonInformativeNotificationDisplay.Message),
                            "Test message"
                        }
                    },
                    null,
                    true,
                    null);

                dispatcher.Dispatch(new NotificationState.RegisterAction(sampleNotificationRecord));
                dispatcher.Dispatch(new NotificationState.MakeReadAction(sampleNotificationRecord.DynamicViewModelKey));
                Assert.Equal(2, notificationStateWrap.Value.ReadList.Count);
                Assert.Contains(notificationStateWrap.Value.ReadList, x => x == sampleNotificationRecord);
            }

            dispatcher.Dispatch(new NotificationState.ClearReadAction());
            Assert.Empty(notificationStateWrap.Value.ReadList);
        }

        // 3 items
        {
            Assert.Empty(notificationStateWrap.Value.ReadList);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeReadAction(notificationRecord.DynamicViewModelKey));
            Assert.Single(notificationStateWrap.Value.ReadList);
            Assert.Contains(notificationStateWrap.Value.ReadList, x => x == notificationRecord);

            // Add second item
            {
                var sampleNotificationRecord = new NotificationViewModel(
                    Key<IDynamicViewModel>.NewKey(),
                    "Test title",
                    typeof(CommonInformativeNotificationDisplay),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(CommonInformativeNotificationDisplay.Message),
                            "Test message"
                        }
                    },
                    null,
                    true,
                    null);

                dispatcher.Dispatch(new NotificationState.RegisterAction(sampleNotificationRecord));
                dispatcher.Dispatch(new NotificationState.MakeReadAction(sampleNotificationRecord.DynamicViewModelKey));
                Assert.Equal(2, notificationStateWrap.Value.ReadList.Count);
                Assert.Contains(notificationStateWrap.Value.ReadList, x => x == sampleNotificationRecord);
            }

            // Add third item
            {
                var sampleNotificationRecord = new NotificationViewModel(
                    Key<IDynamicViewModel>.NewKey(),
                    "Test title",
                    typeof(CommonInformativeNotificationDisplay),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(CommonInformativeNotificationDisplay.Message),
                            "Test message"
                        }
                    },
                    null,
                    true,
                    null);

                dispatcher.Dispatch(new NotificationState.RegisterAction(sampleNotificationRecord));
                dispatcher.Dispatch(new NotificationState.MakeReadAction(sampleNotificationRecord.DynamicViewModelKey));
                Assert.Equal(3, notificationStateWrap.Value.ReadList.Count);
                Assert.Contains(notificationStateWrap.Value.ReadList, x => x == sampleNotificationRecord);
            }

            dispatcher.Dispatch(new NotificationState.ClearReadAction());
            Assert.Empty(notificationStateWrap.Value.ReadList);
        }
    }

    /// <summary>
    /// <see cref="NotificationState.Reducer.ReduceClearDeletedAction(NotificationState)"/>
    /// </summary>
    [Fact]
    public void ReduceClearDeletedAction()
    {
        InitializeNotificationStateReducerTests(
            out var serviceProvider,
            out var notificationStateWrap,
            out var dispatcher,
            out var notificationRecord);

        // 0 items
        {
            Assert.Empty(notificationStateWrap.Value.DeletedList);

            dispatcher.Dispatch(new NotificationState.ClearDeletedAction());
            Assert.Empty(notificationStateWrap.Value.DeletedList);
        }

        // 1 item
        {
            Assert.Empty(notificationStateWrap.Value.DeletedList);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeDeletedAction(notificationRecord.DynamicViewModelKey));
            Assert.Single(notificationStateWrap.Value.DeletedList);
            Assert.Contains(notificationStateWrap.Value.DeletedList, x => x == notificationRecord);

            dispatcher.Dispatch(new NotificationState.ClearDeletedAction());
            Assert.Empty(notificationStateWrap.Value.DeletedList);
        }

        // 2 items
        {
            Assert.Empty(notificationStateWrap.Value.DeletedList);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeDeletedAction(notificationRecord.DynamicViewModelKey));
            Assert.Single(notificationStateWrap.Value.DeletedList);
            Assert.Contains(notificationStateWrap.Value.DeletedList, x => x == notificationRecord);

            // Add second item
            {
                var sampleNotificationRecord = new NotificationViewModel(
                    Key<IDynamicViewModel>.NewKey(),
                    "Test title",
                    typeof(CommonInformativeNotificationDisplay),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(CommonInformativeNotificationDisplay.Message),
                            "Test message"
                        }
                    },
                    null,
                    true,
                    null);

                dispatcher.Dispatch(new NotificationState.RegisterAction(sampleNotificationRecord));
                dispatcher.Dispatch(new NotificationState.MakeDeletedAction(sampleNotificationRecord.DynamicViewModelKey));
                Assert.Equal(2, notificationStateWrap.Value.DeletedList.Count);
                Assert.Contains(notificationStateWrap.Value.DeletedList, x => x == sampleNotificationRecord);
            }

            dispatcher.Dispatch(new NotificationState.ClearDeletedAction());
            Assert.Empty(notificationStateWrap.Value.DeletedList);
        }

        // 3 items
        {
            Assert.Empty(notificationStateWrap.Value.DeletedList);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeDeletedAction(notificationRecord.DynamicViewModelKey));
            Assert.Single(notificationStateWrap.Value.DeletedList);
            Assert.Contains(notificationStateWrap.Value.DeletedList, x => x == notificationRecord);

            // Add second item
            {
                var sampleNotificationRecord = new NotificationViewModel(
                    Key<IDynamicViewModel>.NewKey(),
                    "Test title",
                    typeof(CommonInformativeNotificationDisplay),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(CommonInformativeNotificationDisplay.Message),
                            "Test message"
                        }
                    },
                    null,
                    true,
                    null);

                dispatcher.Dispatch(new NotificationState.RegisterAction(sampleNotificationRecord));
                dispatcher.Dispatch(new NotificationState.MakeDeletedAction(sampleNotificationRecord.DynamicViewModelKey));
                Assert.Equal(2, notificationStateWrap.Value.DeletedList.Count);
                Assert.Contains(notificationStateWrap.Value.DeletedList, x => x == sampleNotificationRecord);
            }

            // Add third item
            {
                var sampleNotificationRecord = new NotificationViewModel(
                    Key<IDynamicViewModel>.NewKey(),
                    "Test title",
                    typeof(CommonInformativeNotificationDisplay),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(CommonInformativeNotificationDisplay.Message),
                            "Test message"
                        }
                    },
                    null,
                    true,
                    null);

                dispatcher.Dispatch(new NotificationState.RegisterAction(sampleNotificationRecord));
                dispatcher.Dispatch(new NotificationState.MakeDeletedAction(sampleNotificationRecord.DynamicViewModelKey));
                Assert.Equal(3, notificationStateWrap.Value.DeletedList.Count);
                Assert.Contains(notificationStateWrap.Value.DeletedList, x => x == sampleNotificationRecord);
            }

            dispatcher.Dispatch(new NotificationState.ClearDeletedAction());
            Assert.Empty(notificationStateWrap.Value.DeletedList);
        }
    }

    /// <summary>
    /// <see cref="NotificationState.Reducer.ReduceClearArchivedAction(NotificationState)"/>
    /// </summary>
    [Fact]
    public void ReduceClearArchivedAction()
    {
        InitializeNotificationStateReducerTests(
            out var serviceProvider,
            out var notificationStateWrap,
            out var dispatcher,
            out var notificationRecord);

        // 0 items
        {
            Assert.Empty(notificationStateWrap.Value.ArchivedList);

            dispatcher.Dispatch(new NotificationState.ClearArchivedAction());
            Assert.Empty(notificationStateWrap.Value.ArchivedList);
        }

        // 1 item
        {
            Assert.Empty(notificationStateWrap.Value.ArchivedList);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeArchivedAction(notificationRecord.DynamicViewModelKey));
            Assert.Single(notificationStateWrap.Value.ArchivedList);
            Assert.Contains(notificationStateWrap.Value.ArchivedList, x => x == notificationRecord);

            dispatcher.Dispatch(new NotificationState.ClearArchivedAction());
            Assert.Empty(notificationStateWrap.Value.ArchivedList);
        }

        // 2 items
        {
            Assert.Empty(notificationStateWrap.Value.ArchivedList);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeArchivedAction(notificationRecord.DynamicViewModelKey));
            Assert.Single(notificationStateWrap.Value.ArchivedList);
            Assert.Contains(notificationStateWrap.Value.ArchivedList, x => x == notificationRecord);

            // Add second item
            {
                var sampleNotificationRecord = new NotificationViewModel(
                    Key<IDynamicViewModel>.NewKey(),
                    "Test title",
                    typeof(CommonInformativeNotificationDisplay),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(CommonInformativeNotificationDisplay.Message),
                            "Test message"
                        }
                    },
                    null,
                    true,
                    null);

                dispatcher.Dispatch(new NotificationState.RegisterAction(sampleNotificationRecord));
                dispatcher.Dispatch(new NotificationState.MakeArchivedAction(sampleNotificationRecord.DynamicViewModelKey));
                Assert.Equal(2, notificationStateWrap.Value.ArchivedList.Count);
                Assert.Contains(notificationStateWrap.Value.ArchivedList, x => x == sampleNotificationRecord);
            }

            dispatcher.Dispatch(new NotificationState.ClearArchivedAction());
            Assert.Empty(notificationStateWrap.Value.ArchivedList);
        }

        // 3 items
        {
            Assert.Empty(notificationStateWrap.Value.ArchivedList);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeArchivedAction(notificationRecord.DynamicViewModelKey));
            Assert.Single(notificationStateWrap.Value.ArchivedList);
            Assert.Contains(notificationStateWrap.Value.ArchivedList, x => x == notificationRecord);

            // Add second item
            {
                var sampleNotificationRecord = new NotificationViewModel(
                    Key<IDynamicViewModel>.NewKey(),
                    "Test title",
                    typeof(CommonInformativeNotificationDisplay),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(CommonInformativeNotificationDisplay.Message),
                            "Test message"
                        }
                    },
                    null,
                    true,
                    null);

                dispatcher.Dispatch(new NotificationState.RegisterAction(sampleNotificationRecord));
                dispatcher.Dispatch(new NotificationState.MakeArchivedAction(sampleNotificationRecord.DynamicViewModelKey));
                Assert.Equal(2, notificationStateWrap.Value.ArchivedList.Count);
                Assert.Contains(notificationStateWrap.Value.ArchivedList, x => x == sampleNotificationRecord);
            }

            // Add third item
            {
                var sampleNotificationRecord = new NotificationViewModel(
                    Key<IDynamicViewModel>.NewKey(),
                    "Test title",
                    typeof(CommonInformativeNotificationDisplay),
                    new Dictionary<string, object?>
                    {
                        {
                            nameof(CommonInformativeNotificationDisplay.Message),
                            "Test message"
                        }
                    },
                    null,
                    true,
                    null);

                dispatcher.Dispatch(new NotificationState.RegisterAction(sampleNotificationRecord));
                dispatcher.Dispatch(new NotificationState.MakeArchivedAction(sampleNotificationRecord.DynamicViewModelKey));
                Assert.Equal(3, notificationStateWrap.Value.ArchivedList.Count);
                Assert.Contains(notificationStateWrap.Value.ArchivedList, x => x == sampleNotificationRecord);
            }

            dispatcher.Dispatch(new NotificationState.ClearArchivedAction());
            Assert.Empty(notificationStateWrap.Value.ArchivedList);
        }
    }

    private void InitializeNotificationStateReducerTests(
        out ServiceProvider serviceProvider,
        out IState<NotificationState> notificationStateWrap,
        out IDispatcher dispatcher, 
        out INotification sampleNotificationRecord)
    {
        var services = new ServiceCollection()
            .AddFluxor(options => options.ScanAssemblies(typeof(NotificationState).Assembly));

        serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        notificationStateWrap = serviceProvider.GetRequiredService<IState<NotificationState>>();

        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        sampleNotificationRecord = new NotificationViewModel(
            Key<IDynamicViewModel>.NewKey(),
            "Test title",
            typeof(CommonInformativeNotificationDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(CommonInformativeNotificationDisplay.Message),
                    "Test message"
                }
            },
            null,
            true,
            null);
    }
}