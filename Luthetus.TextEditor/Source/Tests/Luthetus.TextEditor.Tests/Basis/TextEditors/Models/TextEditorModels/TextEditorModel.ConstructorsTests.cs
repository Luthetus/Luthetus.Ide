using Xunit;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Keymaps.Models;

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
		var compilerService = new TextEditorDefaultCompilerService();
		var keymap = TextEditorKeymapFacts.DefaultKeymap;

        var model = new TextEditorModel(
            resourceUri,
            resourceLastWriteTime,
            fileExtension,
            content,
            decorationMapper,
            compilerService,
            keymap);

		Assert.Equal(resourceUri, model.ResourceUri);
		Assert.Equal(resourceLastWriteTime, model.ResourceLastWriteTime);
		Assert.Equal(fileExtension, model.FileExtension);
		Assert.Equal(content, model.GetAllText());
		Assert.Equal(decorationMapper, model.DecorationMapper);
		Assert.Equal(compilerService, model.CompilerService);
		Assert.Equal(keymap, model.TextEditorKeymap);
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
            new TextEditorDefaultCompilerService(),
            TextEditorKeymapFacts.DefaultKeymap);

		var cloneModel = new TextEditorModel(
			originalModel.ContentBag,
			originalModel.EditBlocksBag,
            originalModel.RowEndingPositionsBag,
            originalModel.RowEndingKindCountsBag,
            originalModel.PresentationModelsBag,
            originalModel.TabKeyPositionsBag,
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
			originalModel.RenderStateKey,
			originalModel.TextEditorKeymap,
			originalModel.TextEditorOptions);

        Assert.Equal(originalModel.ContentBag, cloneModel.ContentBag);
		Assert.Equal(originalModel.EditBlocksBag, cloneModel.EditBlocksBag);
        Assert.Equal(originalModel.RowEndingPositionsBag, cloneModel.RowEndingPositionsBag);
        Assert.Equal(originalModel.RowEndingKindCountsBag, cloneModel.RowEndingKindCountsBag);
        Assert.Equal(originalModel.PresentationModelsBag, cloneModel.PresentationModelsBag);
        Assert.Equal(originalModel.TabKeyPositionsBag, cloneModel.TabKeyPositionsBag);
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
		Assert.Equal(originalModel.TextEditorKeymap, cloneModel.TextEditorKeymap);
		Assert.Equal(originalModel.TextEditorOptions, cloneModel.TextEditorOptions);
	}
}