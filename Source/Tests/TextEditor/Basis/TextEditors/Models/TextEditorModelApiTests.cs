using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Options.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models;

/// <summary>
/// <see cref="TextEditorModelApi"/>
/// </summary>
public class TextEditorModelApiTests
{
    /// <summary>
    /// <see cref="TextEditorModelApi(ITextEditorService, IDecorationMapperRegistry, ICompilerServiceRegistry, IBackgroundTaskService, IDispatcher)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        InitializeTextEditorModelApiTests(
            out var inModel,
            out var inViewModel,
            out var textEditorService,
            out var textEditorOptionsStateWrap,
            out var dispatcher,
            out var serviceProvider);

        Assert.NotNull(textEditorService.ModelApi);
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.UndoEditFactory(ResourceUri)"/>
    /// </summary>
    [Fact]
    public async Task UndoEdit()
    {
        InitializeTextEditorModelApiTests(
            out var inModel,
            out var inViewModel,
            out var textEditorService,
            out var textEditorOptionsStateWrap,
            out var dispatcher,
            out var serviceProvider);

        var cursor = new TextEditorCursor(0, 0, 0, true, TextEditorSelection.Empty);
        var cursorList = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        var insertedText = "I have something to say: ";

        var cursorModifierBag = new CursorModifierBagTextEditor(Key<TextEditorViewModel>.Empty, cursorList);

        await textEditorService.PostSimpleBatch(
            nameof(textEditorService.ModelApi.InsertTextUnsafeFactory),
            string.Empty,
            textEditorService.ModelApi.InsertTextUnsafeFactory(
                inModel.ResourceUri,
                cursorModifierBag,
                insertedText,
                CancellationToken.None));

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(
            insertedText + inModel.GetAllText(),
            outModel.GetAllText());

        await textEditorService.PostSimpleBatch(
            nameof(textEditorService.ModelApi.UndoEditFactory),
            string.Empty,
            textEditorService.ModelApi.UndoEditFactory(
                inModel.ResourceUri));

        outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(inModel.GetAllText(), outModel.GetAllText());

        throw new NotImplementedException("Test was broken on (2024-04-08)");
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.SetUsingLineEndKindFactory(ResourceUri, LineEndKind)"/>
    /// </summary>
    [Fact]
    public async Task SetUsingRowEndingKind()
    {
        InitializeTextEditorModelApiTests(
            out var inModel,
            out var inViewModel,
            out var textEditorService,
            out var textEditorOptionsStateWrap,
            out var dispatcher,
            out var serviceProvider);

        var rowEndingKind = LineEndKind.CarriageReturn;

        // Assert the current values are different from that which will be set.
        Assert.NotEqual(rowEndingKind, inModel.LineEndKindPreference);

        await textEditorService.PostSimpleBatch(
            nameof(textEditorService.ModelApi.SetUsingLineEndKindFactory),
            string.Empty,
            textEditorService.ModelApi.SetUsingLineEndKindFactory(
                inModel.ResourceUri, rowEndingKind));

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);

        // Assert the value is now set
        Assert.Equal(rowEndingKind, outModel!.LineEndKindPreference);
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.SetResourceDataFactory(ResourceUri, DateTime)"/>
    /// </summary>
    [Fact]
    public async Task SetResourceData()
    {
        InitializeTextEditorModelApiTests(
            out var inModel,
            out var inViewModel,
            out var textEditorService,
            out var textEditorOptionsStateWrap,
            out var dispatcher,
            out var serviceProvider);

        var newResourceLastWriteTime = inModel.ResourceLastWriteTime.AddDays(1);

        // Assert the current values are different from that which will be set.
        Assert.NotEqual(newResourceLastWriteTime, inModel.ResourceLastWriteTime);

        await textEditorService.PostSimpleBatch(
            nameof(textEditorService.ModelApi.SetResourceDataFactory),
            string.Empty,
            textEditorService.ModelApi.SetResourceDataFactory(
                inModel.ResourceUri, newResourceLastWriteTime));

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);

        // Assert the values are now set
        Assert.Equal(newResourceLastWriteTime, outModel!.ResourceLastWriteTime);
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.ReloadFactory(ResourceUri, string, DateTime)"/>
    /// </summary>
    [Fact]
    public async Task Reload()
    {
        InitializeTextEditorModelApiTests(
            out var inModel,
            out var inViewModel,
            out var textEditorService,
            out var textEditorOptionsStateWrap,
            out var dispatcher,
            out var serviceProvider);

        var newContent = "Alphabet Soup";

        // Assert the current values are different from that which will be set.
        Assert.NotEqual(newContent, inModel.GetAllText());

        await textEditorService.PostSimpleBatch(
            nameof(textEditorService.ModelApi.ReloadFactory),
            string.Empty,
            textEditorService.ModelApi.ReloadFactory(
                inModel.ResourceUri, newContent, DateTime.UtcNow));

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);

        // Assert the values are now set
        Assert.Equal(newContent, outModel!.GetAllText());
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.RegisterCustom(TextEditorModel)"/>
    /// </summary>
    [Fact]
    public void RegisterCustom()
    {
        InitializeTextEditorModelApiTests(
            out _,
            out var inViewModel,
            out var textEditorService,
            out var textEditorOptionsStateWrap,
            out var dispatcher,
            out var serviceProvider);

        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;
        var initialContent = "Hello World!";

        var decorationMapperRegistry = serviceProvider.GetRequiredService<IDecorationMapperRegistry>();
        var compilerServiceRegistry = serviceProvider.GetRequiredService<ICompilerServiceRegistry>();

        var decorationMapper = decorationMapperRegistry.GetDecorationMapper(fileExtension);
        var compilerService = compilerServiceRegistry.GetCompilerService(fileExtension);

        var model = new TextEditorModel(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            initialContent,
            decorationMapper,
            compilerService);

        textEditorService.ModelApi.RegisterCustom(model);

        var existingModel = textEditorService.ModelApi.GetOrDefault(model.ResourceUri);

        Assert.Equal(resourceUri, existingModel.ResourceUri);
        Assert.Equal(resourceLastWriteTime, existingModel.ResourceLastWriteTime);
        Assert.Equal(initialContent, existingModel.GetAllText());
        Assert.Equal(fileExtension, existingModel.FileExtension);
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.RegisterTemplated(string, ResourceUri, DateTime, string, string?)"/>
    /// </summary>
    [Fact]
    public void RegisterTemplated()
    {
        InitializeTextEditorModelApiTests(
            out _,
            out var inViewModel,
            out var textEditorService,
            out var textEditorOptionsStateWrap,
            out var dispatcher,
            out var serviceProvider);

        Assert.Empty(textEditorService.ModelApi.GetModels());

        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;
        var initialContent = "Hello World!";

        textEditorService.ModelApi.RegisterTemplated(
            fileExtension,
            resourceUri,
            resourceLastWriteTime,
            initialContent);

        var model = textEditorService.ModelApi.GetOrDefault(resourceUri);

        Assert.Equal(resourceUri, model.ResourceUri);
        Assert.Equal(resourceLastWriteTime, model.ResourceLastWriteTime);
        Assert.Equal(initialContent, model.GetAllText());
        Assert.Equal(fileExtension, model.FileExtension);
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.RedoEditFactory(ResourceUri)"/>
    /// </summary>
    [Fact]
    public async Task RedoEditEnqueue()
    {
        InitializeTextEditorModelApiTests(
            out var inModel,
            out var inViewModel,
            out var textEditorService,
            out var textEditorOptionsStateWrap,
            out var dispatcher,
            out var serviceProvider);

        var cursor = new TextEditorCursor(0, 0, 0, true, TextEditorSelection.Empty);
        var cursorList = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        var insertedText = "I have something to say: ";

        var cursorModifierBag = new CursorModifierBagTextEditor(Key<TextEditorViewModel>.Empty, cursorList);

        await textEditorService.PostSimpleBatch(
            nameof(textEditorService.ModelApi.InsertTextUnsafeFactory),
            string.Empty,
            textEditorService.ModelApi.InsertTextUnsafeFactory(
                inModel.ResourceUri,
                cursorModifierBag,
                insertedText,
                CancellationToken.None));

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(insertedText + inModel.GetAllText(), outModel.GetAllText());

        await textEditorService.PostSimpleBatch(
            nameof(textEditorService.ModelApi.UndoEditFactory),
            string.Empty,
            textEditorService.ModelApi.UndoEditFactory(
                inModel.ResourceUri));

        outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(inModel.GetAllText(), outModel.GetAllText());

        await textEditorService.PostSimpleBatch(
            nameof(textEditorService.ModelApi.RedoEditFactory),
            string.Empty,
            textEditorService.ModelApi.RedoEditFactory(
                inModel.ResourceUri));

        outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(insertedText + inModel.GetAllText(), outModel.GetAllText());
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.InsertTextFactory(ResourceUri, Key{TextEditorViewModel}, string, CancellationToken)"/>
    /// </summary>
    [Fact]
    public async Task InsertTextEnqueue()
    {
        InitializeTextEditorModelApiTests(
            out var inModel,
            out var inViewModel,
            out var textEditorService,
            out var textEditorOptionsStateWrap,
            out var dispatcher,
            out var serviceProvider);

        var cursor = new TextEditorCursor(0, 0, 0, true, TextEditorSelection.Empty);
        var cursorList = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        var insertedText = "I have something to say: ";

        var cursorModifierBag = new CursorModifierBagTextEditor(Key<TextEditorViewModel>.Empty, cursorList);

        await textEditorService.PostSimpleBatch(
            nameof(textEditorService.ModelApi.InsertTextUnsafeFactory),
            string.Empty,
            textEditorService.ModelApi.InsertTextUnsafeFactory(
                inModel.ResourceUri,
                cursorModifierBag,
                insertedText,
                CancellationToken.None));

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(insertedText + inModel.GetAllText(), outModel.GetAllText());
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.HandleKeyboardEventFactory(ResourceUri, Key{TextEditorViewModel}, KeyboardEventArgs, CancellationToken)"/>
    /// </summary>
    [Fact]
    public async Task HandleKeyboardEventEnqueue()
    {
        InitializeTextEditorModelApiTests(
            out var inModel,
            out var inViewModel,
            out var textEditorService,
            out var textEditorOptionsStateWrap,
            out var dispatcher,
            out var serviceProvider);

        var cursor = new TextEditorCursor(0, 0, 0, true, TextEditorSelection.Empty);
        var cursorList = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        var key = "a";

        var keyboardEventArgs = new KeyboardEventArgs
        {
            Key = key
        };

        var cursorModifierBag = new CursorModifierBagTextEditor(Key<TextEditorViewModel>.Empty, cursorList);

        await textEditorService.PostSimpleBatch(
            nameof(textEditorService.ModelApi.HandleKeyboardEventUnsafeFactory),
            string.Empty,
            textEditorService.ModelApi.HandleKeyboardEventUnsafeFactory(
                inModel.ResourceUri,
                Key<TextEditorViewModel>.Empty,
                keyboardEventArgs,
                CancellationToken.None,
                cursorModifierBag));

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(key + inModel.GetAllText(), outModel.GetAllText());
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.GetViewModelsOrEmpty(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void GetViewModelsOrEmpty()
    {
        InitializeTextEditorModelApiTests(
            out var inModel,
            out var inViewModel,
            out var textEditorService,
            out var textEditorOptionsStateWrap,
            out var dispatcher,
            out var serviceProvider);

        Assert.Empty(textEditorService.ModelApi.GetViewModelsOrEmpty(inModel.ResourceUri));

        textEditorService.ViewModelApi.Register(
            Key<TextEditorViewModel>.NewKey(),
            inModel.ResourceUri,
            new Category("UnitTesting"));

        Assert.Single(textEditorService.ModelApi.GetViewModelsOrEmpty(inModel.ResourceUri));
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.GetAllText(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void GetAllText()
    {
        InitializeTextEditorModelApiTests(
            out var inModel,
            out var inViewModel,
            out var textEditorService,
            out var textEditorOptionsStateWrap,
            out var dispatcher,
            out var serviceProvider);

        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;
        var initialContent = "Hello World!";

        textEditorService.ModelApi.RegisterTemplated(
            fileExtension,
            resourceUri,
            resourceLastWriteTime,
            initialContent);

        var model = textEditorService.ModelApi.GetOrDefault(resourceUri);
        Assert.Equal(initialContent, model.GetAllText());
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.GetOrDefault(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void GetOrDefault()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.NotNull(outModel);
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.Dispose(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void Dispose()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var model,
            out var inViewModel,
            out var serviceProvider);

        Assert.Single(textEditorService.ModelApi.GetModels());

        textEditorService.ModelApi.Dispose(model.ResourceUri);
        Assert.Empty(textEditorService.ModelApi.GetModels());
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.DeleteTextByRangeFactory(ResourceUri, Key{TextEditorViewModel}, int, CancellationToken)"/>
    /// </summary>
    [Fact]
    public async Task DeleteTextByRangeEnqueue()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var textToDelete = " World!";
        var expectedContent = inModel.GetAllText().Replace(textToDelete, string.Empty);

        var columnIndex = inModel.GetAllText().IndexOf(textToDelete);

        var cursor = new TextEditorCursor(0, columnIndex, columnIndex, true, TextEditorSelection.Empty);
        var cursorList = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        var cursorModifierBag = new CursorModifierBagTextEditor(Key<TextEditorViewModel>.Empty, cursorList);

        await textEditorService.PostSimpleBatch(
            nameof(textEditorService.ModelApi.DeleteTextByRangeUnsafeFactory),
            string.Empty,
            textEditorService.ModelApi.DeleteTextByRangeUnsafeFactory(
                inModel.ResourceUri,
                cursorModifierBag,
                textToDelete.Length,
                CancellationToken.None));

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(expectedContent, outModel!.GetAllText());
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.DeleteTextByMotionFactory(ResourceUri, Key{TextEditorViewModel}, MotionKind, CancellationToken)"/>
    /// </summary>
    [Fact]
    public async Task DeleteTextByMotionEnqueue_Backspace()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        // "Hello World" -> "HelloWorld" is the expected output
        var wordToJoin = "World!";
        var expectedContent = "HelloWorld!";

        var columnIndex = inModel.GetAllText().IndexOf(wordToJoin);

        var cursor = new TextEditorCursor(0, columnIndex, columnIndex, true, TextEditorSelection.Empty);
        var cursorList = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        var cursorModifierBag = new CursorModifierBagTextEditor(Key<TextEditorViewModel>.Empty, cursorList);

        await textEditorService.PostSimpleBatch(
            nameof(textEditorService.ModelApi.DeleteTextByMotionUnsafeFactory),
            string.Empty,
            textEditorService.ModelApi.DeleteTextByMotionUnsafeFactory(
                inModel.ResourceUri,
                cursorModifierBag,
                MotionKind.Backspace,
                CancellationToken.None));

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(expectedContent, outModel!.GetAllText());
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.AddPresentationModelFactory(ResourceUri, TextEditorPresentationModel)"/>
    /// </summary>
    [Fact]
    public async Task RegisterPresentationModel()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.Empty(inModel!.PresentationModelList);

        await textEditorService.PostSimpleBatch(
            nameof(textEditorService.ModelApi.AddPresentationModelFactory),
            string.Empty,
            textEditorService.ModelApi.AddPresentationModelFactory(inModel.ResourceUri, DiffPresentationFacts.EmptyOutPresentationModel));

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.NotEmpty(outModel!.PresentationModelList);
    }

    private void InitializeTextEditorModelApiTests(
        out TextEditorModel model,
        out TextEditorViewModel viewModel,
        out ITextEditorService textEditorService,
        out IState<TextEditorOptionsState> textEditorOptionsState,
        out IDispatcher dispatcher,
        out ServiceProvider serviceProvider)
    {
        var backgroundTaskService = new BackgroundTaskServiceSynchronous();

        var serviceCollection = new ServiceCollection()
            .AddScoped<IJSRuntime, DoNothingJsRuntime>()
            .AddLuthetusTextEditor(new LuthetusHostingInformation(LuthetusHostingKind.UnitTestingSynchronous, backgroundTaskService))
            .AddFluxor(options => options.ScanAssemblies(
                typeof(LuthetusCommonConfig).Assembly,
                typeof(LuthetusTextEditorConfig).Assembly));

        serviceProvider = serviceCollection.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        textEditorService = serviceProvider.GetRequiredService<ITextEditorService>();
        textEditorOptionsState = serviceProvider.GetRequiredService<IState<TextEditorOptionsState>>();

        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;
        var content = "Hello World!";

        model = new TextEditorModel(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            content,
            null,
            null,
            4096);

        textEditorService.ModelApi.RegisterCustom(model);
        
        var viewModelKey = Key<TextEditorViewModel>.NewKey();

        textEditorService.ViewModelApi.Register(
            viewModelKey,
            resourceUri,
            new Category("UnitTesting"));

        viewModel = textEditorService.ViewModelApi.GetOrDefault(viewModelKey)
           ?? throw new ArgumentNullException();
    }
}