using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;

/// <summary>
/// <see cref="ITextEditorService.TextEditorModelApi"/>
/// </summary>
public class TextEditorModelApiTests
{
    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.TextEditorModelApi(ITextEditorService, IDecorationMapperRegistry, ICompilerServiceRegistry, Common.RazorLib.BackgroundTasks.Models.IBackgroundTaskService, Fluxor.IDispatcher)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
            out var serviceProvider);

        Assert.NotNull(textEditorService.ModelApi);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.UndoEditEnqueue(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void UndoEdit()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
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

        var originalModel = textEditorService.ModelApi.GetModels().Single();

        var cursor = new TextEditorCursor(0, 0, 0, true, TextEditorSelection.Empty);
        var cursorBag = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        var insertedText = "I have something to say: ";

        textEditorService.ModelApi.InsertTextEnqueue(new TextEditorModelState.InsertTextAction(
            resourceUri,
            null,
            cursorBag,
            insertedText,
            CancellationToken.None));

        var modifiedModel = textEditorService.ModelApi.GetModels().Single();
        Assert.Equal(
            insertedText + originalModel.GetAllText(),
            modifiedModel.GetAllText());

        textEditorService.ModelApi.UndoEditEnqueue(resourceUri);

        modifiedModel = textEditorService.ModelApi.GetModels().Single();
        Assert.Equal(initialContent, modifiedModel.GetAllText());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.SetUsingRowEndingKindEnqueue(ResourceUri, RowEndingKind)"/>
    /// </summary>
    [Fact]
    public void SetUsingRowEndingKind()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
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

        Assert.Single(textEditorService.ModelApi.GetModels());
        var existingModel = textEditorService.ModelApi.GetModels().Single();

        var rowEndingKind = RowEndingKind.CarriageReturn;

        // Assert the current values are different from that which will be set.
        Assert.NotEqual(rowEndingKind, existingModel.UsingRowEndingKind);

        textEditorService.ModelApi.SetUsingRowEndingKindEnqueue(resourceUri, rowEndingKind);
        existingModel = textEditorService.ModelApi.GetOrDefault(resourceUri);

        // Assert the value is now set
        Assert.Equal(rowEndingKind, existingModel!.UsingRowEndingKind);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.SetResourceDataEnqueue(ResourceUri, DateTime)"/>
    /// </summary>
    [Fact]
    public void SetResourceData()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
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

        Assert.Single(textEditorService.ModelApi.GetModels());
        var existingModel = textEditorService.ModelApi.GetModels().Single();

        var newResourceLastWriteTime = resourceLastWriteTime.AddDays(1);

        // Assert the current values are different from that which will be set.
        Assert.NotEqual(newResourceLastWriteTime, existingModel.ResourceLastWriteTime);

        textEditorService.ModelApi.SetResourceDataEnqueue(resourceUri, newResourceLastWriteTime);
        existingModel = textEditorService.ModelApi.GetOrDefault(resourceUri);

        // Assert the values are now set
        Assert.Equal(newResourceLastWriteTime, existingModel!.ResourceLastWriteTime);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.ReloadEnqueue(ResourceUri, string, DateTime)"/>
    /// </summary>
    [Fact]
    public void Reload()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
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

        Assert.Single(textEditorService.ModelApi.GetModels());
        var existingModel = textEditorService.ModelApi.GetModels().Single();

        var newContent = "Alphabet Soup";

        // Assert the current values are different from that which will be set.
        Assert.NotEqual(newContent, existingModel.GetAllText());

        textEditorService.ModelApi.ReloadEnqueue(resourceUri, newContent, DateTime.UtcNow);
        existingModel = textEditorService.ModelApi.GetOrDefault(resourceUri);

        // Assert the values are now set
        Assert.Equal(newContent, existingModel!.GetAllText());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.RegisterCustom(TextEditorModel)"/>
    /// </summary>
    [Fact]
    public void RegisterCustom()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
            out var serviceProvider);

        Assert.Empty(textEditorService.ModelApi.GetModels());

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

        Assert.Single(textEditorService.ModelApi.GetModels());
        var existingModel = textEditorService.ModelApi.GetModels().Single();

        Assert.Equal(resourceUri, existingModel.ResourceUri);
        Assert.Equal(resourceLastWriteTime, existingModel.ResourceLastWriteTime);
        Assert.Equal(initialContent, existingModel.GetAllText());
        Assert.Equal(fileExtension, existingModel.FileExtension);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.RegisterTemplated(string, ResourceUri, DateTime, string, string?)"/>
    /// </summary>
    [Fact]
    public void RegisterTemplated()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
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

        Assert.Single(textEditorService.ModelApi.GetModels());
        var existingModel = textEditorService.ModelApi.GetModels().Single();

        Assert.Equal(resourceUri, existingModel.ResourceUri);
        Assert.Equal(resourceLastWriteTime, existingModel.ResourceLastWriteTime);
        Assert.Equal(initialContent, existingModel.GetAllText());
        Assert.Equal(fileExtension, existingModel.FileExtension);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.RedoEditEnqueue(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void RedoEditEnqueue()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
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

        var originalModel = textEditorService.ModelApi.GetModels().Single();

        var cursor = new TextEditorCursor(0, 0, 0, true, TextEditorSelection.Empty);
        var cursorBag = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        var insertedText = "I have something to say: ";

        textEditorService.ModelApi.InsertTextEnqueue(new TextEditorModelState.InsertTextAction(
            resourceUri,
            null,
            cursorBag,
            insertedText,
            CancellationToken.None));

        var modifiedModel = textEditorService.ModelApi.GetModels().Single();
        Assert.Equal(
            insertedText + originalModel.GetAllText(),
            modifiedModel.GetAllText());

        textEditorService.ModelApi.UndoEditEnqueue(resourceUri);

        modifiedModel = textEditorService.ModelApi.GetModels().Single();
        Assert.Equal(initialContent, modifiedModel.GetAllText());

        textEditorService.ModelApi.RedoEditEnqueue(resourceUri);

        modifiedModel = textEditorService.ModelApi.GetModels().Single();
        Assert.Equal(
            insertedText + originalModel.GetAllText(),
            modifiedModel.GetAllText());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.InsertTextEnqueue(TextEditorModelState.InsertTextAction)"/>
    /// </summary>
    [Fact]
    public void InsertTextEnqueue()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
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

        var originalModel = textEditorService.ModelApi.GetModels().Single();

        var cursor = new TextEditorCursor(0, 0, 0, true, TextEditorSelection.Empty);
        var cursorBag = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        var insertedText = "I have something to say: ";

        textEditorService.ModelApi.InsertTextEnqueue(new TextEditorModelState.InsertTextAction(
            resourceUri,
            null,
            cursorBag,
            insertedText,
            CancellationToken.None));

        var modifiedModel = textEditorService.ModelApi.GetModels().Single();

        Assert.Equal(
            insertedText + originalModel.GetAllText(),
            modifiedModel.GetAllText());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.HandleKeyboardEventEnqueue(TextEditorModelState.KeyboardEventAction)"/>
    /// </summary>
    [Fact]
    public void HandleKeyboardEventEnqueue()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
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

        var originalModel = textEditorService.ModelApi.GetModels().Single();

        var cursor = new TextEditorCursor(0, 0, 0, true, TextEditorSelection.Empty);
        var cursorBag = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        var key = "a";

        var keyboardEventArgs = new KeyboardEventArgs
        {
            Key = key
        };

        textEditorService.ModelApi.HandleKeyboardEventEnqueue(new TextEditorModelState.KeyboardEventAction(
            resourceUri,
            null,
            cursorBag,
            keyboardEventArgs,
            CancellationToken.None));

        var modifiedModel = textEditorService.ModelApi.GetModels().Single();

        Assert.Equal(
            key + originalModel.GetAllText(),
            modifiedModel.GetAllText());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.GetViewModelsOrEmpty(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void GetViewModelsOrEmpty()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
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

        var model = textEditorService.ModelApi.GetModels().Single();

        Assert.Empty(textEditorService.ModelApi.GetViewModelsOrEmpty(model.ResourceUri));

        textEditorService.ViewModelApi.Register(
            Key<TextEditorViewModel>.NewKey(),
            model.ResourceUri);
        
        Assert.Single(textEditorService.ModelApi.GetViewModelsOrEmpty(model.ResourceUri));
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.GetAllText(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void GetAllText()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
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

        var model = textEditorService.ModelApi.GetModels().Single();

        Assert.Equal(initialContent, model.GetAllText());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.GetOrDefault(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void GetOrDefault()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
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
        Assert.NotNull(model);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.Dispose(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void Dispose()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
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

        Assert.Single(textEditorService.ModelApi.GetModels());

        textEditorService.ModelApi.Dispose(resourceUri);
        Assert.Empty(textEditorService.ModelApi.GetModels());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.DeleteTextByRangeEnqueue(TextEditorModelState.DeleteTextByRangeAction)"/>
    /// </summary>
    [Fact]
    public void DeleteTextByRangeEnqueue()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
            out var serviceProvider);

        Assert.Empty(textEditorService.ModelApi.GetModels());

        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;
        var initialContent = "Hello World!";

        var textToDelete = " World!";
        var expectedContent = initialContent.Replace(textToDelete, string.Empty);

        textEditorService.ModelApi.RegisterTemplated(
            fileExtension,
            resourceUri,
            resourceLastWriteTime,
            initialContent);

        var model = textEditorService.ModelApi.GetOrDefault(resourceUri);

        var columnIndex = initialContent.IndexOf(textToDelete);

        var cursor = new TextEditorCursor(0, columnIndex, columnIndex, true, TextEditorSelection.Empty);
        var cursorBag = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        textEditorService.ModelApi.DeleteTextByRangeEnqueue(new TextEditorModelState.DeleteTextByRangeAction(
            resourceUri,
            null,
            cursorBag,
            textToDelete.Length,
            CancellationToken.None));

        model = textEditorService.ModelApi.GetOrDefault(resourceUri);

        Assert.Equal(expectedContent, model!.GetAllText());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.DeleteTextByMotionEnqueue(TextEditorModelState.DeleteTextByMotionAction)"/>
    /// </summary>
    [Fact]
    public void DeleteTextByMotionEnqueue_Backspace()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
            out var serviceProvider);

        Assert.Empty(textEditorService.ModelApi.GetModels());

        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;
        var initialContent = "Hello World!";

        // "Hello World" -> "HelloWorld" is the expected output
        var wordToJoin = "World!";
        var expectedContent = "HelloWorld!";

        textEditorService.ModelApi.RegisterTemplated(
            fileExtension,
            resourceUri,
            resourceLastWriteTime,
            initialContent);

        var model = textEditorService.ModelApi.GetOrDefault(resourceUri);

        var columnIndex = initialContent.IndexOf(wordToJoin);

        var cursor = new TextEditorCursor(0, columnIndex, columnIndex, true, TextEditorSelection.Empty);
        var cursorBag = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        textEditorService.ModelApi.DeleteTextByMotionEnqueue(new TextEditorModelState.DeleteTextByMotionAction(
            resourceUri,
            null,
            cursorBag,
            MotionKind.Backspace,
            CancellationToken.None));

        model = textEditorService.ModelApi.GetOrDefault(resourceUri);
        Assert.Equal(expectedContent, model!.GetAllText());
    }
    
    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.DeleteTextByMotionEnqueue(TextEditorModelState.DeleteTextByMotionAction)"/>
    /// </summary>
    [Fact]
    public void DeleteTextByMotionEnqueue_Delete()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
            out var serviceProvider);

        Assert.Empty(textEditorService.ModelApi.GetModels());

        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;
        var initialContent = "Hello World!";

        // "Hello World" -> "HelloWorld" is the expected output
        var spaceText = " ";
        var expectedContent = "HelloWorld!";

        textEditorService.ModelApi.RegisterTemplated(
            fileExtension,
            resourceUri,
            resourceLastWriteTime,
            initialContent);

        var model = textEditorService.ModelApi.GetOrDefault(resourceUri);

        var columnIndex = initialContent.IndexOf(spaceText);

        var cursor = new TextEditorCursor(0, columnIndex, columnIndex, true, TextEditorSelection.Empty);
        var cursorBag = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        textEditorService.ModelApi.DeleteTextByMotionEnqueue(new TextEditorModelState.DeleteTextByMotionAction(
            resourceUri,
            null,
            cursorBag,
            MotionKind.Delete,
            CancellationToken.None));

        model = textEditorService.ModelApi.GetOrDefault(resourceUri);
        Assert.Equal(expectedContent, model!.GetAllText());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.RegisterPresentationModel(ResourceUri, TextEditorPresentationModel)"/>
    /// </summary>
    [Fact]
    public void RegisterPresentationModel()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
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
        Assert.Empty(model!.PresentationModelsBag);
        
        textEditorService.ModelApi.RegisterPresentationModel(resourceUri, DiffPresentationFacts.EmptyOutPresentationModel);

        model = textEditorService.ModelApi.GetOrDefault(resourceUri);
        Assert.NotEmpty(model!.PresentationModelsBag);
    }
}