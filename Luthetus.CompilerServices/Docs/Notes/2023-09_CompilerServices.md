2023-09 | Luthetus.CompilerServices | Notes

---

## 2023-09-04

### Ideas
- Expressions need to be parsed.
    - Example: Add(int a, int b). The arguments are both ints. But one could invoke the method like: Add(9/3, 7);
    - Currently I just grab the "9/3" by reading until the comma which is no good.
- Control keywords need to be parsed
    - for (int i = 0; i < 5; i++){ /* Body */ }
    - Currently if one types a for loop its quite incorrectly parsed.
- Autocomplete using the Binder from ICompilerService
    - Example: If I type 'Console' then a '.' I should see the 'WriteLine' method in the autocomplete list.
    - (because 'WriteLine' is a method on the 'Console' class)

### Expressions need to be parsed (thoughts)
I want to correctly parse expressions. The issue is I don't know how, and this task intimidates me, so I procrastinate.

If I consider the simplest case for parsing expressions. I suppose it would be:

> Input: "3"

The lexer will tell me there is a NumericLiteralToken. Furthermore, that the next token is the EndOfFileToken.

So its just literally the number 3. I can evaluate that to be 3.

> Input: "3 + 2"

Lexer gives me:

- NumericLiteralToken
- PlusToken
- NumericLiteralToken

Writing out these thoughts I'm wondering to myself, "why am I so intimidated by this task".

It seems fine. I'm going to just open Visual Studio and see where things go.

## 2023-09-07

### ICompilerService 'Design' Idea
I will use the `C# CompilerService` as an example.

How does one parse a file when using the `CompilerService`?

- `Lexer`
- `Parser`

> How does the `Lexer` work?

`Lexer` takes in a `string`, and outputs an `IEnumerable`&lt;`ISyntaxToken`&gt;

The `Lexer` has one public method `public void Lex()`

The internals for this method follow the pattern of:

```csharp
List<ISyntaxToken> _syntaxTokens = new();

while (readingText)
{
    switch (currentCharacter)
    {
        /* Lowercase Letters */
        case 'a': /* ... */ case 'z':
        /* Uppercase Letters */
        case 'A': /* ... */ case 'Z':
        /* Underscore */
        case '_':
            LexIdentifierOrKeywordOrKeywordContextual();
            break;
        /* Numeric Digits */
        case '1': /* ... */ case '9':
            LexNumericLiteralToken();
            break;
        case '/':
            if (_stringWalker.PeekCharacter(1) == '/')
                LexCommentSingleLineToken();
            else if (_stringWalker.PeekCharacter(1) == '*')
                LexCommentMultiLineToken();
            else
                LexDivisionToken();

            break;
        case '+':
            if (_stringWalker.PeekCharacter(1) == '+')
                LexPlusPlusToken();
            else
                LexPlusToken();

            break;
        case '*':
            LexStarToken();
            break;
        // case '...'
        default:
            _ = _stringWalker.ReadCharacter();
            break;
    }
}

private void LexNumericLiteralToken()
{
    _syntaxTokens.Add(new NumericLiteralToken());
}
```

I don't want to change how the lexer is written, I like this way of writing it for now.

> How does the `Parser` work?

The parser is what I want to re-organize a bit.

The Parser has 3 situations
- The main while(...) loop that iterates over all the tokens
- A method invoked when one has a SyntaxToken, but no context.
- A method invoked when one has a SyntaxToken, AND context.

The main while(...) loop follows this pattern:

```csharp
while (true)
{
    var consumedToken = _tokenWalker.Consume();

    switch (consumedToken.SyntaxKind)
    {
        case SyntaxKind.NumericLiteralToken:
            ParseNumericLiteralToken((NumericLiteralToken)consumedToken);
            break;
        case SyntaxKind.StringLiteralToken:
            ParseStringLiteralToken((StringLiteralToken)consumedToken);
            break;
        case SyntaxKind.PlusToken:
            ParsePlusToken((PlusToken)consumedToken);
            break;
        // case SyntaxKind.etc...
        case SyntaxKind.EndOfFileToken:
            if (_nodeRecent is IExpressionNode)
            {
                _currentCodeBlockBuilder.IsExpression = true;
                _currentCodeBlockBuilder.Children.Add(_nodeRecent);
            }
            break;
        default:
            if (_tokenWalker.IsContextualKeywordSyntaxKind(consumedToken.SyntaxKind))
                ParseKeywordContextualToken((KeywordContextualToken)consumedToken);
            else if (_tokenWalker.IsKeywordSyntaxKind(consumedToken.SyntaxKind))
                ParseKeywordToken((KeywordToken)consumedToken);
            break;
    }

    if (consumedToken.SyntaxKind == SyntaxKind.EndOfFileToken)
        break;
}
```

I like the main while loop pattern that is currently in place.

But, note the method names all start with Parse...(...)

