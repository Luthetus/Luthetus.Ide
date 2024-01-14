using Xunit;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorModels;

/// <summary>
/// <see cref="TextEditorModel"/>
/// </summary>
public class TextEditorModelConstructorsTests
{
	/// <summary>
	/// <see cref="TextEditorModel(ResourceUri, DateTime, string, string, RazorLib.Decorations.Models.IDecorationMapper?, RazorLib.CompilerServices.ICompilerService?, Common.RazorLib.Keymaps.Models.Keymap?)"/>
	/// </summary>
	[Fact]
	public void Constructor_New()
	{
		var resourceUri = new ResourceUri("/unitTests.txt");
		var resourceLastWriteTime = DateTime.UtcNow;
		var fileExtension = ".txt";
		var content = "Hello World!";
		var decorationMapper = new TextEditorDecorationMapperDefault();
		var compilerService = new TextEditorCompilerServiceDefault();

        var model = new TextEditorModel(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            content,
            decorationMapper,
            compilerService);

		Assert.Equal(resourceUri, model.ResourceUri);
		Assert.Equal(resourceLastWriteTime, model.ResourceLastWriteTime);
		Assert.Equal(fileExtension, model.FileExtension);
		Assert.Equal(content, model.GetAllText());
		Assert.Equal(decorationMapper, model.DecorationMapper);
		Assert.Equal(compilerService, model.CompilerService);
	}

	/// <summary>
	/// <see cref="TextEditorModel(System.Collections.Immutable.ImmutableList{RazorLib.Characters.Models.RichCharacter}, System.Collections.Immutable.ImmutableList{RazorLib.Edits.Models.EditBlock}, System.Collections.Immutable.ImmutableList{ValueTuple{int, RazorLib.Rows.Models.RowEndingKind}}, System.Collections.Immutable.ImmutableList{ValueTuple{RazorLib.Rows.Models.RowEndingKind, int}}, System.Collections.Immutable.ImmutableList{RazorLib.Decorations.Models.TextEditorPresentationModel}, System.Collections.Immutable.ImmutableList{int}, RazorLib.Rows.Models.RowEndingKind?, RazorLib.Rows.Models.RowEndingKind, RazorLib.Lexes.Models.ResourceUri, DateTime, string, RazorLib.Decorations.Models.IDecorationMapper, RazorLib.CompilerServices.ICompilerService, RazorLib.TextEditors.Models.TextEditorSaveFileHelper, int, ValueTuple{int, int}, Common.RazorLib.Keys.Models.Key{Common.RazorLib.RenderStates.Models.RenderState}, Common.RazorLib.Keymaps.Models.Keymap, RazorLib.Options.Models.TextEditorOptions?)"/>
	/// </summary>
	[Fact]
	public void Constructor_Clone()
	{
        var originalModel = new TextEditorModel(
            new ResourceUri("/unitTests.txt"),
            DateTime.UtcNow,
            ".txt",
            "Hello World!",
            new TextEditorDecorationMapperDefault(),
            new TextEditorCompilerServiceDefault());

		var cloneModel = new TextEditorModel(
			originalModel.ContentList,
			originalModel.EditBlocksList,
            originalModel.RowEndingPositionsList,
            originalModel.RowEndingKindCountsList,
            originalModel.PresentationModelsList,
            originalModel.TabKeyPositionsList,
            originalModel.OnlyRowEndingKind,
			originalModel.UsingRowEndingKind,
			originalModel.ResourceUri,
			originalModel.ResourceLastWriteTime,
			originalModel.FileExtension,
			originalModel.DecorationMapper,
			originalModel.CompilerService,
			originalModel.TextEditorSaveFileHelper,
			originalModel.EditBlockIndex,
			originalModel.MostCharactersOnASingleRowTuple,
			originalModel.RenderStateKey);

        Assert.Equal(originalModel.ContentList, cloneModel.ContentList);
		Assert.Equal(originalModel.EditBlocksList, cloneModel.EditBlocksList);
        Assert.Equal(originalModel.RowEndingPositionsList, cloneModel.RowEndingPositionsList);
        Assert.Equal(originalModel.RowEndingKindCountsList, cloneModel.RowEndingKindCountsList);
        Assert.Equal(originalModel.PresentationModelsList, cloneModel.PresentationModelsList);
        Assert.Equal(originalModel.TabKeyPositionsList, cloneModel.TabKeyPositionsList);
        Assert.Equal(originalModel.OnlyRowEndingKind, cloneModel.OnlyRowEndingKind);
		Assert.Equal(originalModel.UsingRowEndingKind, cloneModel.UsingRowEndingKind);
		Assert.Equal(originalModel.ResourceUri, cloneModel.ResourceUri);
		Assert.Equal(originalModel.ResourceLastWriteTime, cloneModel.ResourceLastWriteTime);
		Assert.Equal(originalModel.FileExtension, cloneModel.FileExtension);
		Assert.Equal(originalModel.DecorationMapper, cloneModel.DecorationMapper);
		Assert.Equal(originalModel.CompilerService, cloneModel.CompilerService);
		Assert.Equal(originalModel.TextEditorSaveFileHelper, cloneModel.TextEditorSaveFileHelper);
		Assert.Equal(originalModel.EditBlockIndex, cloneModel.EditBlockIndex);
		Assert.Equal(originalModel.MostCharactersOnASingleRowTuple, cloneModel.MostCharactersOnASingleRowTuple);
		Assert.Equal(originalModel.RenderStateKey, cloneModel.RenderStateKey);
	}
}