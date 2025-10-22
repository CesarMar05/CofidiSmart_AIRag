using AT2Soft.RAGEngine.Application.Abstractions.Queue;

namespace AT2Soft.RAGEngine.WebAPI.BackgroundServices;

public class RagIngestWorker : BackgroundService
{
    //private readonly IBackgroundTaskQueue _queue;
    //private readonly IServiceProvider _sp;

    //public RagIngestWorker(IBackgroundTaskQueue queue, IServiceProvider sp)
    //{
    //    _queue = queue; _sp = sp;
    //}

    private readonly IBackgroundTaskQueue _queue;
    private readonly IServiceProvider _rootProvider;
    private readonly ILogger<RagIngestWorker> _logger;

    public RagIngestWorker(
        IBackgroundTaskQueue queue,
        IServiceProvider rootProvider,
        ILogger<RagIngestWorker> logger)
    {
        _queue = queue;
        _rootProvider = rootProvider;
        _logger = logger;
    }


    //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //{
        //while (!stoppingToken.IsCancellationRequested)
        //{
            //var work = await _queue.DequeueAsync(stoppingToken);
            //try
            //{
                //await work(stoppingToken);
            //}
            //catch
            //{
                /* log */
            //}
        //}
    //}

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RagIngestWorker iniciado.");
        while (!stoppingToken.IsCancellationRequested)
        {
            Func<IServiceProvider, CancellationToken, Task>? work = null;
            try
            {
                work = await _queue.DequeueAsync(stoppingToken);

                // Crear scope por trabajo (para scoped services como DbContext)
                using var scope = _rootProvider.CreateScope();

                await work(scope.ServiceProvider, stoppingToken);

                _logger.LogInformation("cola terminada");
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Apagando…
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando work item.");
                // Opcional: métrica / contador de fallos
            }
        }
        _logger.LogInformation("RagIngestWorker detenido.");
    }
}
