namespace Luthetus.TextEditor.Tests.Basis;

/// <summary>
/// <see cref="TextEditorService"/>
/// </summary>
public class ServiceTests
{
    /// <summary>
    /// <see cref="TextEditorService(IState{RazorLib.TextEditors.States.TextEditorModelState}, IState{RazorLib.TextEditors.States.TextEditorViewModelState}, IState{RazorLib.Groups.States.TextEditorGroupState}, IState{RazorLib.Diffs.States.TextEditorDiffState}, IState{Common.RazorLib.Themes.States.ThemeState}, IState{RazorLib.Options.States.TextEditorOptionsState}, IState{RazorLib.Finds.States.TextEditorSearchEngineState}, RazorLib.Installations.Models.LuthetusTextEditorConfig, Common.RazorLib.Storages.Models.IStorageService, Microsoft.JSInterop.IJSRuntime, Common.RazorLib.Storages.States.StorageSync, IDispatcher)"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorService.ModelStateWrap"/>
    /// <see cref="TextEditorService.ViewModelStateWrap"/>
    /// <see cref="TextEditorService.GroupStateWrap"/>
    /// <see cref="TextEditorService.DiffStateWrap"/>
    /// <see cref="TextEditorService.ThemeStateWrap"/>
    /// <see cref="TextEditorService.OptionsStateWrap"/>
    /// <see cref="TextEditorService.SearchEngineStateWrap"/>
    /// <see cref="TextEditorService.ThemeCssClassString"/>
    /// <see cref="TextEditorService.ModelApi"/>
    /// <see cref="TextEditorService.ViewModelApi"/>
    /// <see cref="TextEditorService.GroupApi"/>
    /// <see cref="TextEditorService.DiffApi"/>
    /// <see cref="TextEditorService.OptionsApi"/>
    /// <see cref="TextEditorService.SearchEngineApi"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.NotNull(textEditorService);
        Assert.NotNull(textEditorService.TextEditorStateWrap);
        Assert.NotNull(textEditorService.GroupStateWrap);
        Assert.NotNull(textEditorService.DiffStateWrap);
        Assert.NotNull(textEditorService.ThemeStateWrap);
        Assert.NotNull(textEditorService.OptionsStateWrap);
        Assert.NotNull(textEditorService.FindAllStateWrap);
        Assert.NotNull(textEditorService.ThemeCssClassString);
        Assert.NotNull(textEditorService.ModelApi);
        Assert.NotNull(textEditorService.ViewModelApi);
        Assert.NotNull(textEditorService.GroupApi);
        Assert.NotNull(textEditorService.DiffApi);
        Assert.NotNull(textEditorService.OptionsApi);
    }
}