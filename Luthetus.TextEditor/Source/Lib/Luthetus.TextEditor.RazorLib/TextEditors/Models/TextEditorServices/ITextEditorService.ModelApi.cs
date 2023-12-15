using Fluxor;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using static Luthetus.TextEditor.RazorLib.TextEditors.States.TextEditorModelState;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Reflection;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

public partial interface ITextEditorService
{
    public interface ITextEditorModelApi
    {
        #region CREATE_METHODS
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

        public void RegisterPresentationModel(
            ResourceUri resourceUri,
            TextEditorPresentationModel emptyPresentationModel);
        #endregion

        #region READ_METHODS
        /// <summary>
        /// One should store the result of invoking this method in a variable, then reference that variable.
        /// If one continually invokes this, there is no guarantee that the data had not changed
        /// since the previous invocation.
        /// </summary>
        public ImmutableList<TextEditorModel> GetModels();
        public TextEditorModel? GetOrDefault(ResourceUri resourceUri);
        public ImmutableArray<TextEditorViewModel> GetViewModelsOrEmpty(ResourceUri resourceUri);
        public string? GetAllText(ResourceUri resourceUri);
        #endregion

        #region UPDATE_METHODS
        public void DeleteTextByMotion(DeleteTextByMotionAction deleteTextByMotionAction, bool shouldEnqueue = true);
        public void DeleteTextByRange(DeleteTextByRangeAction deleteTextByRangeAction, bool shouldEnqueue = true);
        public void HandleKeyboardEvent(KeyboardEventAction keyboardEventAction, bool shouldEnqueue = true);
        public void InsertText(InsertTextAction insertTextAction, bool shouldEnqueue = true);
        public void RedoEdit(ResourceUri resourceUri, bool shouldEnqueue = true);

        public void Reload(
            ResourceUri resourceUri,
            string content,
            DateTime resourceLastWriteTime,
            bool shouldEnqueue = true);

        public void SetResourceData(
            ResourceUri resourceUri,
            DateTime resourceLastWriteTime,
            bool shouldEnqueue = true);

        public void SetUsingRowEndingKind(
            ResourceUri resourceUri,
            RowEndingKind rowEndingKind,
            bool shouldEnqueue = true);

        public void UndoEdit(ResourceUri resourceUri, bool shouldEnqueue = true);
        #endregion

        #region DELETE_METHODS
        public void Dispose(ResourceUri resourceUri);
        #endregion
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

        #region CREATE_METHODS
        public void RegisterCustom(TextEditorModel model)
        {
            var commandArgs = new TextEditorCommandArgs(
                model.ResourceUri, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(RegisterCustom),
                commandArgs,
                (_, _, _, _, _) =>
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
            var commandArgs = new TextEditorCommandArgs(
                resourceUri, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(RegisterTemplated),
                commandArgs,
                (_, _, _, _, _) =>
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

        public void RegisterPresentationModel(
            ResourceUri resourceUri,
            TextEditorPresentationModel emptyPresentationModel)
        {
            var commandArgs = new TextEditorCommandArgs(
                resourceUri, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(RegisterPresentationModel),
                commandArgs,
                (_, _, _, _, _) =>
                {
                    _dispatcher.Dispatch(new RegisterPresentationModelAction(
                        resourceUri,
                        emptyPresentationModel));

                    return Task.CompletedTask;
                });
        }
        #endregion

        #region READ_METHODS
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
        #endregion

        #region UPDATE_METHODS
        public void UndoEdit(ResourceUri resourceUri, bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                resourceUri, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = (_, _, _, _, _) =>
            {
                _dispatcher.Dispatch(new UndoEditAction(resourceUri));
                return Task.CompletedTask;
            };

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(UndoEdit),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(UndoEdit),
                    commandArgs,
                    modificationTask).Wait();
            }
        }

        public void SetUsingRowEndingKind(
            ResourceUri resourceUri,
            RowEndingKind rowEndingKind,
            bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                resourceUri, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = (_, _, _, _, _) =>
            {
                _dispatcher.Dispatch(new SetUsingRowEndingKindAction(
                    resourceUri,
                    rowEndingKind));

                return Task.CompletedTask;
            };

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(SetUsingRowEndingKind),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(SetUsingRowEndingKind),
                    commandArgs,
                    modificationTask).Wait();
            }
        }

