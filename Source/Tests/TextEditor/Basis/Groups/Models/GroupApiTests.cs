using Fluxor;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib;

namespace Luthetus.TextEditor.Tests.Basis.Groups.Models;

/// <summary>
/// <see cref="ITextEditorService.TextEditorGroupApi"/>
/// </summary>
public class GroupApiTests
{
    /// <summary>
    /// <see cref="ITextEditorService.TextEditorGroupApi.TextEditorGroupApi(ITextEditorService, IDispatcher)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        InitializeTextEditorGroupApiTests(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var inGroup,
            out var serviceProvider);

        Assert.NotNull(textEditorService.GroupApi);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorGroupApi.SetActiveViewModel(Key{TextEditorGroup}, Key{TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void SetActiveViewModel()
    {
        InitializeTextEditorGroupApiTests(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var inGroup,
            out var serviceProvider);

        Assert.Equal(Key<TextEditorViewModel>.Empty, inGroup.ActiveViewModelKey);

        textEditorService.GroupApi.AddViewModel(inGroup.GroupKey, inViewModel.ViewModelKey);
        textEditorService.GroupApi.SetActiveViewModel(inGroup.GroupKey, inViewModel.ViewModelKey);

        var outGroup = textEditorService.GroupApi.GetOrDefault(inGroup.GroupKey)!;

        Assert.Equal(
            inViewModel.ViewModelKey,
            outGroup.ActiveViewModelKey);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorGroupApi.RemoveViewModel(Key{TextEditorGroup}, Key{TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void RemoveViewModel()
    {
        InitializeTextEditorGroupApiTests(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var inGroup,
            out var serviceProvider);

        Assert.Empty(inGroup.ViewModelKeyList);

        textEditorService.GroupApi.AddViewModel(inGroup.GroupKey, inViewModel.ViewModelKey);
        var refGroup = textEditorService.GroupApi.GetOrDefault(inGroup.GroupKey)!;

        Assert.NotEmpty(refGroup.ViewModelKeyList);

        textEditorService.GroupApi.RemoveViewModel(inGroup.GroupKey, inViewModel.ViewModelKey);
        var outGroup = textEditorService.GroupApi.GetOrDefault(inGroup.GroupKey)!;

        Assert.Empty(outGroup.ViewModelKeyList);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorGroupApi.Register(Key{TextEditorGroup})"/>
    /// </summary>
    [Fact]
    public void Register()
    {
        InitializeTextEditorGroupApiTests(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var inGroup,
            out var serviceProvider);

        Assert.Single(textEditorService.GroupApi.GetGroups());

        var groupKey = Key<TextEditorGroup>.NewKey();
        textEditorService.GroupApi.Register(groupKey);
        var outGroup = textEditorService.GroupApi.GetOrDefault(groupKey)!;

        Assert.NotNull(outGroup);
        Assert.Equal(2, textEditorService.GroupApi.GetGroups().Count);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorGroupApi.Dispose(Key{TextEditorGroup})"/>
    /// </summary>
    [Fact]
    public void Dispose()
    {
        InitializeTextEditorGroupApiTests(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var inGroup,
            out var serviceProvider);

        Assert.Single(textEditorService.GroupApi.GetGroups());

        var groupKey = Key<TextEditorGroup>.NewKey();

        textEditorService.GroupApi.Register(groupKey);
        Assert.Equal(2, textEditorService.GroupApi.GetGroups().Count);

        textEditorService.GroupApi.Dispose(groupKey);
        Assert.Single(textEditorService.GroupApi.GetGroups());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorGroupApi.GetOrDefault(Key{TextEditorGroup})"/>
    /// </summary>
    [Fact]
    public void GetOrDefault()
    {
        InitializeTextEditorGroupApiTests(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var inGroup,
            out var serviceProvider);

        Assert.Single(textEditorService.GroupApi.GetGroups());

        var groupKey = Key<TextEditorGroup>.NewKey();
        textEditorService.GroupApi.Register(groupKey);
        var outGroup = textEditorService.GroupApi.GetOrDefault(groupKey)!;

        Assert.NotNull(outGroup);
        Assert.Equal(groupKey, outGroup.GroupKey);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorGroupApi.AddViewModel(Key{TextEditorGroup}, Key{TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void AddViewModel()
    {
        InitializeTextEditorGroupApiTests(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var inGroup,
            out var serviceProvider);

        Assert.Empty(inGroup.ViewModelKeyList);

        textEditorService.GroupApi.AddViewModel(inGroup.GroupKey, inViewModel.ViewModelKey);
        var outGroup = textEditorService.GroupApi.GetOrDefault(inGroup.GroupKey)!;

        Assert.Single(outGroup.ViewModelKeyList);
    }

    private void InitializeTextEditorGroupApiTests(
        out ITextEditorService textEditorService,
        out TextEditorModel inModel,
        out TextEditorViewModel inViewModel,
        out TextEditorGroup inGroup,
        out IServiceProvider serviceProvider)
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out textEditorService,
            out inModel,
            out inViewModel,
            out serviceProvider);

        // TODO: Separately, register existing needs to be made for all API that...
        // ...registers with a key alone.
        var groupKey = Key<TextEditorGroup>.NewKey();

        textEditorService.GroupApi.Register(groupKey);
        inGroup = textEditorService.GroupApi.GetOrDefault(groupKey)
            ?? throw new ArgumentNullException();
    }
}