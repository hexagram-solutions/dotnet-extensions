using System.Net;
using FakeItEasy;
using Hexagrams.Extensions.Authentication.OAuth;
using Hexagrams.Extensions.Testing;
using Hexagrams.Extensions.Testing.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Hexagrams.Extensions.Authentication.Tests.OAuth;

public class DummyHttpService(HttpClient client)
{
    public Task MakeRequest()
    {
        return client.GetAsync("http://example.com");
    }
}

public class BearerTokenAuthenticationHandlerTests
{
    [Fact]
    public Task Access_tokens_are_added_in_authentication_header_of_request()
    {
        var actualRequest = new HttpRequestMessage();

        const string accessTokenValue = "access_token";

        var fakeAccessTokenProvider = A.Fake<IAccessTokenProvider>();

        A.CallTo(() => fakeAccessTokenProvider.GetAccessTokenAsync(A<string[]>._))
            .Returns(Task.FromResult(new AccessTokenResponse { AccessToken = accessTokenValue }));

        async Task TestAction(DummyHttpService service)
        {
            await service.MakeRequest();

            var authorizationHeader = actualRequest.Headers.Authorization;

            authorizationHeader!.Scheme.Should().Be("Bearer");
            authorizationHeader.Parameter.Should().Be(accessTokenValue);
        }

        return ServiceTestHarness<DummyHttpService>.Create(TestAction)
            .WithServices(services =>
            {
                services.AddAccessTokenProvider(
                    configure => configure.UseCustomProvider(fakeAccessTokenProvider));

                services.AddTransient(_ => new HttpTestUtilities.DelegatingHandlerStub((request, _) =>
                {
                    actualRequest = request;

                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
                }));

                services.AddTransient(sp =>
                    new BearerTokenAuthenticationHandler(
                        sp.GetRequiredService<IAccessTokenProvider>(),
                        "foo", "bar"));

                services.AddHttpClient<DummyHttpService>()
                    .AddHttpMessageHandler<BearerTokenAuthenticationHandler>()
                    .AddHttpMessageHandler<HttpTestUtilities.DelegatingHandlerStub>();
            })
            .TestAsync();
    }
}
