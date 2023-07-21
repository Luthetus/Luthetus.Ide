namespace Luthetus.Ide.Tests.Basics.Terminal;

public class CliWrapTests
{
    [Fact]
    public async Task DOTNET_NEW_SLN()
    {
        var standardErrorBuilder = new StringBuilder();
        var standardInputBuilder = new StringBuilder();

        await Cli.Wrap("dotnet")
            .WithArguments(new[] { "new", "sln" })
            .WithWorkingDirectory(@"C:\Users\hunte\Desktop\TestLuthetus")
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(standardErrorBuilder))
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(standardInputBuilder))
            .WithValidation(CommandResultValidation.None)
            .ExecuteBufferedAsync();
    }

    [Fact]
    public void AdhocTest()
    {

    }
}