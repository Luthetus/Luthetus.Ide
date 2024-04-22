using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models;

/// <summary>
/// <see cref="TextEditorModel"/>
/// </summary>
public class TextEditorModelTests
{
    /// <summary>
    /// <see cref="TextEditorModel(ResourceUri, DateTime, string, string, IDecorationMapper?, ILuthCompilerService?, int)"/>
    /// </summary>
    [Fact]
    public void Constructor_New()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorModel(ImmutableList{char}, ImmutableList{byte}, int, ImmutableList{TextEditorPartition}, ImmutableList{EditBlock}, ImmutableList{LineEnd}, ImmutableList{ValueTuple{LineEndKind, int}}, ImmutableList{TextEditorPresentationModel}, ImmutableList{int}, LineEndKind?, LineEndKind, ResourceUri, DateTime, string, IDecorationMapper, ILuthCompilerService, SaveFileHelper, int, bool, ValueTuple{int, int}, Key{RenderState})"/>
    /// </summary>
    [Fact]
    public void Constructor_Clone()
    {
        throw new NotImplementedException();
    }

    /// <summary>
	/// <see cref="TextEditorModel.ContentList"/>
    /// <see cref="TextEditorModel.EditBlockList"/>
    /// <see cref="TextEditorModel.LineEndList "/>
    /// <see cref="TextEditorModel.LineEndKindCountList"/>
    /// <see cref="TextEditorModel.PresentationModelList"/>
    /// <see cref="TextEditorModel.TabKeyPositionList"/>
	/// <see cref="TextEditorModel.ContentList"/>
    /// <see cref="TextEditorModel.EditBlockList"/>
    /// <see cref="TextEditorModel.LineEndList"/>
    /// <see cref="TextEditorModel.LineEndKindCountList"/>
    /// <see cref="TextEditorModel.PresentationModelList"/>
    /// <see cref="TextEditorModel.TabKeyPositionList"/>
    /// <see cref="TextEditorModel.OnlyLineEndKind"/>
    /// <see cref="TextEditorModel.LineEndKindPreference"/>
    /// <see cref="TextEditorModel.ResourceUri"/>
    /// <see cref="TextEditorModel.ResourceLastWriteTime"/>
    /// <see cref="TextEditorModel.FileExtension"/>
    /// <see cref="TextEditorModel.DecorationMapper"/>
    /// <see cref="TextEditorModel.CompilerService"/>
    /// <see cref="TextEditorModel.TextEditorSaveFileHelper"/>
    /// <see cref="TextEditorModel.EditBlockIndex"/>
    /// <see cref="TextEditorModel.MostCharactersOnASingleLineTuple"/>
    /// <see cref="TextEditorModel.RenderStateKey"/>
    /// <see cref="TextEditorModel.TextEditorOptions"/>
    /// <see cref="TextEditorModel.LineCount"/>
    /// <see cref="TextEditorModel.DocumentLength"/>
	/// </summary>
	[Fact]
    public void Properties()
    {
        throw new NotImplementedException();
    }
}