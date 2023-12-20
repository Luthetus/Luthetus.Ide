using Fluxor;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using static Luthetus.TextEditor.RazorLib.TextEditors.States.TextEditorModelState;
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
        public TextEditorEdit UndoEdit(
            ResourceUri resourceUri);

        public TextEditorEdit SetUsingRowEndingKind(
            ResourceUri resourceUri,
            RowEndingKind rowEndingKind);

        public TextEditorEdit SetResourceData(
            ResourceUri resourceUri,
            DateTime resourceLastWriteTime);

        public TextEditorEdit Reload(
        ResourceUri resourceUri,
            string content,
            DateTime resourceLastWriteTime);

        public TextEditorEdit RedoEdit(ResourceUri resourceUri);

        public TextEditorEdit InsertText(
            InsertTextAction insertTextAction,
            RefreshCursorsRequest refreshCursorsRequest);

        public TextEditorEdit HandleKeyboardEvent(
            KeyboardEventAction keyboardEventAction,
            RefreshCursorsRequest refreshCursorsRequest);

        public TextEditorEdit DeleteTextByRange(
            DeleteTextByRangeAction deleteTextByRangeAction,
            RefreshCursorsRequest refreshCursorsRequest);

        public TextEditorEdit DeleteTextByMotion(
            DeleteTextByMotionAction deleteTextByMotionAction,
            RefreshCursorsRequest refreshCursorsRequest);
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
        public TextEditorEdit UndoEdit(ResourceUri resourceUri)
        {
            return context =>
            {
                _dispatcher.Dispatch(new UndoEditAction(resourceUri));
                return Task.CompletedTask;
            };
        }

        public TextEditorEdit SetUsingRowEndingKind(
            ResourceUri resourceUri,
            RowEndingKind rowEndingKind)
        {
            return context =>
            {
                _dispatcher.Dispatch(new SetUsingRowEndingKindAction(
                    resourceUri,
                    rowEndingKind));

                return Task.CompletedTask;
            };
        }

        public TextEditorEdit SetResourceData(
            ResourceUri resourceUri,
            DateTime resourceLastWriteTime)
        {
            return context =>
            {
                _dispatcher.Dispatch(new SetResourceDataAction(
                    resourceUri,
                    resourceLastWriteTime));

                return Task.CompletedTask;
            };
        }

        public TextEditorEdit Reload(
            ResourceUri resourceUri,
            string content,
            DateTime resourceLastWriteTime)
        {
            return context =>
            {
                _dispatcher.Dispatch(new ReloadAction(
                    resourceUri,
                    content,
                    resourceLastWriteTime));

                return Task.CompletedTask;
            };
        }

        public TextEditorEdit RedoEdit(ResourceUri resourceUri)
        {
            return context =>
            {
                _dispatcher.Dispatch(new RedoEditAction(resourceUri));
                return Task.CompletedTask;
            };
        }

        public TextEditorEdit InsertText(
            InsertTextAction insertTextAction,
            RefreshCursorsRequest refreshCursorsRequest)
        {
            return context =>
            {
                var cursorBag = refreshCursorsRequest?.CursorBag ?? insertTextAction.CursorModifierBag;

                insertTextAction = insertTextAction with
                {
                    CursorModifierBag = cursorBag,
                };

                _dispatcher.Dispatch(insertTextAction);

                return Task.CompletedTask;
            };
        }

        public TextEditorEdit HandleKeyboardEvent(
            KeyboardEventAction keyboardEventAction,
            RefreshCursorsRequest refreshCursorsRequest)
        {
            return context =>
            {
                var cursorBag = refreshCursorsRequest?.CursorBag ?? keyboardEventAction.CursorModifierBag;

                keyboardEventAction = keyboardEventAction with
                {
                    CursorModifierBag = cursorBag,
                };

                _dispatcher.Dispatch(keyboardEventAction);

                return Task.CompletedTask;
            };
        }

        public TextEditorEdit DeleteTextByRange(
            DeleteTextByRangeAction deleteTextByRangeAction,
            RefreshCursorsRequest refreshCursorsRequest)
        {
            return context =>
            {
                var cursorBag = refreshCursorsRequest?.CursorBag ?? deleteTextByRangeAction.CursorModifierBag;

                deleteTextByRangeAction = deleteTextByRangeAction with
                {
                    CursorModifierBag = cursorBag,
                };

                _dispatcher.Dispatch(deleteTextByRangeAction);

                return Task.CompletedTask;
            };
        }

        public TextEditorEdit DeleteTextByMotion(
            DeleteTextByMotionAction deleteTextByMotionAction,
            RefreshCursorsRequest refreshCursorsRequest)
        {
            return context =>
            {
                var cursorBag = refreshCursorsRequest?.CursorBag ?? deleteTextByMotionAction.CursorModifierBag;

                deleteTextByMotionAction = deleteTextByMotionAction with
                {
                    CursorModifierBag = cursorBag,
                };

                _dispatcher.Dispatch(deleteTextByMotionAction);

                return Task.CompletedTask;
            };
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