using System.Threading.Channels;
using AT2Soft.RAGEngine.Application.Abstractions.Queue;

namespace AT2Soft.RAGEngine.Infrastructure.Queue;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    //private readonly Channel<Func<CancellationToken, Task>> _queue =
    //    Channel.CreateUnbounded<Func<CancellationToken, Task>>();

    //public void Queue(Func<CancellationToken, Task> workItem) => _queue.Writer.TryWrite(workItem);

    //public Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken) =>
    //    _queue.Reader.ReadAsync(cancellationToken).AsTask();

    private readonly Channel<Func<IServiceProvider, CancellationToken, Task>> _channel;

    public BackgroundTaskQueue(int capacity = 1000)
    {
        // Bounded = control de memoria + backpressure
        var options = new BoundedChannelOptions(capacity)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        };
        _channel = Channel.CreateBounded<Func<IServiceProvider, CancellationToken, Task>>(options);
    }

    public ValueTask EnqueueAsync(
        Func<IServiceProvider, CancellationToken, Task> workItem,
        CancellationToken cancellationToken = default)
        => _channel.Writer.WriteAsync(workItem, cancellationToken);

    public ValueTask<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(
        CancellationToken cancellationToken)
        => _channel.Reader.ReadAsync(cancellationToken);
}
