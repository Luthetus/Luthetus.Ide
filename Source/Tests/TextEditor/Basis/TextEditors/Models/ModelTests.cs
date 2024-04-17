using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models;

/// <summary>
/// <see cref="TextEditorModel"/>
/// </summary>
public class ModelTests
{
    /// <summary>
    /// <see cref="TextEditorModel(ResourceUri, DateTime, string, string, RazorLib.Decorations.Models.IDecorationMapper?, Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces.ILuthCompilerService?, Common.RazorLib.Keymaps.Models.Keymap?)"/>
    /// </summary>
    [Fact]
    public void Constructor_New()
    {
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //var resourceUri = new ResourceUri("/unitTests.txt");
        //var resourceLastWriteTime = DateTime.UtcNow;
        //var fileExtension = ".txt";
        //var content = "Hello World!";
        //var decorationMapper = new TextEditorDecorationMapperDefault();
        //var compilerService = new LuthCompilerService(null);

        //      var model = new TextEditorModel(
        //          resourceUri,
        //          resourceLastWriteTime,
        //          fileExtension,
        //          content,
        //          decorationMapper,
        //          compilerService);

        //Assert.Equal(resourceUri, model.ResourceUri);
        //Assert.Equal(resourceLastWriteTime, model.ResourceLastWriteTime);
        //Assert.Equal(fileExtension, model.FileExtension);
        //Assert.Equal(content, model.GetAllText());
        //Assert.Equal(decorationMapper, model.DecorationMapper);
        //Assert.Equal(compilerService, model.CompilerService);
    }

    /// <summary>
    /// <see cref="TextEditorModel(System.Collections.Immutable.ImmutableList{RazorLib.Characters.Models.RichCharacter}, System.Collections.Immutable.ImmutableList{RazorLib.Edits.Models.EditBlock}, System.Collections.Immutable.ImmutableList{ValueTuple{int, RazorLib.Rows.Models.LineEndKind}}, System.Collections.Immutable.ImmutableList{ValueTuple{RazorLib.Rows.Models.LineEndKind, int}}, System.Collections.Immutable.ImmutableList{RazorLib.Decorations.Models.TextEditorPresentationModel}, System.Collections.Immutable.ImmutableList{int}, RazorLib.Rows.Models.LineEndKind?, RazorLib.Rows.Models.LineEndKind, RazorLib.Lexes.Models.ResourceUri, DateTime, string, RazorLib.Decorations.Models.IDecorationMapper, ILuthCompilerService, SaveFileHelper, int, ValueTuple{int, int}, Common.RazorLib.Keys.Models.Key{Common.RazorLib.RenderStates.Models.RenderState}, Common.RazorLib.Keymaps.Models.Keymap, RazorLib.Options.Models.TextEditorOptions?)"/>
    /// </summary>
    [Fact]
    public void Constructor_Clone()
    {
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //      var originalModel = new TextEditorModel(
        //          new ResourceUri("/unitTests.txt"),
        //          DateTime.UtcNow,
        //          ".txt",
        //          "Hello World!",
        //          new TextEditorDecorationMapperDefault(),
        //          new LuthCompilerService(null));

        //var cloneModel = new TextEditorModel(
        //	originalModel.ContentList,
        //	originalModel.PartitionSize,
        //	originalModel.PartitionList,
        //	originalModel.EditBlocksList,
        //          originalModel.RowEndingPositionsList,
        //          originalModel.RowEndingKindCountsList,
        //          originalModel.PresentationModelsList,
        //          originalModel.TabKeyPositionsList,
        //          originalModel.OnlyRowEndingKind,
        //	originalModel.UsingRowEndingKind,
        //	originalModel.ResourceUri,
        //	originalModel.ResourceLastWriteTime,
        //	originalModel.FileExtension,
        //	originalModel.DecorationMapper,
        //	originalModel.CompilerService,
        //	originalModel.TextEditorSaveFileHelper,
        //	originalModel.EditBlockIndex,
        //	originalModel.IsDirty,
        //	originalModel.MostCharactersOnASingleRowTuple,
        //	originalModel.RenderStateKey);

        //      Assert.Equal(originalModel.ContentList, cloneModel.ContentList);
        //Assert.Equal(originalModel.EditBlocksList, cloneModel.EditBlocksList);
        //      Assert.Equal(originalModel.RowEndingPositionsList, cloneModel.RowEndingPositionsList);
        //      Assert.Equal(originalModel.RowEndingKindCountsList, cloneModel.RowEndingKindCountsList);
        //      Assert.Equal(originalModel.PresentationModelsList, cloneModel.PresentationModelsList);
        //      Assert.Equal(originalModel.TabKeyPositionsList, cloneModel.TabKeyPositionsList);
        //      Assert.Equal(originalModel.OnlyRowEndingKind, cloneModel.OnlyRowEndingKind);
        //Assert.Equal(originalModel.UsingRowEndingKind, cloneModel.UsingRowEndingKind);
        //Assert.Equal(originalModel.ResourceUri, cloneModel.ResourceUri);
        //Assert.Equal(originalModel.ResourceLastWriteTime, cloneModel.ResourceLastWriteTime);
        //Assert.Equal(originalModel.FileExtension, cloneModel.FileExtension);
        //Assert.Equal(originalModel.DecorationMapper, cloneModel.DecorationMapper);
        //Assert.Equal(originalModel.CompilerService, cloneModel.CompilerService);
        //Assert.Equal(originalModel.TextEditorSaveFileHelper, cloneModel.TextEditorSaveFileHelper);
        //Assert.Equal(originalModel.EditBlockIndex, cloneModel.EditBlockIndex);
        //Assert.Equal(originalModel.IsDirty, cloneModel.IsDirty);
        //Assert.Equal(originalModel.MostCharactersOnASingleRowTuple, cloneModel.MostCharactersOnASingleRowTuple);
        //Assert.Equal(originalModel.RenderStateKey, cloneModel.RenderStateKey);
    }

