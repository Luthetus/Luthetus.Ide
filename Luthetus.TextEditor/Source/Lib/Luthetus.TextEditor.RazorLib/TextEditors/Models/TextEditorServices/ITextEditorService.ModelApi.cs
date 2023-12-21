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
using Luthetus.Common.RazorLib.Keys.Models;

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
            Key<TextEditorViewModel> viewModelKey);

        /// <summary>
        /// If one wants to guarantee that the state is up to date use <see cref="InsertText"/>
        /// instead of this method. This is because, the <see cref="ITextEditorService"/> will provide
        /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
        /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
        /// <br/><br/>
        /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
        /// map to the view model's cursors, then one would use this method. Since an attempt to map
        /// the cursor key would come back as the cursor not existing.
        /// </summary>
        public TextEditorEdit InsertTextUnsafe(
            InsertTextAction insertTextAction,
            TextEditorCursorModifierBag cursorModifierBag);

        public TextEditorEdit HandleKeyboardEvent(
            KeyboardEventAction keyboardEventAction,
            Key<TextEditorViewModel> viewModelKey);

        /// <summary>
        /// If one wants to guarantee that the state is up to date use <see cref="HandleKeyboardEvent"/>
        /// instead of this method. This is because, the <see cref="ITextEditorService"/> will provide
        /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
        /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
        /// <br/><br/>
        /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
        /// map to the view model's cursors, then one would use this method. Since an attempt to map
        /// the cursor key would come back as the cursor not existing.
        /// </summary>
        public TextEditorEdit HandleKeyboardEventUnsafe(
            KeyboardEventAction keyboardEventAction,
            TextEditorCursorModifierBag cursorModifierBag);

        public TextEditorEdit DeleteTextByRange(
            DeleteTextByRangeAction deleteTextByRangeAction,
            Key<TextEditorViewModel> viewModelKey);

        /// <summary>
        /// If one wants to guarantee that the state is up to date use <see cref="DeleteTextByRange"/>
        /// instead of this method. This is because, the <see cref="ITextEditorService"/> will provide
        /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
        /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
        /// <br/><br/>
        /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
        /// map to the view model's cursors, then one would use this method. Since an attempt to map
        /// the cursor key would come back as the cursor not existing.
        /// </summary>
        public TextEditorEdit DeleteTextByRangeUnsafe(
            DeleteTextByRangeAction deleteTextByRangeAction,
            TextEditorCursorModifierBag cursorModifierBag);

        public TextEditorEdit DeleteTextByMotion(
            DeleteTextByMotionAction deleteTextByMotionAction,
            Key<TextEditorViewModel> viewModelKey);

        /// <summary>
        /// If one wants to guarantee that the state is up to date use <see cref="DeleteTextByMotion"/>
        /// instead of this method. This is because, the <see cref="ITextEditorService"/> will provide
        /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
        /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
        /// <br/><br/>
        /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
        /// map to the view model's cursors, then one would use this method. Since an attempt to map
        /// the cursor key would come back as the cursor not existing.
        /// </summary>
        public TextEditorEdit DeleteTextByMotionUnsafe(
            DeleteTextByMotionAction deleteTextByMotionAction,
            TextEditorCursorModifierBag cursorModifierBag);
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
                _dispatcher.Dispatch(new UndoEditAction(
                    resourceUri,
                    context.AuthenticatedActionKey));

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
                    rowEndingKind,
                    context.AuthenticatedActionKey));

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
                    resourceLastWriteTime,
                    context.AuthenticatedActionKey));

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
                    resourceLastWriteTime,
                    context.AuthenticatedActionKey));

                return Task.CompletedTask;
            };
        }

        public TextEditorEdit RedoEdit(ResourceUri resourceUri)
        {
            return context =>
            {
                _dispatcher.Dispatch(new RedoEditAction(resourceUri, context.AuthenticatedActionKey));
                return Task.CompletedTask;
            };
        }

        public TextEditorEdit InsertText(
            InsertTextAction insertTextAction,
            Key<TextEditorViewModel> viewModelKey)
        {
            return editContext =>
            {
                var modelModifier = editContext.GetModelModifierByViewModelKey(viewModelKey);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

                if (modelModifier is null || viewModelModifier is null)
                    return Task.CompletedTask;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;

                return InsertTextUnsafe(insertTextAction, cursorModifierBag)
                    .Invoke(editContext);
            };
        }
        
        public TextEditorEdit InsertTextUnsafe(
            InsertTextAction insertTextAction,
            TextEditorCursorModifierBag cursorModifierBag)
        {
            return context =>
            {
                insertTextAction = insertTextAction with
                {
                    CursorModifierBag = cursorModifierBag,
                };

                _dispatcher.Dispatch(insertTextAction);

                return Task.CompletedTask;
            };
        }

        public TextEditorEdit HandleKeyboardEvent(
            KeyboardEventAction keyboardEventAction,
            Key<TextEditorViewModel> viewModelKey)
        {
            return editContext =>
            {
                var modelModifier = editContext.GetModelModifierByViewModelKey(viewModelKey);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

                if (modelModifier is null || viewModelModifier is null)
                    return Task.CompletedTask;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;

                return HandleKeyboardEventUnsafe(keyboardEventAction, cursorModifierBag)
                    .Invoke(editContext);
            };
        }

        public TextEditorEdit HandleKeyboardEventUnsafe(
            KeyboardEventAction keyboardEventAction,
            TextEditorCursorModifierBag cursorModifierBag)
        {
            return context =>
            {
                keyboardEventAction = keyboardEventAction with
                {
                    CursorModifierBag = cursorModifierBag,
                };

                _dispatcher.Dispatch(keyboardEventAction);

                return Task.CompletedTask;
            };
        }

        public TextEditorEdit DeleteTextByRange(
            DeleteTextByRangeAction deleteTextByRangeAction,
            Key<TextEditorViewModel> viewModelKey)
        {
            return editContext =>
            {
                var modelModifier = editContext.GetModelModifierByViewModelKey(viewModelKey);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

                if (modelModifier is null || viewModelModifier is null)
                    return Task.CompletedTask;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;

                return DeleteTextByRangeUnsafe(deleteTextByRangeAction, cursorModifierBag)
                    .Invoke(editContext);
            };
        }
        
        public TextEditorEdit DeleteTextByRangeUnsafe(
            DeleteTextByRangeAction deleteTextByRangeAction,
            TextEditorCursorModifierBag cursorModifierBag)
        {
            return context =>
            {
                deleteTextByRangeAction = deleteTextByRangeAction with
                {
                    CursorModifierBag = cursorModifierBag,
                };

                _dispatcher.Dispatch(deleteTextByRangeAction);

                return Task.CompletedTask;
            };
        }

        public TextEditorEdit DeleteTextByMotion(
            DeleteTextByMotionAction deleteTextByMotionAction,
            Key<TextEditorViewModel> viewModelKey)
        {
            return editContext =>
            {
                var modelModifier = editContext.GetModelModifierByViewModelKey(viewModelKey);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

                if (modelModifier is null || viewModelModifier is null)
                    return Task.CompletedTask;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (cursorModifierBag is null || primaryCursorModifier is null)
                    return Task.CompletedTask;

                return DeleteTextByMotionUnsafe(deleteTextByMotionAction, cursorModifierBag)
                    .Invoke(editContext);
            };
        }
        
        public TextEditorEdit DeleteTextByMotionUnsafe(
            DeleteTextByMotionAction deleteTextByMotionAction,
            TextEditorCursorModifierBag cursorModifierBag)
        {
            return context =>
            {
                deleteTextByMotionAction = deleteTextByMotionAction with
                {
                    CursorModifierBag = cursorModifierBag,
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