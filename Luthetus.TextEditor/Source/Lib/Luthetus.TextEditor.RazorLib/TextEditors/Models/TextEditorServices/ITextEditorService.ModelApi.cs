using Fluxor;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using static Luthetus.TextEditor.RazorLib.TextEditors.States.TextEditorModelState;
using System.Reflection;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

public partial interface ITextEditorService
{
    public interface ITextEditorModelApi
    {
        public void DeleteTextByMotion(DeleteTextByMotionAction deleteTextByMotionAction);
        public void DeleteTextByRange(DeleteTextByRangeAction deleteTextByRangeAction);
        public void Dispose(ResourceUri resourceUri);
        public TextEditorModel? GetOrDefault(ResourceUri resourceUri);
        public string? GetAllText(ResourceUri resourceUri);
        public ImmutableArray<TextEditorViewModel> GetViewModelsOrEmpty(ResourceUri resourceUri);
        public void HandleKeyboardEvent(KeyboardEventAction keyboardEventAction);
        public void InsertText(InsertTextAction insertTextAction);
        public void RedoEdit(ResourceUri resourceUri);
        /// <summary>It is recommended to use the <see cref="RegisterTemplated" /> method as it will internally reference the <see cref="ITextEditorLexer" /> and <see cref="IDecorationMapper" /> that correspond to the desired text editor.</summary>
        public void RegisterCustom(TextEditorModel model);
        /// <summary>
        /// Plain Text Editor: one would pass in an <see cref="extensionNoPeriod"/>
        /// of "txt" or the constant varible: <see cref="ExtensionNoPeriodFacts.TXT"/>.<br /><br />
        /// 
        /// C# Text Editor: one would pass in an <see cref="extensionNoPeriod"/>
        /// of "cs" or the constant varible: <see cref="ExtensionNoPeriodFacts.C_SHARP_CLASS"/>;
        /// NOTE: One must first install the Luthetus.CompilerServices.CSharp NuGet package.<br /><br />
        /// </summary>
        public void RegisterTemplated(
            string extensionNoPeriod,
            ResourceUri resourceUri,
            DateTime resourceLastWriteTime,
            string initialContent,
            string? overrideDisplayTextForFileExtension = null);

        public void Reload(ResourceUri resourceUri, string content, DateTime resourceLastWriteTime);

        public void SetResourceData(
            ResourceUri resourceUri,
            DateTime resourceLastWriteTime);

        public void SetUsingRowEndingKind(ResourceUri resourceUri, RowEndingKind rowEndingKind);
        public void UndoEdit(ResourceUri resourceUri);
        public void RegisterPresentationModel(ResourceUri resourceUri, TextEditorPresentationModel emptyPresentationModel);

        /// <summary>
        /// One should store the result of invoking this method in a variable, then reference that variable.
        /// If one continually invokes this, there is no guarantee that the data had not changed
        /// since the previous invocation.
        /// </summary>
        public ImmutableList<TextEditorModel> GetModels();
    }

    public class TextEditorModelApi : ITextEditorModelApi
    {
        private readonly ITextEditorService _textEditorService;
        private readonly IDecorationMapperRegistry _decorationMapperRegistry;
        private readonly ICompilerServiceRegistry _compilerServiceRegistry;
        private readonly IBackgroundTaskService _backgroundTaskService;
        private readonly IDispatcher _dispatcher;

        public TextEditorModelApi(
            ITextEditorService textEditorService,
            IDecorationMapperRegistry decorationMapperRegistry,
            ICompilerServiceRegistry compilerServiceRegistry,
            IBackgroundTaskService backgroundTaskService,
            IDispatcher dispatcher)
        {
            _textEditorService = textEditorService;
            _decorationMapperRegistry = decorationMapperRegistry;
            _compilerServiceRegistry = compilerServiceRegistry;
            _backgroundTaskService = backgroundTaskService;
            _dispatcher = dispatcher;
        }

        public void UndoEdit(ResourceUri resourceUri)
        {
            _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(),
                ContinuousBackgroundTaskWorker.GetQueueKey(),
                nameof(UndoEdit),
                () =>
                {
                    _dispatcher.Dispatch(new UndoEditAction(resourceUri));
                    return Task.CompletedTask;
                });
        }

