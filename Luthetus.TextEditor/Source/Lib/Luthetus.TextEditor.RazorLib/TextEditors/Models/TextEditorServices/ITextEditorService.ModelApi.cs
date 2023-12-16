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
using static Luthetus.TextEditor.RazorLib.Commands.Models.TextEditorCommand;
using static Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorService;

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
        /// <summary>
        /// To ensure text editor modifications are synced, use <see cref="DeleteTextByMotionEnqueue"/> instead.
        /// </summary>
        public ModificationTask DeleteTextByMotionAsync(DeleteTextByMotionAction deleteTextByMotionAction);
        /// <summary>
        /// To ensure text editor modifications are synced, use <see cref="DeleteTextByRangeEnqueue"/> instead.
        /// </summary>
        public ModificationTask DeleteTextByRangeAsync(DeleteTextByRangeAction deleteTextByRangeAction);
        /// <summary>
        /// To ensure text editor modifications are synced, use <see cref="HandleKeyboardEventEnqueue"/> instead.
        /// </summary>
        public ModificationTask HandleKeyboardEventAsync(KeyboardEventAction keyboardEventAction);
        /// <summary>
        /// To ensure text editor modifications are synced, use <see cref="InsertTextEnqueue"/> instead.
        /// </summary>
        public ModificationTask InsertTextAsync(InsertTextAction insertTextAction);
        /// <summary>
        /// To ensure text editor modifications are synced, use <see cref="RedoEditEnqueue"/> instead.
        /// </summary>
        public ModificationTask RedoEditAsync(ResourceUri resourceUri);
        /// <summary>
        /// To ensure text editor modifications are synced, use <see cref="ReloadEnqueue"/> instead.
        /// </summary>
        public ModificationTask ReloadAsync(ResourceUri resourceUri, string content, DateTime resourceLastWriteTime);
        /// <summary>
        /// To ensure text editor modifications are synced, use <see cref="SetResourceDataEnqueue"/> instead.
        /// </summary>
        public ModificationTask SetResourceDataAsync(ResourceUri resourceUri, DateTime resourceLastWriteTime);
        /// <summary>
        /// To ensure text editor modifications are synced, use <see cref="SetUsingRowEndingKindEnqueue"/> instead.
        /// </summary>
        public ModificationTask SetUsingRowEndingKindAsync(ResourceUri resourceUri, RowEndingKind rowEndingKind);
        /// <summary>
        /// To ensure text editor modifications are synced, use <see cref="UndoEditEnqueue"/> instead.
        /// </summary>
        public ModificationTask UndoEditAsync(ResourceUri resourceUri);

        /// <summary>
        /// Enqueue a background task that invokes <see cref="DeleteTextByMotionAsync"/>
        /// </summary>
        public void DeleteTextByMotionEnqueue(DeleteTextByMotionAction deleteTextByMotionAction);
        /// <summary>
        /// Enqueue a background task that invokes <see cref="DeleteTextByRangeAsync"/>
        /// </summary>
        public void DeleteTextByRangeEnqueue(DeleteTextByRangeAction deleteTextByRangeAction);
        /// <summary>
        /// Enqueue a background task that invokes <see cref="HandleKeyboardEventAsync"/>
        /// </summary>
        public void HandleKeyboardEventEnqueue(KeyboardEventAction keyboardEventAction);
        /// <summary>
        /// Enqueue a background task that invokes <see cref="InsertTextAsync"/>
        /// </summary>
        public void InsertTextEnqueue(InsertTextAction insertTextAction);
        /// <summary>
        /// Enqueue a background task that invokes <see cref="RedoEditAsync"/>
        /// </summary>
        public void RedoEditEnqueue(ResourceUri resourceUri);
        /// <summary>
        /// Enqueue a background task that invokes <see cref="ReloadAsync"/>
        /// </summary>
        public void ReloadEnqueue(ResourceUri resourceUri, string content, DateTime resourceLastWriteTime);
        /// <summary>
        /// Enqueue a background task that invokes <see cref="SetResourceDataAsync"/>
        /// </summary>
        public void SetResourceDataEnqueue(ResourceUri resourceUri, DateTime resourceLastWriteTime);
        /// <summary>
        /// Enqueue a background task that invokes <see cref="SetUsingRowEndingKindAsync"/>
        /// </summary>
        public void SetUsingRowEndingKindEnqueue(ResourceUri resourceUri, RowEndingKind rowEndingKind);
        /// <summary>
        /// Enqueue a background task that invokes <see cref="UndoEditAsync"/>
        /// </summary>
        public void UndoEditEnqueue(ResourceUri resourceUri);
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
            _dispatcher.Dispatch(new RegisterAction(model));

            _ = Task.Run(async () =>
            {
                try
                {
                    await model.ApplySyntaxHighlightingAsync();
                    _dispatcher.Dispatch(new ForceRerenderAction(model.ResourceUri));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }, CancellationToken.None);
        }

        public void RegisterTemplated(
            string extensionNoPeriod,
            ResourceUri resourceUri,
            DateTime resourceLastWriteTime,
            string initialContent,
            string? overrideDisplayTextForFileExtension = null)
        {
            var textEditorModel = new TextEditorModel(
                resourceUri,
                resourceLastWriteTime,
                overrideDisplayTextForFileExtension ?? extensionNoPeriod,
                initialContent,
                _decorationMapperRegistry.GetDecorationMapper(extensionNoPeriod),
                _compilerServiceRegistry.GetCompilerService(extensionNoPeriod));
            
            _dispatcher.Dispatch(new RegisterAction(textEditorModel));

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
        }

        public void RegisterPresentationModel(
            ResourceUri resourceUri,
            TextEditorPresentationModel emptyPresentationModel)
        {
            _dispatcher.Dispatch(new RegisterPresentationModelAction(
                resourceUri,
                emptyPresentationModel));
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
        public ModificationTask UndoEditAsync(ResourceUri resourceUri) =>
            (_, _, _, _, _) =>
            {
                _dispatcher.Dispatch(new UndoEditAction(resourceUri));
                return Task.CompletedTask;
            };

        public void UndoEditEnqueue(ResourceUri resourceUri)
        {
            var commandArgs = new TextEditorCommandArgs(
                resourceUri, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(UndoEditEnqueue),
                commandArgs,
                UndoEditAsync(resourceUri));
        }

        public ModificationTask SetUsingRowEndingKindAsync(ResourceUri resourceUri, RowEndingKind rowEndingKind) =>
            (_, _, _, _, _) =>
            {
                _dispatcher.Dispatch(new SetUsingRowEndingKindAction(
                    resourceUri,
                    rowEndingKind));

                return Task.CompletedTask;
            };

        public void SetUsingRowEndingKindEnqueue(ResourceUri resourceUri, RowEndingKind rowEndingKind)
        {
            var commandArgs = new TextEditorCommandArgs(
                resourceUri, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(SetUsingRowEndingKindEnqueue),
                commandArgs,
                SetUsingRowEndingKindAsync(resourceUri, rowEndingKind));
        }

        public ModificationTask SetResourceDataAsync(ResourceUri resourceUri, DateTime resourceLastWriteTime) =>
            (_, _, _, _, _) =>
            {
                _dispatcher.Dispatch(new SetResourceDataAction(
                    resourceUri,
                    resourceLastWriteTime));

                return Task.CompletedTask;
            };

        public void SetResourceDataEnqueue(ResourceUri resourceUri, DateTime resourceLastWriteTime)
        {
            var commandArgs = new TextEditorCommandArgs(
                resourceUri, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(SetResourceDataEnqueue),
                commandArgs,
                SetResourceDataAsync(resourceUri, resourceLastWriteTime));
        }

        public ModificationTask ReloadAsync(ResourceUri resourceUri, string content, DateTime resourceLastWriteTime) =>
            (_, _, _, _, _) =>
            {
                _dispatcher.Dispatch(new ReloadAction(
                    resourceUri,
                    content,
                    resourceLastWriteTime));

                return Task.CompletedTask;
            };

        public void ReloadEnqueue(ResourceUri resourceUri, string content, DateTime resourceLastWriteTime)
        {
            var commandArgs = new TextEditorCommandArgs(
                resourceUri, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(ReloadEnqueue),
                commandArgs,
                ReloadAsync(resourceUri, content, resourceLastWriteTime));
        }

        public ModificationTask RedoEditAsync(ResourceUri resourceUri) =>
            (_, _, _, _, _) =>
            {
                _dispatcher.Dispatch(new RedoEditAction(resourceUri));
                return Task.CompletedTask;
            };

        public void RedoEditEnqueue(ResourceUri resourceUri)
        {
            var commandArgs = new TextEditorCommandArgs(
                resourceUri, Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(RedoEditEnqueue),
                commandArgs,
                RedoEditAsync(resourceUri));
        }

        public ModificationTask InsertTextAsync(InsertTextAction insertTextAction) =>
            (_, _, viewModel, refreshCursorsRequest, primaryCursor) =>
            {
                var cursorBag = refreshCursorsRequest?.CursorBag ?? insertTextAction.CursorModifierBag;

                insertTextAction = insertTextAction with
                {
                    CursorModifierBag = cursorBag,
                };

                _dispatcher.Dispatch(insertTextAction);

                return Task.CompletedTask;
            };
        public void InsertTextEnqueue(InsertTextAction insertTextAction)
        {
            var commandArgs = new TextEditorCommandArgs(
                insertTextAction.ResourceUri, insertTextAction.ViewModelKey ?? Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(InsertTextEnqueue),
                commandArgs,
                InsertTextAsync(insertTextAction));
        }

        public ModificationTask HandleKeyboardEventAsync(KeyboardEventAction keyboardEventAction) =>
            (_, _, _, refreshCursorsRequest, _) =>
            {
                var cursorBag = refreshCursorsRequest?.CursorBag ?? keyboardEventAction.CursorModifierBag;

                keyboardEventAction = keyboardEventAction with
                {
                    CursorModifierBag = cursorBag,
                };

                _dispatcher.Dispatch(keyboardEventAction);

                return Task.CompletedTask;
            };

        public void HandleKeyboardEventEnqueue(KeyboardEventAction keyboardEventAction)
        {
            var commandArgs = new TextEditorCommandArgs(
                keyboardEventAction.ResourceUri, keyboardEventAction.ViewModelKey ?? Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(HandleKeyboardEventEnqueue),
                commandArgs,
                HandleKeyboardEventAsync(keyboardEventAction));
        }

        public ModificationTask DeleteTextByRangeAsync(DeleteTextByRangeAction deleteTextByRangeAction) =>
            (_, _, _, refreshCursorsRequest, _) =>
            {
                var cursorBag = refreshCursorsRequest?.CursorBag ?? deleteTextByRangeAction.CursorModifierBag;

                deleteTextByRangeAction = deleteTextByRangeAction with
                {
                    CursorModifierBag = cursorBag,
                };

                _dispatcher.Dispatch(deleteTextByRangeAction);

                return Task.CompletedTask;
            };

        public void DeleteTextByRangeEnqueue(DeleteTextByRangeAction deleteTextByRangeAction)
        {
            var commandArgs = new TextEditorCommandArgs(
                deleteTextByRangeAction.ResourceUri, deleteTextByRangeAction.ViewModelKey ?? Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(DeleteTextByRangeEnqueue),
                commandArgs,
                DeleteTextByRangeAsync(deleteTextByRangeAction));
        }

        public ModificationTask DeleteTextByMotionAsync(DeleteTextByMotionAction deleteTextByMotionAction) =>
            (_, _, _, refreshCursorsRequest, _) =>
            {
                var cursorBag = refreshCursorsRequest?.CursorBag ?? deleteTextByMotionAction.CursorModifierBag;

                deleteTextByMotionAction = deleteTextByMotionAction with
                {
                    CursorModifierBag = cursorBag,
                };

                _dispatcher.Dispatch(deleteTextByMotionAction);

                return Task.CompletedTask;
            };

        public void DeleteTextByMotionEnqueue(DeleteTextByMotionAction deleteTextByMotionAction)
        {
            var commandArgs = new TextEditorCommandArgs(
                deleteTextByMotionAction.ResourceUri, deleteTextByMotionAction.ViewModelKey ?? Key<TextEditorViewModel>.Empty, false, null,
                _textEditorService, null, null, null, null, null, null);

            _textEditorService.EnqueueModification(
                nameof(DeleteTextByMotionEnqueue),
                commandArgs,
                DeleteTextByMotionAsync(deleteTextByMotionAction));
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