        public void SetResourceData(
            ResourceUri resourceUri,
            DateTime resourceLastWriteTime,
            bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                resourceUri, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = (_, _, _, _, _) =>
            {
                _dispatcher.Dispatch(new SetResourceDataAction(
                    resourceUri,
                    resourceLastWriteTime));

                return Task.CompletedTask;
            };

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(SetResourceData),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(SetResourceData),
                    commandArgs,
                    modificationTask).Wait();
            }
        }

        public void Reload(
            ResourceUri resourceUri,
            string content,
            DateTime resourceLastWriteTime,
            bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                resourceUri, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = (_, _, _, _, _) =>
            {
                _dispatcher.Dispatch(new ReloadAction(
                    resourceUri,
                    content,
                    resourceLastWriteTime));

                return Task.CompletedTask;
            };

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(Reload),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(Reload),
                    commandArgs,
                    modificationTask).Wait();
            }
        }

        public void RedoEdit(ResourceUri resourceUri, bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                resourceUri, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = (_, _, _, _, _) =>
            {
                _dispatcher.Dispatch(new RedoEditAction(resourceUri));
                return Task.CompletedTask;
            };

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(RedoEdit),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(RedoEdit),
                    commandArgs,
                    modificationTask).Wait();
            }
        }

        public void InsertText(InsertTextAction insertTextAction, bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                insertTextAction.ResourceUri, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = (_, _, _, _, _) =>
            {
                var cursorBag = insertTextAction.CursorModifierBag;

                if (insertTextAction.ViewModelKey is not null)
                {
                    var viewModel = _textEditorService.ViewModelApi.GetOrDefault(
                        insertTextAction.ViewModelKey.Value);

                    if (viewModel is not null)
                        cursorBag = viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList();
                }

                insertTextAction = insertTextAction with
                {
                    CursorModifierBag = cursorBag,
                };

                _dispatcher.Dispatch(insertTextAction);

                if (insertTextAction.ViewModelKey is not null)
                {
                    _textEditorService.ViewModelApi.WithAsync(
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
            };

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(InsertText),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(InsertText),
                    commandArgs,
                    modificationTask).Wait();
            }
        }

        public void HandleKeyboardEvent(KeyboardEventAction keyboardEventAction, bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                keyboardEventAction.ResourceUri, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = (_, _, _, _, _) =>
            {
                var cursorBag = keyboardEventAction.CursorModifierBag;

                if (keyboardEventAction.ViewModelKey is not null)
                {
                    var viewModel = _textEditorService.ViewModelApi.GetOrDefault(
                        keyboardEventAction.ViewModelKey.Value);

                    if (viewModel is not null)
                        cursorBag = viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList();
                }

                keyboardEventAction = keyboardEventAction with
                {
                    CursorModifierBag = cursorBag,
                };

                _dispatcher.Dispatch(keyboardEventAction);

                if (keyboardEventAction.ViewModelKey is not null)
                {
                    _textEditorService.ViewModelApi.WithAsync(
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
            };

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(HandleKeyboardEvent),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(HandleKeyboardEvent),
                    commandArgs,
                    modificationTask).Wait();
            }
        }

        public void DeleteTextByRange(DeleteTextByRangeAction deleteTextByRangeAction, bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                deleteTextByRangeAction.ResourceUri, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = (_, _, _, _, _) =>
            {
                var cursorBag = deleteTextByRangeAction.CursorModifierBag;

                if (deleteTextByRangeAction.ViewModelKey is not null)
                {
                    var viewModel = _textEditorService.ViewModelApi.GetOrDefault(
                        deleteTextByRangeAction.ViewModelKey.Value);

                    if (viewModel is not null)
                        cursorBag = viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList();
                }

                deleteTextByRangeAction = deleteTextByRangeAction with
                {
                    CursorModifierBag = cursorBag,
                };

                _dispatcher.Dispatch(deleteTextByRangeAction);

                if (deleteTextByRangeAction.ViewModelKey is not null)
                {
                    _textEditorService.ViewModelApi.WithAsync(
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
            };

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(DeleteTextByRange),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(DeleteTextByRange),
                    commandArgs,
                    modificationTask).Wait();
            }
        }

        public void DeleteTextByMotion(DeleteTextByMotionAction deleteTextByMotionAction, bool shouldEnqueue = true)
        {
            var commandArgs = new TextEditorCommandArgs(
                deleteTextByMotionAction.ResourceUri, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            TextEditorCommand.ModificationTask modificationTask = (_, _, _, _, _) =>
            {
                var cursorBag = deleteTextByMotionAction.CursorModifierBag;

                if (deleteTextByMotionAction.ViewModelKey is not null)
                {
                    var viewModel = _textEditorService.ViewModelApi.GetOrDefault(
                        deleteTextByMotionAction.ViewModelKey.Value);

                    if (viewModel is not null)
                        cursorBag = viewModel.CursorBag.Select(x => new TextEditorCursorModifier(x)).ToList();
                }

                deleteTextByMotionAction = deleteTextByMotionAction with
                {
                    CursorModifierBag = cursorBag,
                };

                _dispatcher.Dispatch(deleteTextByMotionAction);

                if (deleteTextByMotionAction.ViewModelKey is not null)
                {
                    _textEditorService.ViewModelApi.WithAsync(
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
            };

            if (shouldEnqueue)
            {
                _textEditorService.EnqueueModification(
                    nameof(DeleteTextByMotion),
                    commandArgs,
                    modificationTask);
            }
            else
            {
                // TODO: await this
                _textEditorService.ModifyAsync(
                    nameof(DeleteTextByMotion),
                    commandArgs,
                    modificationTask).Wait();
            }
        }
        #endregion

        #region DELETE_METHODS
        public void Dispose(ResourceUri resourceUri)
        {
            _dispatcher.Dispatch(new DisposeAction(resourceUri));
        }
        #endregion
    }
}