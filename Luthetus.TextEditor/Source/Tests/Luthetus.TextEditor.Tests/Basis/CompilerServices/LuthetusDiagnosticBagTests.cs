using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices;

public class LuthetusDiagnosticBagTests
{
	[Fact]
	public void GetEnumerator_A()
	{
		//public IEnumerator<TextEditorDiagnostic> GetEnumerator()
		throw new NotImplementedException();
	}

	[Fact]
	public void GetEnumerator_B()
	{
		//IEnumerator IEnumerable.GetEnumerator()
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportEndOfFileUnexpected()
	{
		//public void ReportEndOfFileUnexpected(TextEditorTextSpan textSpan)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportUnexpectedToken_A()
	{
		//public void ReportUnexpectedToken(
	 //       TextEditorTextSpan textSpan,
	 //       string unexpectedToken,
	 //       string expectedToken)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportUnexpectedToken_B()
	{
		//public void ReportUnexpectedToken(
		//	TextEditorTextSpan textSpan,
		//	string unexpectedToken)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportUndefinedClass()
	{
		//public void ReportUndefinedClass(
	 //       TextEditorTextSpan textSpan,
	 //       string undefinedClassIdentifier)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportUndefinedTypeOrNamespace()
	{
		//public void ReportUndefinedTypeOrNamespace(
	 //       TextEditorTextSpan textSpan,
	 //       string undefinedTypeOrNamespaceIdentifier)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportAlreadyDefinedType()
	{
		//public void ReportAlreadyDefinedType(
	 //       TextEditorTextSpan textSpan,
	 //       string alreadyDefinedTypeIdentifier)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportUndefinedVariable()
	{
		//public void ReportUndefinedVariable(
	 //       TextEditorTextSpan textSpan,
	 //       string undefinedVariableIdentifier)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportAlreadyDefinedVariable()
	{
		//public void ReportAlreadyDefinedVariable(
	 //       TextEditorTextSpan textSpan,
	 //       string alreadyDefinedVariableIdentifier)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportAlreadyDefinedProperty()
	{
		//public void ReportAlreadyDefinedProperty(
	 //       TextEditorTextSpan textSpan,
	 //       string alreadyDefinedPropertyIdentifier)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportUndefinedFunction()
	{
		//public void ReportUndefinedFunction(
	 //       TextEditorTextSpan textSpan,
	 //       string undefinedFunctionIdentifier)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportAlreadyDefinedFunction()
	{
		//public void ReportAlreadyDefinedFunction(
	//       TextEditorTextSpan textSpan,
	//       string alreadyDefinedFunctionIdentifier)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportBadFunctionOptionalArgumentDueToMismatchInType()
	{
		//public void ReportBadFunctionOptionalArgumentDueToMismatchInType(
	 //       TextEditorTextSpan optionalArgumentTextSpan,
	 //       string optionalArgumentVariableIdentifier,
	 //       string typeExpectedIdentifierString,
	 //       string typeActualIdentifierString)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportReturnStatementsAreStillBeingImplemented()
	{
		//public void ReportReturnStatementsAreStillBeingImplemented(TextEditorTextSpan textSpan)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportTagNameMissing()
	{
		//public void ReportTagNameMissing(TextEditorTextSpan textSpan)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportOpenTagWithUnMatchedCloseTag()
	{
		//public void ReportOpenTagWithUnMatchedCloseTag(
	 //       string openTagName,
	 //       string closeTagName,
	 //       TextEditorTextSpan textSpan)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportRazorExplicitExpressionPredicateWasExpected()
	{
		//public void ReportRazorExplicitExpressionPredicateWasExpected(
	 //       string transitionSubstring,
	 //       string keywordText,
	 //       TextEditorTextSpan textSpan)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportRazorCodeBlockWasExpectedToFollowRazorKeyword()
	{
		//public void ReportRazorCodeBlockWasExpectedToFollowRazorKeyword(
	 //       string transitionSubstring,
	 //       string keywordText,
	 //       TextEditorTextSpan textSpan)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportRazorWhitespaceImmediatelyFollowingTransitionCharacterIsUnexpected()
	{
		//public void ReportRazorWhitespaceImmediatelyFollowingTransitionCharacterIsUnexpected(
	 //       TextEditorTextSpan textSpan)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportConstructorsNeedToBeWithinTypeDefinition()
	{
		//public void ReportConstructorsNeedToBeWithinTypeDefinition(TextEditorTextSpan textSpan)
		throw new NotImplementedException();
	}

	[Fact]
	public void ReportTodoException()
	{
		//public void ReportTodoException(TextEditorTextSpan textSpan, string message)
		throw new NotImplementedException();
	}

	[Fact]
	public void Report()
	{
		//private void Report(
	//       TextEditorDiagnosticLevel diagnosticLevel,
	//       string message,
	//       TextEditorTextSpan textSpan,
	//       Guid diagnosticId)
		throw new NotImplementedException();
	}
    
	[Fact]
	public void ClearByResourceUri()
	{
		//public void ClearByResourceUri(ResourceUri resourceUri)
		throw new NotImplementedException();
	}
}