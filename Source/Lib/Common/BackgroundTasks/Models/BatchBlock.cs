namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

/// <summary>
/// Consider the interactions between 'BatchOrDefault' and 'HandleEvent'.
/// 
/// At some point there probably is a need to create a shallow copy,
/// of the BatchBlock, so that during 'HandleEvent',
/// a good reference to the 'BatchBlock' can be used thread safely.
/// </summary>
public class BatchBlock
{
	public void AddToBatch(KeymapArgs keymapArgs)
	{
		if (!BatchHasAvailability)
			throw new LuthetusTextEditorException($"{nameof(BatchLength)} >= {nameof(MAX_BATCH_SIZE)}");

		KeymapArgsList[BatchLength] = keymapArgs;
        BatchLength++;
    }

    public IBackgroundTask? BatchOrDefault(IBackgroundTask upstreamEvent)
    {
		if (upstreamEvent is OnKeyDownLateBatching upstreamOnKeyDownLateBatching)
		{
			if (BatchLength == 1 && upstreamOnKeyDownLateBatching.BatchHasAvailability)
				upstreamOnKeyDownLateBatching.AddToBatch(KeymapArgsList[0]);

            return upstreamOnKeyDownLateBatching;
		}

		return null;
    }
}
