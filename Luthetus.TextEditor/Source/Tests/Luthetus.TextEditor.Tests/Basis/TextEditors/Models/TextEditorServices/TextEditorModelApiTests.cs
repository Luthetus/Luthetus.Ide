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
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.NotNull(textEditorService.ModelApi);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.UndoEditEnqueue(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void UndoEdit()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var cursor = new TextEditorCursor(0, 0, 0, true, TextEditorSelection.Empty);
        var cursorBag = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        var insertedText = "I have something to say: ";

        textEditorService.EnqueueEdit(editContext =>
        {
            return textEditorService.ModelApi
                .InsertText(
                    new TextEditorModelState.InsertTextAction(
                        inModel.ResourceUri,
                        null,
                        cursorBag,
                        insertedText,
                        CancellationToken.None,
                        editContext.AuthenticatedActionKey),
                    editContext.RefreshCursorsRequest)
                .Invoke(editContext);
        });

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(
            insertedText + inModel.GetAllText(),
            outModel.GetAllText());

        textEditorService.EnqueueEdit(
            textEditorService.ModelApi.UndoEdit(
                inModel.ResourceUri));

        outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(inModel.GetAllText(), outModel.GetAllText());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.SetUsingRowEndingKindEnqueue(ResourceUri, RowEndingKind)"/>
    /// </summary>
    [Fact]
    public void SetUsingRowEndingKind()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var rowEndingKind = RowEndingKind.CarriageReturn;

        // Assert the current values are different from that which will be set.
        Assert.NotEqual(rowEndingKind, inModel.UsingRowEndingKind);

        textEditorService.EnqueueEdit(
            textEditorService.ModelApi.SetUsingRowEndingKind(
                inModel.ResourceUri, rowEndingKind));

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);

        // Assert the value is now set
        Assert.Equal(rowEndingKind, outModel!.UsingRowEndingKind);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.SetResourceDataEnqueue(ResourceUri, DateTime)"/>
    /// </summary>
    [Fact]
    public void SetResourceData()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var newResourceLastWriteTime = inModel.ResourceLastWriteTime.AddDays(1);

        // Assert the current values are different from that which will be set.
        Assert.NotEqual(newResourceLastWriteTime, inModel.ResourceLastWriteTime);

        textEditorService.EnqueueEdit(
            textEditorService.ModelApi.SetResourceData(
                inModel.ResourceUri, newResourceLastWriteTime));

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);

        // Assert the values are now set
        Assert.Equal(newResourceLastWriteTime, outModel!.ResourceLastWriteTime);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.ReloadEnqueue(ResourceUri, string, DateTime)"/>
    /// </summary>
    [Fact]
    public void Reload()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var newContent = "Alphabet Soup";

        // Assert the current values are different from that which will be set.
        Assert.NotEqual(newContent, inModel.GetAllText());

        textEditorService.EnqueueEdit(
            textEditorService.ModelApi.Reload(
                inModel.ResourceUri, newContent, DateTime.UtcNow));
        
        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);

        // Assert the values are now set
        Assert.Equal(newContent, outModel!.GetAllText());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.RegisterCustom(TextEditorModel)"/>
    /// </summary>
    [Fact]
    public void RegisterCustom()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out _,
            out var inViewModel,
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
    /// <see cref="ITextEditorService.TextEditorModelApi.RegisterTemplated(string, ResourceUri, DateTime, string, string?)"/>
    /// </summary>
    [Fact]
    public void RegisterTemplated()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out _,
            out var inViewModel,
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
    /// <see cref="ITextEditorService.TextEditorModelApi.RedoEditEnqueue(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void RedoEditEnqueue()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var cursor = new TextEditorCursor(0, 0, 0, true, TextEditorSelection.Empty);
        var cursorBag = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        var insertedText = "I have something to say: ";

        textEditorService.EnqueueEdit(editContext =>
        {
            return textEditorService.ModelApi
                .InsertText(
                    new TextEditorModelState.InsertTextAction(
                        inModel.ResourceUri,
                        null,
                        cursorBag,
                        insertedText,
                        CancellationToken.None,
                        editContext.AuthenticatedActionKey),
                    editContext.RefreshCursorsRequest)
                .Invoke(editContext);
        });

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(insertedText + inModel.GetAllText(), outModel.GetAllText());

        textEditorService.EnqueueEdit(
            textEditorService.ModelApi.UndoEdit(
                inModel.ResourceUri));

        outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(inModel.GetAllText(), outModel.GetAllText());

        textEditorService.EnqueueEdit(
            textEditorService.ModelApi.RedoEdit(
                inModel.ResourceUri));

        outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(insertedText + inModel.GetAllText(), outModel.GetAllText());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.InsertTextEnqueue(TextEditorModelState.InsertTextAction)"/>
    /// </summary>
    [Fact]
    public void InsertTextEnqueue()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var cursor = new TextEditorCursor(0, 0, 0, true, TextEditorSelection.Empty);
        var cursorBag = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        var insertedText = "I have something to say: ";

        textEditorService.EnqueueEdit(editContext =>
        {
            return textEditorService.ModelApi
                .InsertText(
                    new TextEditorModelState.InsertTextAction(
                        inModel.ResourceUri,
                        null,
                        cursorBag,
                        insertedText,
                        CancellationToken.None,
                        editContext.AuthenticatedActionKey),
                    editContext.RefreshCursorsRequest)
                .Invoke(editContext);
        });

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(insertedText + inModel.GetAllText(), outModel.GetAllText());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.HandleKeyboardEventEnqueue(TextEditorModelState.KeyboardEventAction)"/>
    /// </summary>
    [Fact]
    public void HandleKeyboardEventEnqueue()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var cursor = new TextEditorCursor(0, 0, 0, true, TextEditorSelection.Empty);
        var cursorBag = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        var key = "a";

        var keyboardEventArgs = new KeyboardEventArgs
        {
            Key = key
        };

        textEditorService.EnqueueEdit(editContext =>
        {
            return textEditorService.ModelApi
                .HandleKeyboardEvent(
                    new TextEditorModelState.KeyboardEventAction(
                        inModel.ResourceUri,
                        null,
                        cursorBag,
                        keyboardEventArgs,
                        CancellationToken.None,
                        editContext.AuthenticatedActionKey),
                    editContext.RefreshCursorsRequest)
                .Invoke(editContext);
        });

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(key + inModel.GetAllText(), outModel.GetAllText());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.GetViewModelsOrEmpty(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void GetViewModelsOrEmpty()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.Empty(textEditorService.ModelApi.GetViewModelsOrEmpty(inModel.ResourceUri));

        textEditorService.ViewModelApi.Register(
            Key<TextEditorViewModel>.NewKey(),
            inModel.ResourceUri);
        
        Assert.Single(textEditorService.ModelApi.GetViewModelsOrEmpty(inModel.ResourceUri));
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.GetAllText(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void GetAllText()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out _,
            out var inViewModel,
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
    /// <see cref="ITextEditorService.TextEditorModelApi.GetOrDefault(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void GetOrDefault()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.NotNull(outModel);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.Dispose(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void Dispose()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var model,
            out var inViewModel,
            out var serviceProvider);

        Assert.Single(textEditorService.ModelApi.GetModels());

        textEditorService.ModelApi.Dispose(model.ResourceUri);
        Assert.Empty(textEditorService.ModelApi.GetModels());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.DeleteTextByRangeEnqueue(TextEditorModelState.DeleteTextByRangeAction)"/>
    /// </summary>
    [Fact]
    public void DeleteTextByRangeEnqueue()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var textToDelete = " World!";
        var expectedContent = inModel.GetAllText().Replace(textToDelete, string.Empty);

        var columnIndex = inModel.GetAllText().IndexOf(textToDelete);

        var cursor = new TextEditorCursor(0, columnIndex, columnIndex, true, TextEditorSelection.Empty);
        var cursorBag = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        textEditorService.EnqueueEdit(editContext =>
        {
            return textEditorService.ModelApi
                .DeleteTextByRange(
                    new TextEditorModelState.DeleteTextByRangeAction(
                        inModel.ResourceUri,
                        null,
                        cursorBag,
                        textToDelete.Length,
                        CancellationToken.None,
                        editContext.AuthenticatedActionKey),
                    editContext.RefreshCursorsRequest)
                .Invoke(editContext);
        });

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(expectedContent, outModel!.GetAllText());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.DeleteTextByMotionEnqueue(TextEditorModelState.DeleteTextByMotionAction)"/>
    /// </summary>
    [Fact]
    public void DeleteTextByMotionEnqueue_Backspace()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        // "Hello World" -> "HelloWorld" is the expected output
        var wordToJoin = "World!";
        var expectedContent = "HelloWorld!";

        var columnIndex = inModel.GetAllText().IndexOf(wordToJoin);

        var cursor = new TextEditorCursor(0, columnIndex, columnIndex, true, TextEditorSelection.Empty);
        var cursorBag = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        textEditorService.EnqueueEdit(editContext =>
        {
            return textEditorService.ModelApi
                .DeleteTextByMotion(
                    new TextEditorModelState.DeleteTextByMotionAction(
                        inModel.ResourceUri,
                        null,
                        cursorBag,
                        MotionKind.Backspace,
                        CancellationToken.None,
                        editContext.AuthenticatedActionKey),
                    editContext.RefreshCursorsRequest)
                .Invoke(editContext);
        });

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(expectedContent, outModel!.GetAllText());
    }
    
    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.DeleteTextByMotionEnqueue(TextEditorModelState.DeleteTextByMotionAction)"/>
    /// </summary>
    [Fact]
    public void DeleteTextByMotionEnqueue_Delete()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        // "Hello World" -> "HelloWorld" is the expected output
        var spaceText = " ";
        var expectedContent = "HelloWorld!";

        var columnIndex = inModel.GetAllText().IndexOf(spaceText);

        var cursor = new TextEditorCursor(0, columnIndex, columnIndex, true, TextEditorSelection.Empty);
        var cursorBag = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        textEditorService.EnqueueEdit(editContext =>
        {
            return textEditorService.ModelApi
                .DeleteTextByMotion(
                    new TextEditorModelState.DeleteTextByMotionAction(
                        inModel.ResourceUri,
                        null,
                        cursorBag,
                        MotionKind.Delete,
                        CancellationToken.None,
                        editContext.AuthenticatedActionKey),
                    editContext.RefreshCursorsRequest)
                .Invoke(editContext);
        });

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.Equal(expectedContent, outModel!.GetAllText());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorModelApi.RegisterPresentationModel(ResourceUri, TextEditorPresentationModel)"/>
    /// </summary>
    [Fact]
    public void RegisterPresentationModel()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.Empty(inModel!.PresentationModelsBag);
        
        textEditorService.ModelApi.RegisterPresentationModel(inModel.ResourceUri, DiffPresentationFacts.EmptyOutPresentationModel);

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.NotEmpty(outModel!.PresentationModelsBag);
    }
}