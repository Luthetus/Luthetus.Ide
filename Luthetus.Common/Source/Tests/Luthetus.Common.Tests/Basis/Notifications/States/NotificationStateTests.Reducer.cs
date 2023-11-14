using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Notifications.States;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Common.Tests.Basis.Notifications.States;

/// <summary>
/// <see cref="NotificationState"/>
/// </summary>
public class NotificationStateReducerTests
{
    [Fact]
    public void ReduceRegisterAction()
    {
        /*
        [ReducerMethod]
        public static NotificationState ReduceRegisterAction(
            NotificationState inState, RegisterAction registerAction)
         */

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

    [Fact]
    public void ReduceDisposeAction()
    {
        /*
        [ReducerMethod]
        public static NotificationState ReduceDisposeAction(
            NotificationState inState, DisposeAction disposeAction)
         */

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

    [Fact]
    public void ReduceMakeReadAction()
    {
        /*
        [ReducerMethod]
        public static NotificationState ReduceMakeReadAction(
            NotificationState inState, MakeReadAction makeReadAction)
         */

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

    [Fact]
    public void ReduceUndoMakeReadAction()
    {
        /*
        [ReducerMethod]
        public static NotificationState ReduceUndoMakeReadAction(
            NotificationState inState, UndoMakeReadAction undoMakeReadAction)
         */

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

    [Fact]
    public void ReduceMakeDeletedAction()
    {
        /*
        [ReducerMethod]
        public static NotificationState ReduceMakeDeletedAction(
            NotificationState inState, MakeDeletedAction makeDeletedAction)
         */

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

    [Fact]
    public void ReduceUndoMakeDeletedAction()
    {
        /*
        [ReducerMethod]
        public static NotificationState ReduceUndoMakeDeletedAction(
            NotificationState inState, UndoMakeDeletedAction undoMakeDeletedAction)
         */

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

    [Fact]
    public void ReduceMakeArchivedAction()
    {
        /*
        [ReducerMethod]
        public static NotificationState ReduceMakeArchivedAction(
            NotificationState inState, MakeArchivedAction makeArchivedAction)
         */

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

    [Fact]
    public void ReduceUndoMakeArchivedAction()
    {
        /*
        [ReducerMethod]
        public static NotificationState ReduceUndoMakeArchivedAction(
            NotificationState inState, UndoMakeArchivedAction undoMakeArchivedAction)
         */

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

    [Fact]
    public void ReduceClearDefaultAction()
    {
        /*
        [ReducerMethod(typeof(ClearDefaultAction))]
        public static NotificationState ReduceClearDefaultAction(
            NotificationState inState)
         */

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

    [Fact]
    public void ReduceClearReadAction()
    {
        /*
        [ReducerMethod(typeof(ClearReadAction))]
        public static NotificationState ReduceClearReadAction(
            NotificationState inState)
         */

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

    [Fact]
    public void ReduceClearDeletedAction()
    {
        /*
        [ReducerMethod(typeof(ClearDeletedAction))]
        public static NotificationState ReduceClearDeletedAction(
            NotificationState inState)
         */

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

    [Fact]
    public void ReduceClearArchivedAction()
    {
        /*
        [ReducerMethod(typeof(ClearArchivedAction))]
        public static NotificationState ReduceClearArchivedAction(
            NotificationState inState)
         */

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