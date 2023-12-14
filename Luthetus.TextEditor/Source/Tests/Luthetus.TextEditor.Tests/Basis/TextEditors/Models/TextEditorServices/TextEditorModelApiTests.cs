using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;

/// <summary>
/// <see cref="ITextEditorService.TextEditorModelApi"/>
/// </summary>
public class TextEditorModelApiTests
{
    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.TextEditorModelApi(ITextEditorService, Common.RazorLib.BackgroundTasks.Models.IBackgroundTaskService, Fluxor.IDispatcher)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(out var textEditorService);

        Assert.NotNull(textEditorService.ModelApi);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.UndoEdit(RazorLib.Lexes.Models.ResourceUri)"/>
    /// </summary>
    [Fact]
    public void UndoEdit()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.SetUsingRowEndingKind(RazorLib.Lexes.Models.ResourceUri, RazorLib.Rows.Models.RowEndingKind)"/>
    /// </summary>
    [Fact]
    public void SetUsingRowEndingKind()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.SetResourceData(RazorLib.Lexes.Models.ResourceUri, DateTime)"/>
    /// </summary>
    [Fact]
    public void SetResourceData()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.Reload(RazorLib.Lexes.Models.ResourceUri, string, DateTime)"/>
    /// </summary>
    [Fact]
    public void Reload()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.RegisterCustom(RazorLib.TextEditors.Models.TextEditorModels.TextEditorModel)"/>
    /// </summary>
    [Fact]
    public void RegisterCustom()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.RegisterTemplated(RazorLib.Decorations.Models.IDecorationMapperRegistry, RazorLib.CompilerServices.ICompilerServiceRegistry, string, RazorLib.Lexes.Models.ResourceUri, DateTime, string, string?)"/>
    /// </summary>
    [Fact]
    public void RegisterTemplated()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(out var textEditorService);

        // textEditorService.ModelApi.RegisterTemplated();

        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.RedoEdit(RazorLib.Lexes.Models.ResourceUri)"/>
    /// </summary>
    [Fact]
    public void RedoEdit()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.InsertText(RazorLib.TextEditors.States.TextEditorModelState.InsertTextAction)"/>
    /// </summary>
    [Fact]
    public void InsertText()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.HandleKeyboardEvent(RazorLib.TextEditors.States.TextEditorModelState.KeyboardEventAction)"/>
    /// </summary>
    [Fact]
    public void HandleKeyboardEvent()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.GetViewModelsOrEmpty(RazorLib.Lexes.Models.ResourceUri)"/>
    /// </summary>
    [Fact]
    public void GetViewModelsOrEmpty()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.GetAllText(RazorLib.Lexes.Models.ResourceUri)"/>
    /// </summary>
    [Fact]
    public void GetAllText()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.FindOrDefault(RazorLib.Lexes.Models.ResourceUri)"/>
    /// </summary>
    [Fact]
    public void FindOrDefault()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.Dispose(RazorLib.Lexes.Models.ResourceUri)"/>
    /// </summary>
    [Fact]
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.DeleteTextByRange(RazorLib.TextEditors.States.TextEditorModelState.DeleteTextByRangeAction)"/>
    /// </summary>
    [Fact]
    public void DeleteTextByRange()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.DeleteTextByMotion(RazorLib.TextEditors.States.TextEditorModelState.DeleteTextByMotionAction)"/>
    /// </summary>
    [Fact]
    public void DeleteTextByMotion()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.RegisterPresentationModel(RazorLib.Lexes.Models.ResourceUri, RazorLib.Decorations.Models.TextEditorPresentationModel)"/>
    /// </summary>
    [Fact]
    public void RegisterPresentationModel()
    {
        throw new NotImplementedException();
    }
}