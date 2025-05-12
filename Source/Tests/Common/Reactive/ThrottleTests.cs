namespace Luthetus.Common.Tests.Reactive;

public class ThrottleTests
{
	[Fact]
	public async Task Aaa()
	{
		var count = 0;
		
		var countTask = Task.Run(async () =>
		{
			while (true)
			{
				count++;
				Console.WriteLine(count);
				await Task.Delay(100);
			}
		});
		
		await countTask;
	}
}

public class Throttle
{
}