    /// <summary>
	/// <see cref="TextEditorModel.ContentList"/>
	/// </summary>
	[Fact]
    public void ContentList_InterfaceImplementation()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.EditBlockList"/>
    /// </summary>
    [Fact]
    public void EditBlocksList_InterfaceImplementation()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.LineEndList "/>
    /// </summary>
    [Fact]
    public void RowEndingPositionsList_InterfaceImplementation()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.LineEndKindCountList"/>
    /// </summary>
    [Fact]
    public void RowEndingKindCountsList_InterfaceImplementation()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.PresentationModelList"/>
    /// </summary>
    [Fact]
    public void PresentationModelsList_InterfaceImplementation()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.TabKeyPositionList"/>
    /// </summary>
    [Fact]
    public void TabKeyPositionsList_InterfaceImplementation()
    {
        throw new NotImplementedException();
    }

    /// <summary>
	/// <see cref="TextEditorModel.ContentList"/>
	/// </summary>
	[Fact]
    public void ContentList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.EditBlockList"/>
    /// </summary>
    [Fact]
    public void EditBlocksList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.LineEndList"/>
    /// </summary>
    [Fact]
    public void RowEndingPositionsList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.LineEndKindCountList"/>
    /// </summary>
    [Fact]
    public void RowEndingKindCountsList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.PresentationModelList"/>
    /// </summary>
    [Fact]
    public void PresentationModelsList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.TabKeyPositionList"/>
    /// </summary>
    [Fact]
    public void TabKeyPositionsList()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.OnlyLineEndKind"/>
    /// </summary>
    [Fact]
    public void OnlyRowEndingKind()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.UsingLineEndKind"/>
    /// </summary>
    [Fact]
    public void UsingRowEndingKind()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.ResourceUri"/>
    /// </summary>
    [Fact]
    public void ResourceUri()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.ResourceLastWriteTime"/>
    /// </summary>
    [Fact]
    public void ResourceLastWriteTime()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.FileExtension"/>
    /// </summary>
    [Fact]
    public void FileExtension()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.DecorationMapper"/>
    /// </summary>
    [Fact]
    public void DecorationMapper()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.CompilerService"/>
    /// </summary>
    [Fact]
    public void CompilerService()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.TextEditorSaveFileHelper"/>
    /// </summary>
    [Fact]
    public void TextEditorSaveFileHelper()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.EditBlockIndex"/>
    /// </summary>
    [Fact]
    public void EditBlockIndex()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.MostCharactersOnASingleLineTuple"/>
    /// </summary>
    [Fact]
    public void MostCharactersOnASingleRowTuple()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.RenderStateKey"/>
    /// </summary>
    [Fact]
    public void RenderStateKey()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.TextEditorOptions"/>
    /// </summary>
    [Fact]
    public void TextEditorOptions()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.LineCount"/>
    /// </summary>
    [Fact]
    public void RowCount()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel.DocumentLength"/>
    /// </summary>
    [Fact]
    public void DocumentLength()
    {
        throw new NotImplementedException();
    }
}