That is to say, in the case of `SyntaxKind.NumericLiteralToken` one invokes `ParseNumericLiteralToken(...)`

To me, the `Parse...(...)` methods are to only be invoked directly from the main while loop. Futhermore, the main while loop should only take control when there is no context.

By context I more or less mean to say, you are starting a new statement.

As one parses a statement, I believe they should invoke a series of `Handle...(...)` methods.

For an example of what I mean I'll use the '=' character or (`EqualsToken`)

If one the following two string inputs (separately)

> () => 3

> var x = 3

Well, each of those string inputs contains the '=' character or (`EqualsToken`). But, due to context, the meaning of the `EqualsToken` varies.

So, one might have `ParseEqualsToken(...)`; But, they also would have `HandleLambdaExpression` or `HandleVariableInitialization`

All the languages share a common set of Tokens, but the context determines the meaning of the Token.

What I'm trying to say is, I want to break the Parser up into 3 parts.

- Main while loop
- Parser
- Handler

"Main while loop" would iterate over the SyntaxTokens

"Parser" would be invoked from the main while loop. Here one determines the context

"Handler" then is invoked from the "Parser" and a sequence of "Handler" methods are invoked until the statement is finished. Then the main while loop takes back over.

---

```csharp
public class MyClass
{
    public void MyMethod()
    {
    }
}
```

var classDefinition = new ClassDefinition("MyClass");

classDefinition.Body.Add(new MethodDefinition("MyMethod"));

GlobalScope.ClassDefinitions.Add("MyClass")

---

I ran to the store

Lexer Output -> {
    SubjectToken, VerbToken, PrepositionToken, ArticleToken, NounToken
}

ParseSubjectToken()
    .ParseVerbToken()
    .ParsePrepositionToken()
    .ParseArticleToken()
    .ParseNounToken

When I start with parsing the `SubjectToken` I have no context about the sentence, because I'm just starting. I look at this like a statement in the C# parser. If I am just starting to parse a statement I want to invoke `Parse...(...)`

When I start parsing the `VerbToken` I'm doing it as a continuation. I have context on the sentence I'm working with. So I want to invoke `Handle...(...)`

--- 

Given:

```csharp
public class MyClass
{
    public void MyMethod()
    {
    }
}
```

I'm going to start off without any context. So, I invoke `ParseKeyword(...)`.

Well, this loop of the while loop didn't glean any context. All I see is the keyword `public`. I don't understand yet what's going on. So I make _nodeRecent = PublicKeyword and do another while loop iteration.

I invoke `ParseKeyword(...)` again. It does so on the keyword `class`. I now have context on what I'm reading. I'm reading a class definition. Therefore, forget what tokens come next, invoke the `HandleClassDefinition(...)` method and let them deal with it.

Eventually I return from `HandleClassDefinition(...)`
because I've finished reading the class definition, and once again I'm without context.

But then, the method inside the class definition: `MyMethod`.

When I invoke `HandleClassDefinition(...)` what is occurring?

I have a parent context: `ClassDefinition`.
Then the child context is unknown as of yet.

I read the `public` keyword of the method definition.
I still have no child context.

I read the `void`. Now I know I'm reading a function definition. So I have a parent context of a class definition and a child context of a method definition.
So I invoke `HandleMethodDefinition(...)`

---

Continuing on from the previous example. I need to consider 

Given:

```csharp
public void MyMethod()
{
}
```

because this is exactly what I'm confused about.

There seem to be `Parse` methods where one is contextless. Then `Handle` methods where one understands the context.

I say this because in this given source code, I read public, then void and see I'm reading a method definition, so `HandleMethodDefinition(...)`

Yet, while previously parsing a class definition I invoked `HandleMethodDefinition(...)`

So, I need these separate `Handle` methods because language syntax can appear in many locations.

---

I think there are OpenEndedApi which take tokens.

Then there are LanguageSpecificApi which take ???

The OpenEndedApi could be an abstract implementation (I'm not going to do this but it showcases the idea). Everyone has these methods. One for every token. And you continually invoke OpenEndedApi one after another until you determine the singular language specific syntax you are dealing with. Then you go on to invoke the corresponding LanguageSpecificApi.

---

Maybe one parses tokens, but at times determines a language specific syntax, so they handle the language specific syntax, then go back to parsing tokens until they understand the language specific syntax again.

---

- Should General methods accept as an argument the Token being parsed? Or should they use _tokenWalker.CurrentToken.
    - Fabrication of tokens is a massive pain if one doesn't have the method argument be the token being parsed.

- Should General methods return the parsed Node?
    - Yes?
    
- Language Specific methods accept as an argument the Token being parsed? Or should they use _tokenWalker.CurrentToken.
    - Fabrication of tokens is a massive pain if one doesn't have the method argument be the token being parsed.

- Should Language Specific methods return the parsed Node?
    - Yes?