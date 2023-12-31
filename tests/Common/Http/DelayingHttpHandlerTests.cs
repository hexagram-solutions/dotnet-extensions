using System.Net;
using Hexagrams.Extensions.Common.Http;
using Hexagrams.Extensions.Testing;
using Hexagrams.Extensions.Testing.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Hexagrams.Extensions.Common.Tests.Http;

public class DelayingHttpHandlerTests
{
    [Fact]
    public async Task Requests_are_delayed_by_the_configured_amount_of_time()
    {
        var delay = TimeSpan.FromMilliseconds(500);

        using var httpClient = new HttpClient(new DelayingHttpHandler(delay));

        async Task TestAction(DummyHttpService service)
        {
            var task = service.MakeRequest;

            await task.Should().NotCompleteWithinAsync(delay);
        }

        await ServiceTestHarness<DummyHttpService>.Create(TestAction)
            .WithServices(services =>
            {
                services.AddTransient(_ => new DelayingHttpHandler(delay));

                services.AddTransient(_ =>
                    new HttpTestUtilities.DelegatingHandlerStub((_, _) =>
                        Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))));

                services.AddHttpClient<DummyHttpService>()
                    .AddHttpMessageHandler<DelayingHttpHandler>()
                    .AddHttpMessageHandler<HttpTestUtilities.DelegatingHandlerStub>();
            })
            .TestAsync();
    }

    public class DummyHttpService(HttpClient client)
    {
        public Task MakeRequest()
        {
            return client.GetAsync("http://example.com");
        }
    }
}
