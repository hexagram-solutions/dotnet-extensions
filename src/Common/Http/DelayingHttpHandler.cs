namespace Hexagrams.Extensions.Common.Http;

/// <summary>
/// An implementation of <see cref="DelegatingHandler" /> that delays all HTTP requests made by the client by a
/// configured amount of time.
/// </summary>
/// <param name="delay">The amount of time to delay each request made through the handler.</param>
/// <remarks>
/// This is useful when simulating latencies in HTTP requests. For example, simulating the performance of a deployed
/// application environment when doing local development.
/// </remarks>
public class DelayingHttpHandler(TimeSpan delay) : DelegatingHandler
{
    /// <inheritdoc />
    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Thread.Sleep(delay);

        return base.Send(request, cancellationToken);
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
