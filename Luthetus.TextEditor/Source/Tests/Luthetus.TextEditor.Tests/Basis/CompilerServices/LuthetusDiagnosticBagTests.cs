using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

/// <summary>
/// <see cref="LuthetusDiagnosticBag"/>
/// </summary>
public class LuthetusDiagnosticBagTests
{
	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.GetEnumerator()"/>
	/// </summary>
	[Fact]
	public void GetEnumerator_ClassDefinedMethod()
	{
		//public IEnumerator<TextEditorDiagnostic> GetEnumerator()
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.GetEnumerator()"/>
	/// </summary>
	[Fact]
	public void GetEnumerator_ExplicitInterfaceImplementation()
	{
		//IEnumerator IEnumerable.GetEnumerator()
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportEndOfFileUnexpected(RazorLib.Lexes.Models.TextEditorTextSpan)"/>
	/// </summary>
	[Fact]
	public void ReportEndOfFileUnexpected()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportUnexpectedToken(RazorLib.Lexes.Models.TextEditorTextSpan, string, string)"/>
	/// </summary>
	[Fact]
	public void ReportUnexpectedToken_A()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportUnexpectedToken(RazorLib.Lexes.Models.TextEditorTextSpan, string)"/>
	/// </summary>
	[Fact]
	public void ReportUnexpectedToken_B()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportUndefinedClass(RazorLib.Lexes.Models.TextEditorTextSpan, string)"/>
	/// </summary>
	[Fact]
	public void ReportUndefinedClass()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportUndefinedTypeOrNamespace(RazorLib.Lexes.Models.TextEditorTextSpan, string)"/>
	/// </summary>
	[Fact]
	public void ReportUndefinedTypeOrNamespace()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportAlreadyDefinedType(RazorLib.Lexes.Models.TextEditorTextSpan, string)"/>
	/// </summary>
	[Fact]
	public void ReportAlreadyDefinedType()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportUndefinedVariable(RazorLib.Lexes.Models.TextEditorTextSpan, string)"/>
	/// </summary>
	[Fact]
	public void ReportUndefinedVariable()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportAlreadyDefinedVariable(RazorLib.Lexes.Models.TextEditorTextSpan, string)"/>
	/// </summary>
	[Fact]
	public void ReportAlreadyDefinedVariable()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportAlreadyDefinedProperty(RazorLib.Lexes.Models.TextEditorTextSpan, string)"/>
	/// </summary>
	[Fact]
	public void ReportAlreadyDefinedProperty()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportUndefinedFunction(RazorLib.Lexes.Models.TextEditorTextSpan, string)"/>
	/// </summary>
	[Fact]
	public void ReportUndefinedFunction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportAlreadyDefinedFunction(RazorLib.Lexes.Models.TextEditorTextSpan, string)"/>
	/// </summary>
	[Fact]
	public void ReportAlreadyDefinedFunction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportBadFunctionOptionalArgumentDueToMismatchInType(RazorLib.Lexes.Models.TextEditorTextSpan, string, string, string)"/>
	/// </summary>
	[Fact]
	public void ReportBadFunctionOptionalArgumentDueToMismatchInType()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportReturnStatementsAreStillBeingImplemented(RazorLib.Lexes.Models.TextEditorTextSpan)"/>
	/// </summary>
	[Fact]
	public void ReportReturnStatementsAreStillBeingImplemented()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportTagNameMissing(RazorLib.Lexes.Models.TextEditorTextSpan)"/>
	/// </summary>
	[Fact]
	public void ReportTagNameMissing()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportOpenTagWithUnMatchedCloseTag(string, string, RazorLib.Lexes.Models.TextEditorTextSpan)"/>
	/// </summary>
	[Fact]
	public void ReportOpenTagWithUnMatchedCloseTag()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportRazorExplicitExpressionPredicateWasExpected(string, string, RazorLib.Lexes.Models.TextEditorTextSpan)"/>
	/// </summary>
	[Fact]
	public void ReportRazorExplicitExpressionPredicateWasExpected()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportRazorCodeBlockWasExpectedToFollowRazorKeyword(string, string, RazorLib.Lexes.Models.TextEditorTextSpan)"/>
	/// </summary>
	[Fact]
	public void ReportRazorCodeBlockWasExpectedToFollowRazorKeyword()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportRazorWhitespaceImmediatelyFollowingTransitionCharacterIsUnexpected(RazorLib.Lexes.Models.TextEditorTextSpan)"/>
	/// </summary>
	[Fact]
	public void ReportRazorWhitespaceImmediatelyFollowingTransitionCharacterIsUnexpected()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportConstructorsNeedToBeWithinTypeDefinition(RazorLib.Lexes.Models.TextEditorTextSpan)"/>
	/// </summary>
	[Fact]
	public void ReportConstructorsNeedToBeWithinTypeDefinition()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ReportTodoException(RazorLib.Lexes.Models.TextEditorTextSpan, string)"/>
	/// </summary>
	[Fact]
	public void ReportTodoException()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="LuthetusDiagnosticBag.ClearByResourceUri(RazorLib.Lexes.Models.ResourceUri)"/>
	/// </summary>
	[Fact]
	public void ClearByResourceUri()
	{
		throw new NotImplementedException();
	}
}