using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Editors.States;

public partial class EditorSync
{
    public async Task RegisterModelFunc(RegisterModelArgs registerModelArgs)
    {
        var model = _textEditorService.ModelApi.GetOrDefault(registerModelArgs.ResourceUri);

        if (model is null)
        {
            var resourceUri = registerModelArgs.ResourceUri;
            var fileLastWriteTime = await _fileSystemProvider.File.GetLastWriteTimeAsync(resourceUri.Value);
            var content = await _fileSystemProvider.File.ReadAllTextAsync(resourceUri.Value);

            var absolutePath = _environmentProvider.AbsolutePathFactory(resourceUri.Value, false);

            var decorationMapper = _decorationMapperRegistry.GetDecorationMapper(absolutePath.ExtensionNoPeriod);
            var compilerService = _compilerServiceRegistry.GetCompilerService(absolutePath.ExtensionNoPeriod);

            model = new TextEditorModel(
                resourceUri,
                fileLastWriteTime,
                absolutePath.ExtensionNoPeriod,
                content,
                decorationMapper,
                compilerService);

            _textEditorService.ModelApi.RegisterCustom(model);

            _textEditorService.Post(
                nameof(_textEditorService.ModelApi.AddPresentationModelFactory),
                async editContext =>
                {
                    await _textEditorService.ModelApi.AddPresentationModelFactory(
                            model.ResourceUri,
                            CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel)
                        .Invoke(editContext);

					await _textEditorService.ModelApi.AddPresentationModelFactory(
                            model.ResourceUri,
                            FindOverlayPresentationFacts.EmptyPresentationModel)
                        .Invoke(editContext)
                        .ConfigureAwait(false);

                    await _textEditorService.ModelApi.AddPresentationModelFactory(
                            model.ResourceUri,
                            DiffPresentationFacts.EmptyInPresentationModel)
                        .Invoke(editContext);

                    await _textEditorService.ModelApi.AddPresentationModelFactory(
                            model.ResourceUri,
                            DiffPresentationFacts.EmptyOutPresentationModel)
                        .Invoke(editContext);

                    model.CompilerService.RegisterResource(model.ResourceUri);
                });
        }

        await CheckIfContentsWereModifiedAsync(
            Dispatcher,
            registerModelArgs.ResourceUri.Value,
            model);
    }

    public Task<Key<TextEditorViewModel>> TryRegisterViewModelFunc(TryRegisterViewModelArgs registerViewModelArgs)
    {
        var model = _textEditorService.ModelApi.GetOrDefault(registerViewModelArgs.ResourceUri);

        if (model is null)
            return Task.FromResult(Key<TextEditorViewModel>.Empty);

        var viewModel = _textEditorService.ModelApi
            .GetViewModelsOrEmpty(registerViewModelArgs.ResourceUri)
            .FirstOrDefault(x => x.Category == registerViewModelArgs.Category);

        if (viewModel is not null)
            return Task.FromResult(viewModel.ViewModelKey);

        var viewModelKey = Key<TextEditorViewModel>.NewKey();

        _textEditorService.ViewModelApi.Register(
            viewModelKey,
            registerViewModelArgs.ResourceUri,
            registerViewModelArgs.Category);

        var layerLastPresentationKeys = new[]
        {
            CompilerServiceDiagnosticPresentationFacts.PresentationKey,
            FindOverlayPresentationFacts.PresentationKey,
        }.ToImmutableArray();

        var absolutePath = _environmentProvider.AbsolutePathFactory(
            registerViewModelArgs.ResourceUri.Value,
            false);

        _textEditorService.Post(
            nameof(TryRegisterViewModelFunc),
            _textEditorService.ViewModelApi.WithValueFactory(
                viewModelKey,
                textEditorViewModel => textEditorViewModel with
                {
                    OnSaveRequested = HandleOnSaveRequested,
                    GetTabDisplayNameFunc = _ => absolutePath.NameWithExtension,
                    ShouldSetFocusAfterNextRender = registerViewModelArgs.ShouldSetFocusToEditor,
                    LastPresentationLayerKeysList = layerLastPresentationKeys.ToImmutableList()
                }));

        return Task.FromResult(viewModelKey);

        void HandleOnSaveRequested(ITextEditorModel innerTextEditor)
        {
            var innerContent = innerTextEditor.GetAllText();

            var cancellationToken = model.TextEditorSaveFileHelper.GetCancellationToken();

            _fileSystemSync.SaveFile(
                absolutePath,
                innerContent,
                writtenDateTime =>
                {
                    if (writtenDateTime is not null)
                    {
                        _textEditorService.Post(
                            nameof(HandleOnSaveRequested),
                            _textEditorService.ModelApi.SetResourceDataFactory(
                                innerTextEditor.ResourceUri,
                                writtenDateTime.Value));
                    }
                },
                cancellationToken);
        }
    }

    public Task<bool> TryShowViewModelFunc(TryShowViewModelArgs showViewModelArgs)
    {
        _textEditorService.GroupApi.Register(EditorTextEditorGroupKey);

        var viewModel = _textEditorService.ViewModelApi.GetOrDefault(showViewModelArgs.ViewModelKey);

        if (viewModel is null)
            return Task.FromResult(false);

        if (viewModel.Category == new TextEditorCategory("main") &&
            showViewModelArgs.GroupKey == Key<TextEditorGroup>.Empty)
        {
            showViewModelArgs = new TryShowViewModelArgs(
                showViewModelArgs.ViewModelKey,
                EditorTextEditorGroupKey,
                showViewModelArgs.ServiceProvider);
        }

        if (showViewModelArgs.ViewModelKey == Key<TextEditorViewModel>.Empty ||
            showViewModelArgs.GroupKey == Key<TextEditorGroup>.Empty)
        {
            return Task.FromResult(false);
        }

        _textEditorService.GroupApi.AddViewModel(
            showViewModelArgs.GroupKey,
            showViewModelArgs.ViewModelKey);

        _textEditorService.GroupApi.SetActiveViewModel(
            showViewModelArgs.GroupKey,
            showViewModelArgs.ViewModelKey);

        return Task.FromResult(true);
    }
}
