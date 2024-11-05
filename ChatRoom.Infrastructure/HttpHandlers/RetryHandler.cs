using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace ChatRoom.Infrastructure.HttpHandlers;

public sealed class RetryHandler : DelegatingHandler
{
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    public RetryHandler(ILoggerFactory loggerFactory)
    {
        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (result, timeSpan, retryCount, context) =>
                {
                    loggerFactory.CreateLogger<RetryHandler>()
                        .LogWarning("Request failed with {StatusCode}. Waiting {TimeSpan} before next retry. Retry attempt {RetryCount}.",
                            result.Result.StatusCode, timeSpan, retryCount);
                });
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            return response;
        });
    }
}
