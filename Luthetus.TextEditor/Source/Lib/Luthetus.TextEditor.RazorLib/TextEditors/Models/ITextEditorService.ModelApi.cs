using Fluxor;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial interface ITextEditorService
{
    public interface IModelApi
    {
        public void DeleteTextByMotion(TextEditorModelState.DeleteTextByMotionAction deleteTextByMotionAction);
        public void DeleteTextByRange(TextEditorModelState.DeleteTextByRangeAction deleteTextByRangeAction);
        public void Dispose(ResourceUri resourceUri);
        public TextEditorModel? FindOrDefault(ResourceUri resourceUri);
        public string? GetAllText(ResourceUri resourceUri);
        public ImmutableArray<TextEditorViewModel> GetViewModelsOrEmpty(ResourceUri resourceUri);
        public void HandleKeyboardEvent(TextEditorModelState.KeyboardEventAction keyboardEventAction);
        public void InsertText(TextEditorModelState.InsertTextAction insertTextAction);
        public void RedoEdit(ResourceUri resourceUri);
        /// <summary>It is recommended to use the <see cref="RegisterTemplated" /> method as it will internally reference the <see cref="ITextEditorLexer" /> and <see cref="IDecorationMapper" /> that correspond to the desired text editor.</summary>
        public void RegisterCustom(TextEditorModel model);
        ///// 

        /// <summary>
        /// As an example, for a C# Text Editor one would pass in an <see cref="extensionNoPeriod"/>
        /// of "cs" or the constant varible: <see cref="ExtensionNoPeriodFacts.C_SHARP_CLASS"/>.<br /><br />
        /// 
        /// As an example, for a Plain Text Editor one would pass in an <see cref="extensionNoPeriod"/>
        /// of "txt" or the constant varible: <see cref="ExtensionNoPeriodFacts.TXT"/>.<br /><br />
        /// </summary>
        public void RegisterTemplated(
            IDecorationMapperRegistry decorationMapperRegistry,
            ICompilerServiceRegistry compilerServiceRegistry,
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
        public TextEditorModel? FindOrDefaultByResourceUri(ResourceUri resourceUri);
        public void RegisterPresentationModel(ResourceUri resourceUri, TextEditorPresentationModel emptyPresentationModel);
    }

    public class ModelApi : IModelApi
    {
        private readonly ITextEditorService _textEditorService;
        private readonly IDispatcher _dispatcher;

        public ModelApi(ITextEditorService textEditorService, IDispatcher dispatcher)
        {
            _textEditorService = textEditorService;
            _dispatcher = dispatcher;
        }

        public TextEditorModel? FindOrDefaultByResourceUri(ResourceUri resourceUri)
        {
            return _textEditorService.ModelStateWrap.Value.ModelBag.FirstOrDefault(
                x => x.ResourceUri == resourceUri);
        }

        public void UndoEdit(ResourceUri resourceUri)
        {
            _dispatcher.Dispatch(new TextEditorModelState.UndoEditAction(resourceUri));
        }

        public void SetUsingRowEndingKind(ResourceUri resourceUri, RowEndingKind rowEndingKind)
        {
            _dispatcher.Dispatch(new TextEditorModelState.SetUsingRowEndingKindAction(
                resourceUri,
                rowEndingKind));
        }

        public void SetResourceData(
            ResourceUri resourceUri,
            DateTime resourceLastWriteTime)
        {
            _dispatcher.Dispatch(new TextEditorModelState.SetResourceDataAction(
                resourceUri,
                resourceLastWriteTime));
        }

        public void Reload(ResourceUri resourceUri, string content, DateTime resourceLastWriteTime)
        {
            _dispatcher.Dispatch(new TextEditorModelState.ReloadAction(
                resourceUri,
                content,
                resourceLastWriteTime));
        }

        public void RegisterCustom(TextEditorModel model)
        {
            _dispatcher.Dispatch(new TextEditorModelState.RegisterAction(model));
        }

        public void RegisterTemplated(
            IDecorationMapperRegistry decorationMapperRegistry,
            ICompilerServiceRegistry compilerServiceRegistry,
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
                decorationMapperRegistry.GetDecorationMapper(extensionNoPeriod),
                compilerServiceRegistry.GetCompilerService(extensionNoPeriod),
                null,
                new());

            // ICommonBackgroundTaskQueue does not work well here because
            // this Task does not need to be tracked.
            _ = Task.Run(async () =>
            {
                try
                {
                    await textEditorModel.ApplySyntaxHighlightingAsync();
                    _dispatcher.Dispatch(new TextEditorModelState.ForceRerenderAction(textEditorModel.ResourceUri));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }, CancellationToken.None);

            _dispatcher.Dispatch(new TextEditorModelState.RegisterAction(textEditorModel));
        }

        public void RedoEdit(ResourceUri resourceUri)
        {
            _dispatcher.Dispatch(new TextEditorModelState.RedoEditAction(resourceUri));
        }

        public void InsertText(TextEditorModelState.InsertTextAction insertTextAction)
        {
            _dispatcher.Dispatch(insertTextAction);
        }

        public void HandleKeyboardEvent(TextEditorModelState.KeyboardEventAction keyboardEventAction)
        {
            _dispatcher.Dispatch(keyboardEventAction);
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

        public TextEditorModel? FindOrDefault(ResourceUri resourceUri)
        {
            return _textEditorService.ModelStateWrap.Value.ModelBag
                .FirstOrDefault(x => x.ResourceUri == resourceUri);
        }

        public void Dispose(ResourceUri resourceUri)
        {
            _dispatcher.Dispatch(new TextEditorModelState.DisposeAction(resourceUri));
        }

        public void DeleteTextByRange(TextEditorModelState.DeleteTextByRangeAction deleteTextByRangeAction)
        {
            _dispatcher.Dispatch(deleteTextByRangeAction);
        }

        public void DeleteTextByMotion(TextEditorModelState.DeleteTextByMotionAction deleteTextByMotionAction)
        {
            _dispatcher.Dispatch(deleteTextByMotionAction);
        }

        public void RegisterPresentationModel(ResourceUri resourceUri, TextEditorPresentationModel emptyPresentationModel)
        {
            _dispatcher.Dispatch(new TextEditorModelState.RegisterPresentationModelAction(
                resourceUri,
                emptyPresentationModel));
        }
    }
}