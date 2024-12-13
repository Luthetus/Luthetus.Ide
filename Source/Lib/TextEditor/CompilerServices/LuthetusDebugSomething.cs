using System.Text;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

/// <summary>
/// Statically track the constructor invocations to make sense of possible optimizations.
/// (this file likely can be deleted if you see this in the future (2024-12-10))
/// </summary>
public static class LuthetusDebugSomething
{
	public static int CompilerService_ConstructorInvocationCount { get; set; }
	public static int Lexer_ConstructorInvocationCount { get; set; }
	public static int StringWalker_ConstructorInvocationCount { get; set; }
	public static int Parser_ConstructorInvocationCount { get; set; }
	public static int TokenWalker_ConstructorInvocationCount { get; set; }
	public static int ParserModel_ConstructorInvocationCount { get; set; }
	public static int Binder_ConstructorInvocationCount { get; set; }
	public static int BinderSession_ConstructorInvocationCount { get; set; }
	
	public static string CreateText()
	{
		var builder = new StringBuilder();
		
		builder.AppendLine();
		builder.AppendLine($"{nameof(CompilerService_ConstructorInvocationCount)}: {CompilerService_ConstructorInvocationCount:N0}");
		builder.AppendLine($"{nameof(Lexer_ConstructorInvocationCount)}:           {Lexer_ConstructorInvocationCount:N0}");
		builder.AppendLine($"{nameof(StringWalker_ConstructorInvocationCount)}:    {StringWalker_ConstructorInvocationCount:N0}");
		builder.AppendLine($"{nameof(Parser_ConstructorInvocationCount)}:          {Parser_ConstructorInvocationCount:N0}");
		builder.AppendLine($"{nameof(TokenWalker_ConstructorInvocationCount)}:     {TokenWalker_ConstructorInvocationCount:N0}");
		builder.AppendLine($"{nameof(ParserModel_ConstructorInvocationCount)}:     {ParserModel_ConstructorInvocationCount:N0}");
		builder.AppendLine($"{nameof(Binder_ConstructorInvocationCount)}:          {Binder_ConstructorInvocationCount:N0}");
		builder.AppendLine($"{nameof(BinderSession_ConstructorInvocationCount)}:   {BinderSession_ConstructorInvocationCount:N0}");
		builder.AppendLine();
		
		return builder.ToString();
	}
}