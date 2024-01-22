namespace Luthetus.CompilerServices.Lang.CSharp.Tests.Basis.ParserCase.Internals;

public class ParseFunctionsTests
{
    /*
     # FunctionDefinition with block body
         public int MyMethod() { return 2; }
     # FunctionDefinition with expression body
         public int MyMethod() => 2;
     # FunctionInvocation
         MyMethod();
     # FunctionInvocation with variable assignment
         var x = MyMethod();
     # LocalFunction with block at start
         public int MyMethod() { int LocalFunction() { return 2; } return LocalFunction(); }
     # LocalFunction with block at (somewhere between start and end)
         public int MyMethod() { var y = 2; int LocalFunction() { return 2; } return LocalFunction(); }
     # LocalFunction with block at end
         public int MyMethod() { return LocalFunction(); int LocalFunction() { return 2; } }
     # LocalFunction with expression
         public int MyMethod() { int LocalFunction() => 2; return LocalFunction(); }
     # Delegate
         public delegate Task TextEditorEdit(ITextEditorEditContext editContext);
     # Func
         Func<char, char> toUpperFunc = value => char.ToUpper(value);
     # Action
         Action<string> logConsoleAction = value => Console.WriteLine(value);
     # MethodGroup(?) Passing of a method identifier to match the signature of an Action or Func?
         public void SomeMethod(Action<bool> action) { }
             public void BoolMethod(bool someBool) { }
             SomeMethod(BoolMethod);
     # Lambda expression
         public List<int> SearchIntegers(Func<int, bool> predicate);
             var result = SearchIntegers(integer => integer > 0);
     # Lambda code block
         public List<int> SearchIntegers(Func<int, bool> predicate);
             var result = SearchIntegers(integer =>
                 {
                     return integer > 0;
                 });
     */

    [Fact]
    public void FunctionDefinition_WITH_BlockBody()
    {
        var sourceText = "public int MyMethod() { return 2; }";
    }

    [Fact]
    public void FunctionDefinition_WITH_ExpressionBody()
    {
        var sourceText = "public int MyMethod() => 2;";
    }

    [Fact]
    public void FunctionInvocation()
    {
        var sourceText = "MyMethod();";
    }

    [Fact]
    public void FunctionInvocation_WITH_VariableAssignment()
    {
        var sourceText = "var x = MyMethod();";
    }

    [Fact]
    public void LocalFunction_WITH_Block_AT_Start()
    {
        var sourceText = "public int MyMethod() { int LocalFunction() { return 2; } return LocalFunction(); }";
    }

    [Fact]
    public void LocalFunction_WITH_Block_AT_SomewhereBetweenStartAndEnd()
    {
        var sourceText = "public int MyMethod() { var y = 2; int LocalFunction() { return 2; } return LocalFunction(); }";
    }

    [Fact]
    public void LocalFunction_WITH_Block_AT_End()
    {
        var sourceText = "public int MyMethod() { return LocalFunction(); int LocalFunction() { return 2; } }";
    }
    
    [Fact]
    public void LocalFunction_WITH_Expression_AT_Start()
    {
        var sourceText = "public int MyMethod() { int LocalFunction() => 2; return LocalFunction(); }";
    }

    [Fact]
    public void LocalFunction_WITH_Expression_AT_SomewhereBetweenStartAndEnd()
    {
        var sourceText = "public int MyMethod() { var y = 2; int LocalFunction() => 2; return LocalFunction(); }";
    }

    [Fact]
    public void LocalFunction_WITH_Expression_AT_End()
    {
        var sourceText = "public int MyMethod() { return LocalFunction(); int LocalFunction() => 2; }";
    }

    [Fact]
    public void Delegate()
    {
        var sourceText = "public delegate Task TextEditorEdit(ITextEditorEditContext editContext);";
    }

    [Fact]
    public void Func()
    {
        var sourceText = "Func<char, char> toUpperFunc = value => char.ToUpper(value);";
    }

    [Fact]
    public void Action()
    {
        var sourceText = "Action<string> logConsoleAction = value => Console.WriteLine(value);";
    }

    /// <summary>
    /// Passing of a method identifier to match the signature of an Action or Func?
    /// </summary>
    [Fact]
    public void MethodGroup()
    {
        var sourceText = @"public void SomeMethod(Action<bool> action) { }
             public void BoolMethod(bool someBool) { };
             SomeMethod(BoolMethod);";
    }

    public void LambdaExpression()
    {
        var sourceText = @"public List<int> SearchIntegers(Func<int, bool> predicate);
             var result = SearchIntegers(integer => integer > 0);";
    }

    public void LambdaCodeBlock()
    {
        var sourceText = @"public List<int> SearchIntegers(Func<int, bool> predicate);
             var result = SearchIntegers(integer =>
                 {
                     return integer > 0;
                 });";
    }
}
