using System.Net;
using System.Net.Http.Json;
using FakeItEasy;
using Hexagrams.Extensions.Authentication.OAuth;
using Hexagrams.Extensions.Authentication.OAuth.Internal;
using Hexagrams.Extensions.Common.Serialization;
using Hexagrams.Extensions.Testing;
using Hexagrams.Extensions.Testing.Http;

namespace Hexagrams.Extensions.Authentication.Tests.OAuth;

public class ClientCredentialsAccessTokenProviderTests
{
    [Fact]
    public async Task Returns_expected_access_token()
    {
        // Arrange
        var response = new AccessTokenResponse
        {
            AccessToken = "foobar",
            ExpiresIn = (int) TimeSpan.FromDays(1).TotalSeconds,
            TokenType = "bearer"
        };

        var handlerFake = GetFakeHttpMessageHandlerWithResponse(response);

        await ServiceTestHarness<ClientCredentialsAccessTokenProvider>.Create(TestAction)
            .WithDependency(new HttpClient(handlerFake))
            .WithServices(sp =>
            {
                sp.AddAccessTokenProvider(builder =>
                {
                    builder.UseClientCredentialsFlow(opts =>
                    {
                        opts.TokenEndpoint = new Uri("https://test.com");
                        opts.ClientId = "foo";
                        opts.ClientSecret = "bar";
                    });
                });
            })
            .TestAsync();

        async Task TestAction(IAccessTokenProvider provider)
        {
            // Act
            var actualResponse = await provider.GetAccessTokenAsync();

            // Assert
            actualResponse.Should().BeEquivalentTo(response);
        }
    }

    [Fact]
    public async Task Inner_client_posts_request_as_configured()
    {
        // Arrange
        var response = new AccessTokenResponse
        {
            AccessToken = "foobar",
            ExpiresIn = (int) TimeSpan.FromDays(1).TotalSeconds,
            TokenType = "bearer"
        };

        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(response.ToJson())
        };

        var actualRequest = new HttpRequestMessage();

        var handlerFake = HttpTestUtilities.GetFakeHttpMessageHandler(httpResponse, (req, _) => actualRequest = req);

        var providerOptions = new ClientCredentialsProviderOptions
        {
            TokenEndpoint = new Uri("https://test.com/"),
            ClientId = "foo",
            ClientSecret = "bar",
            AdditionalProperties = new Dictionary<string, string> { { "foo", "bar" } }
        };

        await ServiceTestHarness<ClientCredentialsAccessTokenProvider>.Create(TestAction)
            .WithDependency(new HttpClient(handlerFake))
            .WithServices(sp =>
            {
                sp.AddAccessTokenProvider(builder =>
                {
                    builder.UseClientCredentialsFlow(providerOptions);
                });
            })
            .TestAsync();

        async Task TestAction(IAccessTokenProvider provider)
        {
            const string someScope = "some_scope";

            // Act
            var result = await provider.GetAccessTokenAsync(someScope);

            // Assert
            result.AccessToken.Should().Be(response.AccessToken);

            actualRequest.RequestUri.Should().Be(providerOptions.TokenEndpoint.ToString());

            var actualRequestContent =
                await actualRequest.Content!.ReadFromJsonAsync<ClientCredentialsAccessTokenRequest>();

            actualRequestContent!.ClientId.Should().Be(providerOptions.ClientId);
            actualRequestContent.ClientSecret.Should().Be(providerOptions.ClientSecret);
            actualRequestContent.Scope.Should().Be(someScope);

            actualRequestContent.AdditionalProperties!.Keys
                .Should().BeEquivalentTo(providerOptions.AdditionalProperties.Keys);

            actualRequestContent.AdditionalProperties.Values.Select(v => v.ToString())
                .Should().BeEquivalentTo(providerOptions.AdditionalProperties.Values);

            actualRequestContent.GrantType.Should().Be("client_credentials");
        }
    }

    [Fact]
    public async Task Throws_exception_for_unsuccessful_request()
    {
        // Arrange
        var handlerFake = HttpTestUtilities.GetFakeHttpMessageHandler(
            new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest });

        var providerOptions = new ClientCredentialsProviderOptions
        {
            TokenEndpoint = new Uri("https://test.com/"),
            ClientId = "foo",
            ClientSecret = "bar",
            AdditionalProperties = new Dictionary<string, string> { { "foo", "bar" } }
        };

        await ServiceTestHarness<ClientCredentialsAccessTokenProvider>.Create(TestAction)
            .WithDependency(new HttpClient(handlerFake))
            .WithServices(sp =>
            {
                sp.AddAccessTokenProvider(builder =>
                {
                    builder.UseClientCredentialsFlow(providerOptions);
                });
            })
            .TestAsync();

        async Task TestAction(IAccessTokenProvider provider)
        {
            // Act
            var action = async () => await provider.GetAccessTokenAsync();

            // Assert
            await action.Should().ThrowAsync<HttpRequestException>();
        }
    }

    private static HttpMessageHandler GetFakeHttpMessageHandlerWithResponse(AccessTokenResponse response)
    {
        return HttpTestUtilities.GetFakeHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(response.ToJson())
        });
    }
}