        public void SetUsingRowEndingKind(ResourceUri resourceUri, RowEndingKind rowEndingKind)
        {
            _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(),
                ContinuousBackgroundTaskWorker.GetQueueKey(),
                nameof(SetUsingRowEndingKind),
                () =>
                {
                    _dispatcher.Dispatch(new SetUsingRowEndingKindAction(
                        resourceUri,
                        rowEndingKind));

                    return Task.CompletedTask;
                });
        }

        public void SetResourceData(
            ResourceUri resourceUri,
            DateTime resourceLastWriteTime)
        {
            _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(),
                ContinuousBackgroundTaskWorker.GetQueueKey(),
                nameof(SetResourceData),
                () =>
                {
                    _dispatcher.Dispatch(new SetResourceDataAction(
                        resourceUri,
                        resourceLastWriteTime));

                    return Task.CompletedTask;
                });
        }

        public void Reload(ResourceUri resourceUri, string content, DateTime resourceLastWriteTime)
        {
            _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(),
                ContinuousBackgroundTaskWorker.GetQueueKey(),
                nameof(Reload),
                () =>
                {
                    _dispatcher.Dispatch(new ReloadAction(
                        resourceUri,
                        content,
                        resourceLastWriteTime));

                    return Task.CompletedTask;
                });
        }

        public void RegisterCustom(TextEditorModel model)
        {
            _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(),
                ContinuousBackgroundTaskWorker.GetQueueKey(),
                nameof(RegisterCustom),
                () =>
                {
                    _dispatcher.Dispatch(new RegisterAction(model));
                    return Task.CompletedTask;
                });
        }

        public void RegisterTemplated(
            string extensionNoPeriod,
            ResourceUri resourceUri,
            DateTime resourceLastWriteTime,
            string initialContent,
            string? overrideDisplayTextForFileExtension = null)
        {
            _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(),
                ContinuousBackgroundTaskWorker.GetQueueKey(),
                nameof(RegisterTemplated),
                () =>
                {
                    var textEditorModel = new TextEditorModel(
                        resourceUri,
                        resourceLastWriteTime,
                        overrideDisplayTextForFileExtension ?? extensionNoPeriod,
                        initialContent,
                        _decorationMapperRegistry.GetDecorationMapper(extensionNoPeriod),
                        _compilerServiceRegistry.GetCompilerService(extensionNoPeriod));

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await textEditorModel.ApplySyntaxHighlightingAsync();
                            _dispatcher.Dispatch(new ForceRerenderAction(textEditorModel.ResourceUri));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }, CancellationToken.None);

                    _dispatcher.Dispatch(new RegisterAction(textEditorModel));

                    return Task.CompletedTask;
                });
        }

        public void RedoEdit(ResourceUri resourceUri)
        {
            _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(),
                ContinuousBackgroundTaskWorker.GetQueueKey(),
                nameof(RedoEdit),
                () =>
                {
                    _dispatcher.Dispatch(new RedoEditAction(resourceUri));
                    return Task.CompletedTask;
                });
        }

        public void InsertText(InsertTextAction insertTextAction)
        {
            _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(),
                ContinuousBackgroundTaskWorker.GetQueueKey(),
                nameof(InsertText),
                () =>
                {
                    var cursorBag = insertTextAction.CursorBag;

                    if (insertTextAction.ViewModelKey is not null)
                    {
                        var viewModel = _textEditorService.ViewModelApi.GetOrDefault(
                            insertTextAction.ViewModelKey.Value);

                        if (viewModel is not null)
                            cursorBag = viewModel.CursorBag;
                    }

                    insertTextAction = insertTextAction with
                    {
                        CursorBag = cursorBag,
                        CursorModifierBag = cursorBag.Select(x => new TextEditorCursorModifier(x)).ToImmutableArray(),
                    };

                    _dispatcher.Dispatch(insertTextAction);

                    if (insertTextAction.ViewModelKey is not null)
                    {
                        _textEditorService.ViewModelApi.SetViewModelWith(
                                insertTextAction.ViewModelKey.Value,
                                inViewModel =>
                                {
                                    var outCursorBag = new List<TextEditorCursor>();

                                    foreach (var cursorModifier in insertTextAction.CursorModifierBag)
                                    {
                                        outCursorBag.Add(cursorModifier.ToCursor());
                                    }

                                    return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(
                                        state => state with
                                        {
                                            CursorBag = outCursorBag.ToImmutableArray()
                                        }));
                                });
                    }

                    return Task.CompletedTask;
                });
        }

        public void HandleKeyboardEvent(KeyboardEventAction keyboardEventAction)
        {
            _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(),
                ContinuousBackgroundTaskWorker.GetQueueKey(),
                nameof(HandleKeyboardEvent),
                () =>
                {
                    var cursorBag = keyboardEventAction.CursorBag;

                    if (keyboardEventAction.ViewModelKey is not null)
                    {
                        var viewModel = _textEditorService.ViewModelApi.GetOrDefault(
                            keyboardEventAction.ViewModelKey.Value);

                        if (viewModel is not null)
                            cursorBag = viewModel.CursorBag;
                    }

                    keyboardEventAction = keyboardEventAction with
                    {
                        CursorBag = cursorBag,
                        CursorModifierBag = cursorBag.Select(x => new TextEditorCursorModifier(x)).ToImmutableArray(),
                    };

                    _dispatcher.Dispatch(keyboardEventAction);

                    if (keyboardEventAction.ViewModelKey is not null)
                    {
                        _textEditorService.ViewModelApi.SetViewModelWith(
                            keyboardEventAction.ViewModelKey.Value,
                            inViewModel =>
                            {
                                var outCursorBag = new List<TextEditorCursor>();

                                foreach (var cursorModifier in keyboardEventAction.CursorModifierBag)
                                {
                                    outCursorBag.Add(cursorModifier.ToCursor());
                                }

                                return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(
                                    state => state with
                                    {
                                        CursorBag = outCursorBag.ToImmutableArray()
                                    }));
                            });
                    }

                    return Task.CompletedTask;
                });
        }

        public void DeleteTextByRange(DeleteTextByRangeAction deleteTextByRangeAction)
        {
            _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(),
                ContinuousBackgroundTaskWorker.GetQueueKey(),
                nameof(DeleteTextByRange),
                () =>
                {
                    var cursorBag = deleteTextByRangeAction.CursorBag;

                    if (deleteTextByRangeAction.ViewModelKey is not null)
                    {
                        var viewModel = _textEditorService.ViewModelApi.GetOrDefault(
                            deleteTextByRangeAction.ViewModelKey.Value);

                        if (viewModel is not null)
                            cursorBag = viewModel.CursorBag;
                    }

                    deleteTextByRangeAction = deleteTextByRangeAction with
                    {
                        CursorBag = cursorBag,
                        CursorModifierBag = cursorBag.Select(x => new TextEditorCursorModifier(x)).ToImmutableArray(),
                    };

                    _dispatcher.Dispatch(deleteTextByRangeAction);

                    if (deleteTextByRangeAction.ViewModelKey is not null)
                    {
                        _textEditorService.ViewModelApi.SetViewModelWith(
                            deleteTextByRangeAction.ViewModelKey.Value,
                            inViewModel =>
                            {
                                var outCursorBag = new List<TextEditorCursor>();

                                foreach (var cursorModifier in deleteTextByRangeAction.CursorModifierBag)
                                {
                                    outCursorBag.Add(cursorModifier.ToCursor());
                                }

                                return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(
                                    state => state with
                                    {
                                        CursorBag = outCursorBag.ToImmutableArray()
                                    }));
                            });
                    }

                    return Task.CompletedTask;
                });
        }

        public void DeleteTextByMotion(DeleteTextByMotionAction deleteTextByMotionAction)
        {
            _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(),
                ContinuousBackgroundTaskWorker.GetQueueKey(),
                nameof(DeleteTextByRange),
                () =>
                {
                    var cursorBag = deleteTextByMotionAction.CursorBag;

                    if (deleteTextByMotionAction.ViewModelKey is not null)
                    {
                        var viewModel = _textEditorService.ViewModelApi.GetOrDefault(
                            deleteTextByMotionAction.ViewModelKey.Value);

                        if (viewModel is not null)
                            cursorBag = viewModel.CursorBag;
                    }

                    deleteTextByMotionAction = deleteTextByMotionAction with
                    {
                        CursorBag = cursorBag,
                        CursorModifierBag = cursorBag.Select(x => new TextEditorCursorModifier(x)).ToImmutableArray(),
                    };

                    _dispatcher.Dispatch(deleteTextByMotionAction);

                    if (deleteTextByMotionAction.ViewModelKey is not null)
                    {
                        _textEditorService.ViewModelApi.SetViewModelWith(
                            deleteTextByMotionAction.ViewModelKey.Value,
                            inViewModel =>
                            {
                                var outCursorBag = new List<TextEditorCursor>();

                                foreach (var cursorModifier in deleteTextByMotionAction.CursorModifierBag)
                                {
                                    outCursorBag.Add(cursorModifier.ToCursor());
                                }

                                return Task.FromResult(new Func<TextEditorViewModel, TextEditorViewModel>(
                                    state => state with
                                    {
                                        CursorBag = outCursorBag.ToImmutableArray()
                                    }));
                            });
                    }

                    return Task.CompletedTask;
                });
        }

        public void RegisterPresentationModel(ResourceUri resourceUri, TextEditorPresentationModel emptyPresentationModel)
        {
            _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(),
                ContinuousBackgroundTaskWorker.GetQueueKey(),
                nameof(RegisterPresentationModel),
                () =>
                {
                    _dispatcher.Dispatch(new RegisterPresentationModelAction(
                        resourceUri,
                        emptyPresentationModel));

                    return Task.CompletedTask;
                });
        }

        public ImmutableArray<TextEditorViewModel> GetViewModelsOrEmpty(ResourceUri resourceUri)
        {
            return _textEditorService.ViewModelStateWrap.Value.ViewModelBag
                .Where(x => x.ResourceUri == resourceUri)
                .ToImmutableArray();
        }

        public string? GetAllText(ResourceUri resourceUri)
        {
            return _textEditorService.ModelStateWrap.Value.ModelBag
                .FirstOrDefault(x => x.ResourceUri == resourceUri)
                ?.GetAllText();
        }

        public TextEditorModel? GetOrDefault(ResourceUri resourceUri)
        {
            return _textEditorService.ModelStateWrap.Value.ModelBag
                .FirstOrDefault(x => x.ResourceUri == resourceUri);
        }

        public ImmutableList<TextEditorModel> GetModels()
        {
            return _textEditorService.ModelStateWrap.Value.ModelBag;
        }

        public void Dispose(ResourceUri resourceUri)
        {
            _dispatcher.Dispatch(new DisposeAction(resourceUri));
        }
    }
}