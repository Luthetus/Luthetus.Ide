namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.NewInterfaceCase;

public class CSharpCompilerServiceTests
{
    [Fact]
    public async Task SHOULD_REGISTER_MODEL()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var compilerServiceQueuedHostedService = default(CompilerServiceQueuedHostedService?);

        try
        {
            using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Trace));

            var compilerServiceBackgroundTaskQueue = new CompilerServiceBackgroundTaskQueue();

            compilerServiceQueuedHostedService = ConstructUnitTestsCompilerServiceQueuedHostedService(
                loggerFactory,
                compilerServiceBackgroundTaskQueue);

            _ = Task.Run(async () =>
            {
                await compilerServiceQueuedHostedService.StartAsync(cancellationToken);
            });

            var cSharpCompilerService = new CSharpCompilerService(compilerServiceBackgroundTaskQueue);

            var resourceUri = new ResourceUri("Program.cs");

            var content = @"var x = 2;";

            var textEditorModel = new TextEditorModel(
                resourceUri,
                DateTime.UtcNow,
                ".cs",
                content,
                cSharpCompilerService,
                null,
                null,
                new());

            cSharpCompilerService.RegisterModel(textEditorModel);
        }
        catch
        {
            cancellationTokenSource.Cancel();
        }
        finally
        {
            if (compilerServiceQueuedHostedService is not null)
                await compilerServiceQueuedHostedService.StopAsync(cancellationToken);
        }
    }

    private CompilerServiceQueuedHostedService ConstructUnitTestsCompilerServiceQueuedHostedService(
        ILoggerFactory loggerFactory,
        ICompilerServiceBackgroundTaskQueue compilerServiceBackgroundTaskQueue)
    {
        var nullLuthetusCommonComponentRenderers =
            new LuthetusCommonComponentRenderers(null, null, null, null, null, null, null, null);

        var compilerServiceBackgroundTaskMonitor = new CompilerServiceBackgroundTaskMonitor(nullLuthetusCommonComponentRenderers);

        return new CompilerServiceQueuedHostedService(
            compilerServiceBackgroundTaskQueue,
            compilerServiceBackgroundTaskMonitor,
            nullLuthetusCommonComponentRenderers,
            loggerFactory);
    }
}
