using Quartz;

namespace IPv64IPScanner;

[DisallowConcurrentExecution]
public class ExecuteValidateIp : IJob
{
    private readonly ILogger<ExecuteValidateIp> _logger;

    public ExecuteValidateIp(ILogger<ExecuteValidateIp> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Starting validation Job");
        await Task.Run(() => new JobValidateIp().Execute());
    }
}