using FakeItEasy;

namespace Hexagrams.Extensions.Testing.Http;

/// <summary>
/// Utility methods and classes for faking and capturing HTTP responses.
/// </summary>
public static class HttpTestUtilities
{
    /// <summary>
    /// Creates a fake <see cref="HttpMessageHandler" /> that returns a supplied <see cref="HttpResponseMessage" />.
    /// </summary>
    /// <param name="fakeResponse">The expected result of any HTTP request that passes through this handler.</param>
    /// <param name="requestCallback">
    /// A callback to invoke when the <see cref="HttpMessageHandler" /> handles a request.
    /// </param>
    /// <returns>The <see cref="HttpMessageHandler" /> fake.</returns>
    public static HttpMessageHandler GetFakeHttpMessageHandler(HttpResponseMessage fakeResponse,
        Action<HttpRequestMessage, CancellationToken>? requestCallback = null)
    {
        var handler = A.Fake<HttpMessageHandler>();

        A.CallTo(handler)
            .WithReturnType<Task<HttpResponseMessage>>()
            .Where(call => call.Method.Name == nameof(HttpClient.SendAsync))
            .Invokes(requestCallback ?? ((_, _) =>
            {
                /* no-op */
            }))
            .Returns(fakeResponse);

        return handler;
    }

    /// <summary>
    /// A <see cref="DelegatingHandler" /> that invokes a specified function when it handles a
    /// <see cref="HttpRequestMessage" />.
    /// </summary>
    /// <param name="handlerFunc">The function to invoke when handling <see cref="HttpRequestMessage" /></param>
    public class DelegatingHandlerStub(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc) : DelegatingHandler
    {
        /// <inheritdoc />
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return handlerFunc(request, cancellationToken);
        }
    }
}
