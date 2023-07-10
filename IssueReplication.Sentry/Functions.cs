using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace IssueReplication.Sentry;

public class Functions
{
    private readonly ILogger<Functions> _logger;

    public Functions(ILogger<Functions> logger)
    {
        _logger = logger;
    }
    
    [Function(nameof(HttpStartFakeOrchestrator))]
    public async Task<HttpResponseData> HttpStartFakeOrchestrator(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext
    ) {
        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(FakeOrchestrator));
        return client.CreateCheckStatusResponse(req, instanceId);
    }

    [Function(nameof(FakeOrchestrator))]
    public async Task FakeOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context
    )
    {
        await context.CallActivityAsync<int>(nameof(FakeActivity), 1);
    }

    [Function(nameof(FakeActivity))]
    public async Task<int> FakeActivity([ActivityTrigger] int batchNumber)
    {
        _logger.LogError("Send log error to Sentry");
        return 1;
    }
}