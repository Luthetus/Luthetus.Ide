/*
Goal: "rewrite" the C# Parser (2025-01-13)
==========================================

Issues
------------------------
- CSharpCodeBlockBuilder
- ICodeBlockOwner
- Scope parsing

Examples
--------

```csharp
// Example Description:
// --------------------
// ConstructorDefinitionNode invokes some other constructor via 'base' or 'this'.
//
// Idea A:
// -------
// # Identify ---------------------------------------------------------->
// When parsing a constructor definition,
// immediately open the constructor's scope after identifying the syntax
// to be a constructor definition.
//
//
// # Function arguments ------------------------------------------------>
// When the function arguments are parsed, the variable declaration node
// would then immediately be added to the constructor definitions scope.
//
//
// # Where clause ------------------------------------------------->
// If there is a 'where clause' between the 'CloseParenthesisToken'
// and the 'OpenBraceToken' that denotes the start of the body,
// this 'where clause' will have access to any definitions that
// occurred due to the constructor's signature being parsed
// since the constructor's scope was immediately opened.
//
//
// # EqualsCloseAngleBracketToken -------------------------------->
// If an 'EqualsCloseAngleBracketToken' is encountered
// instead of an 'OpenBraceToken', then the body is an expression.
//
//
// # OpenBraceToken --------------------->
// If an 'OpenBraceToken' is encountered
// then the body is 1 or more statements.
//
//
// # Deferred Parsing -------------------------------------->
// The 'YourClass' TypeDefinitionNode has
// the VariableDeclarationNode for the 'FirstName' property
// textually positioned after the ConstructorDefinitionNode.
// |
// Yet, within the body of the ConstructorDefinitionNode
// the 'FirstName' property is being referenced.
// |
// So, how does the 'FirstName' property get in scope
// if the file is parsed top down.
// |
// The decision was to use 'deferred parsing'.
// |
// Every 'ICodeBlockOwner' has a 'ScopeDirectionKind'.
// Examples of 'ICodeBlockOwner' are:
// TypeDefinitionNode, ConstructorDefinitionNode, ForStatementNode...
// If the current ICodeBlockOwner's 'ScopeDirectionKind' is the enum member 'Both'
// then any child scope should be 'deferred for parsing'.
// |
// When a scope is 'deferred for parsing', the TokenIndex that denotes the start of the scope,
// and the end of the scope are remembered, and this information is added to a 'Queue<CSharpDeferredChildScope>'.
// |
// Once the previously mentioned 'ICodeBlockOwner' with 'ScopeDirectionKind' of 'Both'
// starts to have the token that denotes the end of its scope parsed.
// Then 1 by 1 the 'Queue<CSharpDeferredChildScope>' is dequeued
// and the main loop is returned to, with a flag set to permit
// parsing of the next child scope even though the encompassing 'ICodeBlockOwner' has 'ScopeDirectionKind' of 'Both'.
// |
// There are more details to 'the main loop is returned to'.
// The 'TokenWalker' is told to set its 'Index' to the
// 'Index' of the token that represents the start of the child scope.
// |
// Furthermore, the 'TokenWalker' will internally check for when
// the 'Index' of the token that represents the end of the child scope is consumed.
// |
// Upon consumption of the token that represents the end of the child scope,
// the 'TokenWalker' will restore the 'Index' that represents the end
// of the encompassing 'ICodeBlockOwner' which has 'ScopeDirectionKind' of 'Both'.
// |
// The main loop will then once again parse the token that represents the end of the scope of the
// 'ICodeBlockOwner' which has 'ScopeDirectionKind' of 'Both'.
// The previously described steps are repeated until 'Queue<CSharpDeferredChildScope>'
// is empty.
// |
// Upon 'Queue<CSharpDeferredChildScope>' being empty,
// then the token that represents the end of the scope of the 'ICodeBlockOwner' which has 'ScopeDirectionKind' of 'Both'
// will be truly parsed.
// |
// It is important to note, during this 'deferred parsing' process.
// Any child ICodeBlockOwner is having its scope 'deferred for parsing' yes.
// But, the signature of the 'ICodeBlockOwner' is being parsed in its entirety.
// |
// Thus, any definitions or declarations that come textually after
// a textual reference to it, these will bind properly.
//
//
// # Opening a scope (problem description) -------------------------------------------------------------------->
// If an ICodeBlockOwner can be "expression bodied and deliminated by a StatementDelimiterToken" or
// "deliminated by OpenBraceToken and CloseBraceToken",
// then what does the path look like from opening a scope to then encountering either a StatementDelimiterToken
// or an 'OpenBraceToken'.
//
//
// # Opening a scope (initial step) -------------------------------------------------------->
// Anytime an 'ICodeBlockOwner' is encountered, it should have its scope immediately opened,
// and the 'CSharpParserModel' should have its 'CurrentCodeBlockBuilder' set to
// a newly constructed instance of 'CSharpCodeBlockBuilder' that will
// serve to be the code block of the newly encountered 'ICodeBlockOwner'.
//
//
// # Opening a scope (secondary syntax) ------------------------------------------------>
// "Secondary syntax" will be used when refering to:
//     a TypeDefinitionNode inheriting another class,
//     a ConstructorDefinitionNode invoking a separate constructor via 'base' or 'this',
//     a FunctionDefinitionNode having a 'where' clause,
//     and etc...
// |
// These "secondary syntax" tend to textually be positioned after where
// the scope was opened, and before any token that belongs to either the
// "expression body" or "OpenBraceToken for statement body" is encountered.
// |
// This is not an issue, it should "just work".
// 
//
// # Opening a scope ("expression body" or "OpenBraceToken for statement body") -------------------------------------------->
// When opening a scope, it should be presumed that every ICodeBlockOwner
// is an "expression body"...
// |
// ...unless the 'TokenWalker.Current.SyntaxKind' is 'SyntaxKind.OpenBraceToken'.
// |
// This might sound problematic, because "secondary syntax" might
// be read when checking the 'TokenWalker.Current.SyntaxKind'.
// But, all the presumption does, is set a bool on the 'ICodeBlockOwner'
// that is 'StatementDelimiterCanCloseScope' to 'true'.
// |
// This leaves open the ability to at a later point encounter the 'OpenBraceToken',
// and then change the 'ICodeBlockOwner' to be "brace deliminated statements"
// instead of an "expression body".
// In order to make this change, set 'StatementDelimiterCanCloseScope' to 'false'.
// |
// Note: if ever a "secondary syntax" contained an 'OpenBraceToken' or a 'StatementDelimiterToken',
// then this "simple" solution would need be made more "complex".
// Perhaps if this does come up in the future, that the 'OpenBraceToken' or 'StatementDelimiterToken'
// would be "predictable" and therefore "consumable" so that it does not come up during the "main loop" and cause confusion.
// |
// Anytime "secondary syntax" is involved, I wonder if the 'EqualsCloseAngleBracketToken' would always be
// a marker for when the "expression body" actually begins?
// |
// The first implementation will ignore the 'EqualsCloseAngleBracketToken',
// because for static analysis of code it might not be "too much an issue" / "will just work".
// 
//
// # Opening a scope (NO deferred parsing) -------------------------------->
// If the current 'CodeBlockBuilder' is a 'FunctionDefinitionNode',
// and we encounter an 'ArbitraryCodeBlockNode' (i.e.: "{...}"),
// what does it look like to open a scope for the 'ArbitraryCodeBlockNode'?
//
// 
// # Opening a scope (FunctionArgumentsListingNode) ------------------------------------->
// Further deferred parsing examples will presume that this
// FunctionArgumentsListingNode step was completed successfully.
// |
// The scope is opened once the syntax is fully disambiguated.
// For a 'ConstructorDefinitionNode' full disambiguation
// occurs after encountering the 'OpenParenthesisToken' that
// denotes the start of the 'FunctionArgumentsListingNode'.
// |
// The 'FunctionArgumentsListingNode' has specific syntax,
// rather then following "statement syntax".
// |
// Therefore, the "statement while loop" cannot parse
// these tokens correctly.
// |
// The method 'ParseFunctions.HandleFunctionArguments(...)'
// will parse the tokens properly however, and return the 'FunctionArgumentsListingNode'.
// While this method handles the function arguments, the current scope
// is the ConstructorDefinitionNode's. So, the scope of the function
// arguments are bound to the correct scope.
// |
// After the method returns the 'TokenWalker.Current' will be the token
// that comes immediately after the 'CloseParenthesisToken'.
//
//
// # Opening a scope; side note: "scope delimination" ---------------------------------------------->
// Scope will always be deliminated by two, character position indices.
// |
// The first character position index is the 'StartInclusiveIndex' of the scope.
// The second is the 'EndExclusiveIndex' of the scope.
// |
// Event if the "explicit scope syntax" consists of a single token,
// one of the character position indices would then be "implicit".
// |
// In the case of an "expression bodied" 'for loop',
// the "implicit" 'StartInclusiveIndex' comes immediately
// after the "for keyword" itself.
// |
// The for loop's 'EndExclusiveIndex' is "explicit scope syntax".
// This comes in the form of either a 'StatementDelimiterToken' or
// a 'CloseBraceToken'. Both of these are only valid for the
// 'EndExclusiveIndex' after the for loop's signature "for (int i = 0; i < 5; i++)".
// |
// So, what properties are needed on the ICodeBlockOwner if
// one wanted to track the 'StartInclusiveIndex' and the 'EndExclusiveIndex'
// of the scope?
// |
// The needed properties are:
//     'public int StartInclusiveScopeIndex { get; set; }'
//     'public int EndExclusiveScopeIndex { get; set; }'
// |
// As for the naming of these properties,
// it would depend on whether other 'StartInclusive...Index'
// and 'EndExclusive...Index' variables were in scope or not.
// |
// If only the 'Scope' indices are being referred to,
// then it is more valuable to have the descriptors at the start,
// the noun in the middle, and the purpose at the end.
// |
// The reverse of this is good too 'IndexScopeInclusiveStart'.
// |
// If there were other variables in scope that served a similar purpose
// but for a different noun, then it might be
// better to start or end with text that would quickly disambiguate
// the noun.
// |
// Given a scope, the most disambiguating parts of a variable name should
// avoid being put in the middle of the variable name.
// |
// In the case of a "scope deliminated by brace tokens".
// If one were to render on the UI highlighted text spans
// that showcased where the scope started and ended,
// these actual values might not be desired.
// |
// Because perhaps it looks more visually pleasing to
// highlight the OpenBraceToken and CloseBraceToken,
// if they deliminate the code block, than
// to highlight the start and end of the scope.
// |
// But, this still would only require the previously mentioned 2 properties
// on the 'Scope' type.
// |
// When rendering the highlight, every Scope has a reference to its
// corresponding 'ICodeBlockOwner'.
// |
// Every 'ICodeBlockOwner' then goes on to have properties for the:
//     OpenBraceToken
//     CloseBraceToken
//     StatementDelimiterToken
// |
// If both 'OpenBraceToken.ConstructorWasInvoked' and 'CloseBraceToken.ConstructorWasInvoked'
// for the 'ICodeBlockOwner', then you know the code block is deliminated by a pair of brace tokens.
// You can then choose to their respective text spans for the UI highlighting instead
// of the indices that were parsed for the scope.
// |
// With 'ICodeBlockOwner' being defined in the TextEditor C# project,
// having specific tokens such as 'OpenBraceToken', is not ideal.
// With Python for example, code blocks would presumably
// be deliminated by indentation. So more ideally:
//     TextEditorTextSpan? OpenCodeBlockTextSpan { get; set; }
//     TextEditorTextSpan? CloseCodeBlockTextSpan { get; set; }
//     bool IsImplicitOpenCodeBlockTextSpan { get; set; }
// |
// As well, there is no need for the 'StatementDelimiterToken' property.
// 'bool IsImplicitOpenCodeBlockTextSpan' would be better.
// Then, if 'IsImplicitOpenCodeBlockTextSpan' is 'true',
// 'OpenCodeBlockTextSpan' must be left null and cannot be set.
// |
// And if a 'StatementDelimiterToken' terminates an "expression body",
// then the 'CloseCodeBlockTextSpan' would be the 'StatementDelimiterToken'(s) text span.
//
//
// # Opening a scope (EqualsCloseAngleBracketToken) ----------------------------------------------------->
// If the ICodeBlockOwner uses an 'EqualsCloseAngleBracketToken' to indicate the start
// of an "expression body", it might be more visually pleasing to
// highlight the 'EqualsCloseAngleBracketToken' on the UI,
// and the 'StatementDelimiterToken' that will terminate the "expression body".
// Thus, the 'ICodeBlockOwner' can have the property 'IsImplicitOpenCodeBlockTextSpan'
// set to false, then the 'OpenCodeBlockTextSpan' set to the 'EqualsCloseAngleBracketToken'(s) text span.
// |
// As for static code analysis, the 'EqualsCloseAngleBracketToken' is probably used in some
// cases but not others (function expression body, versus, while loop body)
// due to optional "secondary syntax" being permissible between the start of the scope
// and the start of the code block.
// |
// Therefore, when encountering 'EqualsCloseAngleBracketToken',
// the 'CSharpParserModel.StatementBuilder' should be flushed
// so the "secondary syntax" does not interfere with the "expression body".
// 
//
// # Opening a scope ("an expression terminated by a 'StatementDelimiterToken'") ->
// This case is similar to the 'EqualsCloseAngleBracketToken' case, but
// the 'CSharpParserModel.StatementBuilder' likely won't need to be flushed.
//
//
// # Opening a scope (OpenBraceToken) --------------------------------------------------->
// When the 'OpenBraceToken' is encountered the 'CSharpParserModel.StatementBuilder' will
// automatically be flushed by the "statement while loop".
// |
// Would want to set the 'ICodeBlockOwner.IsImplicitOpenCodeBlockTextSpan' to false,
// then the 'OpenCodeBlockTextSpan' to the 'OpenBraceToken'(s) text span.
// |
// "secondary syntax" should "just work" since the "statement while loop"
// will flush the 'CSharpParserModel.StatementBuilder'.
//
//
// # Opening a scope (with deferred parsing) --------------------------------->
// If the current 'CodeBlockBuilder' is a 'TypeDefinitionNode',
// and we encounter a 'ConstructorDefinitionNode',
// what does it look like to open a scope for the 'ConstructorDefinitionNode'?
// 
// 
// # Opening a scope (deferred parsing + EqualsCloseAngleBracketToken) ----------------------------->
// It is presumed that a limited amount of ICodeBlockOwner(s) use the 'EqualsCloseAngleBracketToken'
// syntax for their "expression body".
// |
// Furthermore it is presumed that they use the 'EqualsCloseAngleBracketToken' in order
// to disambiguate "secondary syntax" from the "expression body".
// |
// So, what needs to be done is:
// determine all the ICodeBlockOwner(s) that use the 'EqualsCloseAngleBracketToken'
// for their "expression body".
// |
// For the example of a 'FunctionDefinitionNode',
// the 'EqualsCloseAngleBracketToken' is valid after the
// 'FunctionArgumentsListingNode' has been parsed.
// |
// So, anything that follows the 'FunctionArgumentsListingNode',
// up to but exclusive the 'EqualsCloseAngleBracketToken'
// is "secondary syntax".
// |
// In some cases there won't be any "secondary syntax"
// and that is fine.
// |
// But, what needs to be done is to
// probably try parsing an expression,
// with a delimiter to return to the statement code being 'EqualsCloseAngleBracketToken'.
// |
// This way the "expression while loop" will make whatever sense of the "secondary syntax" it can,
// then return to the statement code once it finds an un-matched 'EqualsCloseAngleBracketToken'.
// |
// As for the "deferred parsing" part.
// |
// Once the 'EqualsCloseAngleBracketToken' is found, then enqueue into the 'Queue<CSharpDeferredChildScope>'
// the token index of the 'EqualsCloseAngleBracketToken'.
// |
// Additionally, one has to "skip" ahead by iterating the token list until either 'EndOfFileToken',
// or 'StatementDelimiterToken' is found.
// |
// Since one has to enqueue not only the starting token index, but also the ending token index.
//
//
// # Opening a scope (deferred parsing + "an expression terminated by a 'StatementDelimiterToken'") ->
// It is presumed that the lack of the 'EqualsCloseAngleBracketToken' to denote the start
// of an "expression body" is because there is no "secondary syntax"
// that is allowed to be after the scope starts, and before the "expression body".
// |
// So, this step is similar to the 'EqualsCloseAngleBracketToken' case,
// but do not search the token list for a 'EqualsCloseAngleBracketToken'.
// |
// As for the "deferred parsing" part.
// |
// Once the "expression body" is permissible, then enqueue into the 'Queue<CSharpDeferredChildScope>'
// the current token index.
// |
// Additionally, one has to "skip" ahead by iterating the token list until either 'EndOfFileToken',
// or 'StatementDelimiterToken' is found.
// |
// Since one has to enqueue not only the starting token index, but also the ending token index.
//
//
// # Opening a scope (deferred parsing + OpenBraceToken) -------------------------------->
// Any "secondary syntax" that appears just prior to the 'OpenBraceToken' needs to parsed
// with the "expression while loop".
// |
// Once the 'OpenBraceToken' is found,
// then "skip" ahead by iterating the token list until either 'EndOfFileToken'
// or 'CloseBraceToken' is found.
// |
// At this point enqueue into 'Queue<CSharpDeferredChildScope>'
// the starting index, and ending index.

public class YourClass
{
	public YourClass(string firstName)
	{
		FirstName = firstName;
	}
	
	public string FirstName { get; set; }
}

public class MyClass : YourClass
{
	public MyClass(string firstName)
		: base(firstName)
	{
	}
}
```

*/

