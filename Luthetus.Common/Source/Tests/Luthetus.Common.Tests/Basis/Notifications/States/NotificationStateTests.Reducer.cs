using Fluxor;
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

        Assert.Empty(notificationStateWrap.Value.DefaultBag);

        dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));

        Assert.Single(notificationStateWrap.Value.DefaultBag);
        Assert.Contains(notificationStateWrap.Value.DefaultBag, x => x == notificationRecord);
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

        Assert.Single(notificationStateWrap.Value.DefaultBag);
        Assert.Contains(notificationStateWrap.Value.DefaultBag, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.DisposeAction(notificationRecord.Key));
        
        Assert.Empty(notificationStateWrap.Value.DefaultBag);
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

        Assert.Single(notificationStateWrap.Value.DefaultBag);
        Assert.Contains(notificationStateWrap.Value.DefaultBag, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.MakeReadAction(notificationRecord.Key));
        
        Assert.Empty(notificationStateWrap.Value.DefaultBag);

        Assert.Single(notificationStateWrap.Value.ReadBag);
        Assert.Contains(notificationStateWrap.Value.ReadBag, x => x == notificationRecord);
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

        Assert.Single(notificationStateWrap.Value.DefaultBag);
        Assert.Contains(notificationStateWrap.Value.DefaultBag, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.MakeReadAction(notificationRecord.Key));

        Assert.Empty(notificationStateWrap.Value.DefaultBag);

        Assert.Single(notificationStateWrap.Value.ReadBag);
        Assert.Contains(notificationStateWrap.Value.ReadBag, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.UndoMakeReadAction(notificationRecord.Key));

        Assert.Empty(notificationStateWrap.Value.ReadBag);

        Assert.Single(notificationStateWrap.Value.DefaultBag);
        Assert.Contains(notificationStateWrap.Value.DefaultBag, x => x == notificationRecord);
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

        Assert.Single(notificationStateWrap.Value.DefaultBag);
        Assert.Contains(notificationStateWrap.Value.DefaultBag, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.MakeDeletedAction(notificationRecord.Key));

        Assert.Empty(notificationStateWrap.Value.DefaultBag);

        Assert.Single(notificationStateWrap.Value.DeletedBag);
        Assert.Contains(notificationStateWrap.Value.DeletedBag, x => x == notificationRecord);
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

        Assert.Single(notificationStateWrap.Value.DefaultBag);
        Assert.Contains(notificationStateWrap.Value.DefaultBag, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.MakeDeletedAction(notificationRecord.Key));

        Assert.Empty(notificationStateWrap.Value.DefaultBag);

        Assert.Single(notificationStateWrap.Value.DeletedBag);
        Assert.Contains(notificationStateWrap.Value.DeletedBag, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.UndoMakeDeletedAction(notificationRecord.Key));

        Assert.Empty(notificationStateWrap.Value.DeletedBag);

        Assert.Single(notificationStateWrap.Value.DefaultBag);
        Assert.Contains(notificationStateWrap.Value.DefaultBag, x => x == notificationRecord);
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

        Assert.Single(notificationStateWrap.Value.DefaultBag);
        Assert.Contains(notificationStateWrap.Value.DefaultBag, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.MakeArchivedAction(notificationRecord.Key));

        Assert.Empty(notificationStateWrap.Value.DefaultBag);

        Assert.Single(notificationStateWrap.Value.ArchivedBag);
        Assert.Contains(notificationStateWrap.Value.ArchivedBag, x => x == notificationRecord);
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

        Assert.Single(notificationStateWrap.Value.DefaultBag);
        Assert.Contains(notificationStateWrap.Value.DefaultBag, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.MakeArchivedAction(notificationRecord.Key));

        Assert.Empty(notificationStateWrap.Value.DefaultBag);

        Assert.Single(notificationStateWrap.Value.ArchivedBag);
        Assert.Contains(notificationStateWrap.Value.ArchivedBag, x => x == notificationRecord);

        dispatcher.Dispatch(new NotificationState.UndoMakeArchivedAction(notificationRecord.Key));

        Assert.Empty(notificationStateWrap.Value.ArchivedBag);

        Assert.Single(notificationStateWrap.Value.DefaultBag);
        Assert.Contains(notificationStateWrap.Value.DefaultBag, x => x == notificationRecord);
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
            Assert.Empty(notificationStateWrap.Value.DefaultBag);
            
            dispatcher.Dispatch(new NotificationState.ClearDefaultAction());
            Assert.Empty(notificationStateWrap.Value.DefaultBag);
        }

        // 1 item
        {
            Assert.Empty(notificationStateWrap.Value.DefaultBag);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            Assert.Single(notificationStateWrap.Value.DefaultBag);
            Assert.Contains(notificationStateWrap.Value.DefaultBag, x => x == notificationRecord);
            
            dispatcher.Dispatch(new NotificationState.ClearDefaultAction());
            Assert.Empty(notificationStateWrap.Value.DefaultBag);
        }

        // 2 items
        {
            Assert.Empty(notificationStateWrap.Value.DefaultBag);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            Assert.Single(notificationStateWrap.Value.DefaultBag);
            Assert.Contains(notificationStateWrap.Value.DefaultBag, x => x == notificationRecord);

            // Add second item
            {
                var sampleNotificationRecord = new NotificationRecord(
                    Key<NotificationRecord>.NewKey(),
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
                Assert.Equal(2, notificationStateWrap.Value.DefaultBag.Count);
                Assert.Contains(notificationStateWrap.Value.DefaultBag, x => x == sampleNotificationRecord);
            }

            dispatcher.Dispatch(new NotificationState.ClearDefaultAction());
            Assert.Empty(notificationStateWrap.Value.DefaultBag);
        }
        
        // 3 items
        {
            Assert.Empty(notificationStateWrap.Value.DefaultBag);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            Assert.Single(notificationStateWrap.Value.DefaultBag);
            Assert.Contains(notificationStateWrap.Value.DefaultBag, x => x == notificationRecord);

            // Add second item
            {
                var sampleNotificationRecord = new NotificationRecord(
                    Key<NotificationRecord>.NewKey(),
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
                Assert.Equal(2, notificationStateWrap.Value.DefaultBag.Count);
                Assert.Contains(notificationStateWrap.Value.DefaultBag, x => x == sampleNotificationRecord);
            }
            
            // Add third item
            {
                var sampleNotificationRecord = new NotificationRecord(
                    Key<NotificationRecord>.NewKey(),
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
                Assert.Equal(3, notificationStateWrap.Value.DefaultBag.Count);
                Assert.Contains(notificationStateWrap.Value.DefaultBag, x => x == sampleNotificationRecord);
            }

            dispatcher.Dispatch(new NotificationState.ClearDefaultAction());
            Assert.Empty(notificationStateWrap.Value.DefaultBag);
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
            Assert.Empty(notificationStateWrap.Value.ReadBag);

            dispatcher.Dispatch(new NotificationState.ClearReadAction());
            Assert.Empty(notificationStateWrap.Value.ReadBag);
        }

        // 1 item
        {
            Assert.Empty(notificationStateWrap.Value.ReadBag);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeReadAction(notificationRecord.Key));
            Assert.Single(notificationStateWrap.Value.ReadBag);
            Assert.Contains(notificationStateWrap.Value.ReadBag, x => x == notificationRecord);

            dispatcher.Dispatch(new NotificationState.ClearReadAction());
            Assert.Empty(notificationStateWrap.Value.ReadBag);
        }

        // 2 items
        {
            Assert.Empty(notificationStateWrap.Value.ReadBag);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeReadAction(notificationRecord.Key));
            Assert.Single(notificationStateWrap.Value.ReadBag);
            Assert.Contains(notificationStateWrap.Value.ReadBag, x => x == notificationRecord);

            // Add second item
            {
                var sampleNotificationRecord = new NotificationRecord(
                    Key<NotificationRecord>.NewKey(),
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
                dispatcher.Dispatch(new NotificationState.MakeReadAction(sampleNotificationRecord.Key));
                Assert.Equal(2, notificationStateWrap.Value.ReadBag.Count);
                Assert.Contains(notificationStateWrap.Value.ReadBag, x => x == sampleNotificationRecord);
            }

            dispatcher.Dispatch(new NotificationState.ClearReadAction());
            Assert.Empty(notificationStateWrap.Value.ReadBag);
        }

        // 3 items
        {
            Assert.Empty(notificationStateWrap.Value.ReadBag);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeReadAction(notificationRecord.Key));
            Assert.Single(notificationStateWrap.Value.ReadBag);
            Assert.Contains(notificationStateWrap.Value.ReadBag, x => x == notificationRecord);

            // Add second item
            {
                var sampleNotificationRecord = new NotificationRecord(
                    Key<NotificationRecord>.NewKey(),
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
                dispatcher.Dispatch(new NotificationState.MakeReadAction(sampleNotificationRecord.Key));
                Assert.Equal(2, notificationStateWrap.Value.ReadBag.Count);
                Assert.Contains(notificationStateWrap.Value.ReadBag, x => x == sampleNotificationRecord);
            }

            // Add third item
            {
                var sampleNotificationRecord = new NotificationRecord(
                    Key<NotificationRecord>.NewKey(),
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
                dispatcher.Dispatch(new NotificationState.MakeReadAction(sampleNotificationRecord.Key));
                Assert.Equal(3, notificationStateWrap.Value.ReadBag.Count);
                Assert.Contains(notificationStateWrap.Value.ReadBag, x => x == sampleNotificationRecord);
            }

            dispatcher.Dispatch(new NotificationState.ClearReadAction());
            Assert.Empty(notificationStateWrap.Value.ReadBag);
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
            Assert.Empty(notificationStateWrap.Value.DeletedBag);

            dispatcher.Dispatch(new NotificationState.ClearDeletedAction());
            Assert.Empty(notificationStateWrap.Value.DeletedBag);
        }

        // 1 item
        {
            Assert.Empty(notificationStateWrap.Value.DeletedBag);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeDeletedAction(notificationRecord.Key));
            Assert.Single(notificationStateWrap.Value.DeletedBag);
            Assert.Contains(notificationStateWrap.Value.DeletedBag, x => x == notificationRecord);

            dispatcher.Dispatch(new NotificationState.ClearDeletedAction());
            Assert.Empty(notificationStateWrap.Value.DeletedBag);
        }

        // 2 items
        {
            Assert.Empty(notificationStateWrap.Value.DeletedBag);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeDeletedAction(notificationRecord.Key));
            Assert.Single(notificationStateWrap.Value.DeletedBag);
            Assert.Contains(notificationStateWrap.Value.DeletedBag, x => x == notificationRecord);

            // Add second item
            {
                var sampleNotificationRecord = new NotificationRecord(
                    Key<NotificationRecord>.NewKey(),
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
                dispatcher.Dispatch(new NotificationState.MakeDeletedAction(sampleNotificationRecord.Key));
                Assert.Equal(2, notificationStateWrap.Value.DeletedBag.Count);
                Assert.Contains(notificationStateWrap.Value.DeletedBag, x => x == sampleNotificationRecord);
            }

            dispatcher.Dispatch(new NotificationState.ClearDeletedAction());
            Assert.Empty(notificationStateWrap.Value.DeletedBag);
        }

        // 3 items
        {
            Assert.Empty(notificationStateWrap.Value.DeletedBag);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeDeletedAction(notificationRecord.Key));
            Assert.Single(notificationStateWrap.Value.DeletedBag);
            Assert.Contains(notificationStateWrap.Value.DeletedBag, x => x == notificationRecord);

            // Add second item
            {
                var sampleNotificationRecord = new NotificationRecord(
                    Key<NotificationRecord>.NewKey(),
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
                dispatcher.Dispatch(new NotificationState.MakeDeletedAction(sampleNotificationRecord.Key));
                Assert.Equal(2, notificationStateWrap.Value.DeletedBag.Count);
                Assert.Contains(notificationStateWrap.Value.DeletedBag, x => x == sampleNotificationRecord);
            }

            // Add third item
            {
                var sampleNotificationRecord = new NotificationRecord(
                    Key<NotificationRecord>.NewKey(),
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
                dispatcher.Dispatch(new NotificationState.MakeDeletedAction(sampleNotificationRecord.Key));
                Assert.Equal(3, notificationStateWrap.Value.DeletedBag.Count);
                Assert.Contains(notificationStateWrap.Value.DeletedBag, x => x == sampleNotificationRecord);
            }

            dispatcher.Dispatch(new NotificationState.ClearDeletedAction());
            Assert.Empty(notificationStateWrap.Value.DeletedBag);
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
            Assert.Empty(notificationStateWrap.Value.ArchivedBag);

            dispatcher.Dispatch(new NotificationState.ClearArchivedAction());
            Assert.Empty(notificationStateWrap.Value.ArchivedBag);
        }

        // 1 item
        {
            Assert.Empty(notificationStateWrap.Value.ArchivedBag);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeArchivedAction(notificationRecord.Key));
            Assert.Single(notificationStateWrap.Value.ArchivedBag);
            Assert.Contains(notificationStateWrap.Value.ArchivedBag, x => x == notificationRecord);

            dispatcher.Dispatch(new NotificationState.ClearArchivedAction());
            Assert.Empty(notificationStateWrap.Value.ArchivedBag);
        }

        // 2 items
        {
            Assert.Empty(notificationStateWrap.Value.ArchivedBag);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeArchivedAction(notificationRecord.Key));
            Assert.Single(notificationStateWrap.Value.ArchivedBag);
            Assert.Contains(notificationStateWrap.Value.ArchivedBag, x => x == notificationRecord);

            // Add second item
            {
                var sampleNotificationRecord = new NotificationRecord(
                    Key<NotificationRecord>.NewKey(),
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
                dispatcher.Dispatch(new NotificationState.MakeArchivedAction(sampleNotificationRecord.Key));
                Assert.Equal(2, notificationStateWrap.Value.ArchivedBag.Count);
                Assert.Contains(notificationStateWrap.Value.ArchivedBag, x => x == sampleNotificationRecord);
            }

            dispatcher.Dispatch(new NotificationState.ClearArchivedAction());
            Assert.Empty(notificationStateWrap.Value.ArchivedBag);
        }

        // 3 items
        {
            Assert.Empty(notificationStateWrap.Value.ArchivedBag);

            dispatcher.Dispatch(new NotificationState.RegisterAction(notificationRecord));
            dispatcher.Dispatch(new NotificationState.MakeArchivedAction(notificationRecord.Key));
            Assert.Single(notificationStateWrap.Value.ArchivedBag);
            Assert.Contains(notificationStateWrap.Value.ArchivedBag, x => x == notificationRecord);

            // Add second item
            {
                var sampleNotificationRecord = new NotificationRecord(
                    Key<NotificationRecord>.NewKey(),
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
                dispatcher.Dispatch(new NotificationState.MakeArchivedAction(sampleNotificationRecord.Key));
                Assert.Equal(2, notificationStateWrap.Value.ArchivedBag.Count);
                Assert.Contains(notificationStateWrap.Value.ArchivedBag, x => x == sampleNotificationRecord);
            }

            // Add third item
            {
                var sampleNotificationRecord = new NotificationRecord(
                    Key<NotificationRecord>.NewKey(),
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
                dispatcher.Dispatch(new NotificationState.MakeArchivedAction(sampleNotificationRecord.Key));
                Assert.Equal(3, notificationStateWrap.Value.ArchivedBag.Count);
                Assert.Contains(notificationStateWrap.Value.ArchivedBag, x => x == sampleNotificationRecord);
            }

            dispatcher.Dispatch(new NotificationState.ClearArchivedAction());
            Assert.Empty(notificationStateWrap.Value.ArchivedBag);
        }
    }

    private void InitializeNotificationStateReducerTests(
        out ServiceProvider serviceProvider,
        out IState<NotificationState> notificationStateWrap,
        out IDispatcher dispatcher, 
        out NotificationRecord sampleNotificationRecord)
    {
        var services = new ServiceCollection()
            .AddFluxor(options => options.ScanAssemblies(typeof(NotificationState).Assembly));

        serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        notificationStateWrap = serviceProvider.GetRequiredService<IState<NotificationState>>();

        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        sampleNotificationRecord = new NotificationRecord(
            Key<NotificationRecord>.NewKey(),
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