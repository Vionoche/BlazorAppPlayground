namespace BlazorApp.BackgroundTasks;

public class TimedHostedService : BackgroundService
{
    public TimedHostedService(ILogger<TimedHostedService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");
        
        await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Scoped Processing Service is working.");

                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Timed Hosted Service has thrown an Exception.");
        }
    }

    private readonly ILogger<TimedHostedService> _logger;
}