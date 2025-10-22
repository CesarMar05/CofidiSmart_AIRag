using System;

namespace AT2Soft.RAGEngine.Application.Abstractions.Queue;

public interface IBackgroundTaskQueue
{
    //void Queue(Func<CancellationToken, Task> workItem);
    //Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);

    ValueTask EnqueueAsync(
        Func<IServiceProvider, CancellationToken, Task> workItem,
        CancellationToken cancellationToken = default);

    // Devuelve el próximo trabajo cuando esté disponible
    ValueTask<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(
        CancellationToken cancellationToken);
}
