using Polly;
using Polly.Retry;

namespace RequestService
{
    public class ClientPolicy
    {
        public AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get; }
        public AsyncRetryPolicy<HttpResponseMessage> LinearHttpRetry { get; set; }
        public AsyncRetryPolicy<HttpResponseMessage> ExponentialHttpRetry { get; set; }

        public ClientPolicy()
        {
            ImmediateHttpRetry = Policy.HandleResult<HttpResponseMessage>(
                response => !response.IsSuccessStatusCode)
                .RetryAsync(5);

            LinearHttpRetry = Policy.HandleResult<HttpResponseMessage>(
                response => !response.IsSuccessStatusCode)
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(3));

            ExponentialHttpRetry = Policy.HandleResult<HttpResponseMessage>(
                response => !response.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
