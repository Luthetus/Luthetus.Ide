﻿using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models;

/// <summary>
/// <see cref="TextEditorModelApi"/>
/// </summary>
public class ModelApiTests
{
    /// <summary>
    /// <see cref="TextEditorModelApi(ITextEditorService, IDecorationMapperRegistry, ICompilerServiceRegistry, IBackgroundTaskService, IDispatcher)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
        //    out var textEditorService,
        //    out var inModel,
        //    out var inViewModel,
        //    out var serviceProvider);

        //Assert.NotNull(textEditorService.ModelApi);
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.UndoEditFactory(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void UndoEdit()
    {
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
        //    out var textEditorService,
        //    out var inModel,
        //    out var inViewModel,
        //    out var serviceProvider);

        //var cursor = new TextEditorCursor(0, 0, 0, true, TextEditorSelection.Empty);
        //var cursorList = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        //var insertedText = "I have something to say: ";

        //var cursorModifierBag = new TextEditorCursorModifierBag(Key<TextEditorViewModel>.Empty, cursorList);

        //textEditorService.Post(
        //    nameof(textEditorService.ModelApi.InsertTextUnsafeFactory),
        //    textEditorService.ModelApi.InsertTextUnsafeFactory(
        //        inModel.ResourceUri,
        //        cursorModifierBag,
        //        insertedText,
        //        CancellationToken.None));

        //var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        //Assert.Equal(
        //    insertedText + inModel.GetAllText(),
        //    outModel.GetAllText());

        //textEditorService.Post(nameof(textEditorService.ModelApi.UndoEditFactory),
        //    textEditorService.ModelApi.UndoEditFactory(
        //        inModel.ResourceUri));

        //outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        //Assert.Equal(inModel.GetAllText(), outModel.GetAllText());
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.SetUsingLineEndKindFactory(ResourceUri, LineEndKind)"/>
    /// </summary>
    [Fact]
    public void SetUsingRowEndingKind()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var rowEndingKind = LineEndKind.CarriageReturn;

        // Assert the current values are different from that which will be set.
        Assert.NotEqual(rowEndingKind, inModel.UsingLineEndKind);

        textEditorService.Post(
            nameof(textEditorService.ModelApi.SetUsingLineEndKindFactory),
            textEditorService.ModelApi.SetUsingLineEndKindFactory(
                inModel.ResourceUri, rowEndingKind));

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);

        // Assert the value is now set
        Assert.Equal(rowEndingKind, outModel!.UsingLineEndKind);
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.SetResourceDataFactory(ResourceUri, DateTime)"/>
    /// </summary>
    [Fact]
    public void SetResourceData()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var newResourceLastWriteTime = inModel.ResourceLastWriteTime.AddDays(1);

        // Assert the current values are different from that which will be set.
        Assert.NotEqual(newResourceLastWriteTime, inModel.ResourceLastWriteTime);

        textEditorService.Post(
            nameof(textEditorService.ModelApi.SetResourceDataFactory),
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
    public void Reload()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var newContent = "Alphabet Soup";

        // Assert the current values are different from that which will be set.
        Assert.NotEqual(newContent, inModel.GetAllText());

        textEditorService.Post(
            nameof(textEditorService.ModelApi.ReloadFactory),
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
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
        //    out var textEditorService,
        //    out _,
        //    out var inViewModel,
        //    out var serviceProvider);

        //var fileExtension = ExtensionNoPeriodFacts.TXT;
        //var resourceUri = new ResourceUri("/unitTesting.txt");
        //var resourceLastWriteTime = DateTime.UtcNow;
        //var initialContent = "Hello World!";

        //var decorationMapperRegistry = serviceProvider.GetRequiredService<IDecorationMapperRegistry>();
        //var compilerServiceRegistry = serviceProvider.GetRequiredService<ICompilerServiceRegistry>();

        //var decorationMapper = decorationMapperRegistry.GetDecorationMapper(fileExtension);
        //var compilerService = compilerServiceRegistry.GetCompilerService(fileExtension);

        //var model = new TextEditorModel(
        //    resourceUri,
        //    resourceLastWriteTime,
        //    fileExtension,
        //    initialContent,
        //    decorationMapper,
        //    compilerService);

        //textEditorService.ModelApi.RegisterCustom(model);

        //var existingModel = textEditorService.ModelApi.GetOrDefault(model.ResourceUri);

        //Assert.Equal(resourceUri, existingModel.ResourceUri);
        //Assert.Equal(resourceLastWriteTime, existingModel.ResourceLastWriteTime);
        //Assert.Equal(initialContent, existingModel.GetAllText());
        //Assert.Equal(fileExtension, existingModel.FileExtension);
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.RegisterTemplated(string, ResourceUri, DateTime, string, string?)"/>
    /// </summary>
    [Fact]
    public void RegisterTemplated()
    {
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
        //    out var textEditorService,
        //    out _,
        //    out var inViewModel,
        //    out var serviceProvider);

        //Assert.Empty(textEditorService.ModelApi.GetModels());

        //var fileExtension = ExtensionNoPeriodFacts.TXT;
        //var resourceUri = new ResourceUri("/unitTesting.txt");
        //var resourceLastWriteTime = DateTime.UtcNow;
        //var initialContent = "Hello World!";

        //textEditorService.ModelApi.RegisterTemplated(
        //    fileExtension,
        //    resourceUri,
        //    resourceLastWriteTime,
        //    initialContent);

        //var model = textEditorService.ModelApi.GetOrDefault(resourceUri);

        //Assert.Equal(resourceUri, model.ResourceUri);
        //Assert.Equal(resourceLastWriteTime, model.ResourceLastWriteTime);
        //Assert.Equal(initialContent, model.GetAllText());
        //Assert.Equal(fileExtension, model.FileExtension);
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.RedoEditFactory(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void RedoEditEnqueue()
    {
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
        //    out var textEditorService,
        //    out var inModel,
        //    out var inViewModel,
        //    out var serviceProvider);

        //var cursor = new TextEditorCursor(0, 0, 0, true, TextEditorSelection.Empty);
        //var cursorList = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        //var insertedText = "I have something to say: ";

        //var cursorModifierBag = new TextEditorCursorModifierBag(Key<TextEditorViewModel>.Empty, cursorList);

        //textEditorService.Post(
        //    nameof(textEditorService.ModelApi.InsertTextUnsafeFactory),
        //    textEditorService.ModelApi.InsertTextUnsafeFactory(
        //        inModel.ResourceUri,
        //        cursorModifierBag,
        //        insertedText,
        //        CancellationToken.None));

        //var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        //Assert.Equal(insertedText + inModel.GetAllText(), outModel.GetAllText());

        //textEditorService.Post(
        //    nameof(textEditorService.ModelApi.UndoEditFactory),
        //    textEditorService.ModelApi.UndoEditFactory(
        //        inModel.ResourceUri));

        //outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        //Assert.Equal(inModel.GetAllText(), outModel.GetAllText());

        //textEditorService.Post(
        //    nameof(textEditorService.ModelApi.RedoEditFactory),
        //    textEditorService.ModelApi.RedoEditFactory(
        //        inModel.ResourceUri));

        //outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        //Assert.Equal(insertedText + inModel.GetAllText(), outModel.GetAllText());
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.InsertTextFactory(ResourceUri, Key{TextEditorViewModel}, string, CancellationToken)"/>
    /// </summary>
    [Fact]
    public void InsertTextEnqueue()
    {
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
        //    out var textEditorService,
        //    out var inModel,
        //    out var inViewModel,
        //    out var serviceProvider);

        //var cursor = new TextEditorCursor(0, 0, 0, true, TextEditorSelection.Empty);
        //var cursorList = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        //var insertedText = "I have something to say: ";

        //var cursorModifierBag = new TextEditorCursorModifierBag(Key<TextEditorViewModel>.Empty, cursorList);

        //textEditorService.Post(
        //    nameof(textEditorService.ModelApi.InsertTextUnsafeFactory),
        //    textEditorService.ModelApi.InsertTextUnsafeFactory(
        //        inModel.ResourceUri,
        //        cursorModifierBag,
        //        insertedText,
        //        CancellationToken.None));

        //var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        //Assert.Equal(insertedText + inModel.GetAllText(), outModel.GetAllText());
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.HandleKeyboardEventFactory(ResourceUri, Key{TextEditorViewModel}, KeyboardEventArgs, CancellationToken)"/>
    /// </summary>
    [Fact]
    public void HandleKeyboardEventEnqueue()
    {
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
        //    out var textEditorService,
        //    out var inModel,
        //    out var inViewModel,
        //    out var serviceProvider);

        //var cursor = new TextEditorCursor(0, 0, 0, true, TextEditorSelection.Empty);
        //var cursorList = new[] { new TextEditorCursorModifier(cursor) }.ToList();

        //var key = "a";

        //var keyboardEventArgs = new KeyboardEventArgs
        //{
        //    Key = key
        //};

        //var cursorModifierBag = new TextEditorCursorModifierBag(Key<TextEditorViewModel>.Empty, cursorList);

        //textEditorService.Post(
        //    nameof(textEditorService.ModelApi.HandleKeyboardEventUnsafeFactory),
        //    textEditorService.ModelApi.HandleKeyboardEventUnsafeFactory(
        //        inModel.ResourceUri,
        //        Key<TextEditorViewModel>.Empty,
        //        keyboardEventArgs,
        //        CancellationToken.None,
        //        cursorModifierBag));

        //var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        //Assert.Equal(key + inModel.GetAllText(), outModel.GetAllText());
    }

    /// <summary>
    /// <see cref="TextEditorModelApi.GetViewModelsOrEmpty(ResourceUri)"/>
    /// </summary>
    [Fact]
    public void GetViewModelsOrEmpty()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
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
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
        //    out var textEditorService,
        //    out _,
        //    out var inViewModel,
        //    out var serviceProvider);

        //var fileExtension = ExtensionNoPeriodFacts.TXT;
        //var resourceUri = new ResourceUri("/unitTesting.txt");
        //var resourceLastWriteTime = DateTime.UtcNow;
        //var initialContent = "Hello World!";

        //textEditorService.ModelApi.RegisterTemplated(
        //    fileExtension,
        //    resourceUri,
        //    resourceLastWriteTime,
        //    initialContent);

        //var model = textEditorService.ModelApi.GetOrDefault(resourceUri);
        //Assert.Equal(initialContent, model.GetAllText());
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
    public void DeleteTextByRangeEnqueue()
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

        textEditorService.Post(
            nameof(textEditorService.ModelApi.DeleteTextByRangeUnsafeFactory),
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
    public void DeleteTextByMotionEnqueue_Backspace()
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

        textEditorService.Post(
            nameof(textEditorService.ModelApi.DeleteTextByMotionUnsafeFactory),
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
    public void RegisterPresentationModel()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.Empty(inModel!.PresentationModelList);

        textEditorService.Post(
            nameof(textEditorService.ModelApi.AddPresentationModelFactory),
            textEditorService.ModelApi.AddPresentationModelFactory(inModel.ResourceUri, DiffPresentationFacts.EmptyOutPresentationModel));

        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        Assert.NotEmpty(outModel!.PresentationModelList);
    }
}