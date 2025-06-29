using Polly;
using Polly.Retry;
using Polly.Fallback;
using Microsoft.Extensions.Logging;

namespace PagueVeloz.Challenge.Infrastructure.Resilience
{
    public static class PolicyConfiguration
    {
        public static AsyncRetryPolicy RetryPolicy(ILogger logger) =>
            Policy.Handle<HttpRequestException>()
                  .Or<TimeoutException>()
                  .Or<Microsoft.EntityFrameworkCore.DbUpdateException>(ex =>
                  {

                      return true;
                  })
                  .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                      onRetry: (exception, timeSpan, retryCount, context) =>
                      {
                          logger.LogWarning(exception, $"Tentativa {retryCount} de retry para {context.PolicyKey} após {timeSpan.TotalSeconds:N1}s. Erro: {exception.Message}");
                      });

        public static AsyncFallbackPolicy FallbackPolicy(ILogger logger) =>
            Policy.Handle<Exception>()
                    .FallbackAsync(
                    fallbackAction: async (exception, context, cancellationToken) => 
                    {
                        logger.LogError(exception, $"Fallback ativado para {context.PolicyKey}. Erro original: {exception.Message}");
                        await Task.CompletedTask; 
                    },
                    onFallbackAsync: (exception, context) =>
                    {
                        logger.LogWarning($"OnFallbackAsync executado para {context.PolicyKey}. Detalhes adicionais podem ser logados aqui.");
                        return Task.CompletedTask;
                    });

        public static AsyncPolicy DefaultAsyncPolicy(ILogger logger) =>
            Policy.WrapAsync(FallbackPolicy(logger), RetryPolicy(logger));
    }
}