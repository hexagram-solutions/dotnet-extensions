using FakeItEasy;
using Hexagrams.Extensions.Authentication.OAuth;
using Hexagrams.Extensions.Testing;

namespace Hexagrams.Extensions.Authentication.Tests.OAuth;

public class InMemoryCachingAccessTokenProviderTests
{
    [Fact]
    public async Task Unexpired_access_token_responses_are_retrieved_from_cache()
    {
        // Arrange
        var fakeAccessTokenProvider = A.Fake<IAccessTokenProvider>();

        A.CallTo(() => fakeAccessTokenProvider.GetAccessTokenAsync(A<string[]>._))
            .Returns(Task.FromResult(new AccessTokenResponse
            {
                AccessToken = Guid.NewGuid().ToString(), // Repeated calls will generate different values
                ExpiresIn = (int) TimeSpan.FromHours(1).TotalSeconds,
                TokenType = "dummy",
                Scope = "dummy_scope"
            }));

        await ServiceTestHarness<InMemoryCachingAccessTokenProvider>.Create(TestAction)
            .WithServices(sp =>
            {
                sp.AddAccessTokenProvider(builder =>
                {
                    builder.UseCustomProvider(fakeAccessTokenProvider)
                        .WithInMemoryCaching();
                });
            })
            .TestAsync();

        static async Task TestAction(IAccessTokenProvider provider)
        {
            // Act
            var firstResponse = await provider.GetAccessTokenAsync();

            var secondResponse = await provider.GetAccessTokenAsync();

            // Assert
            firstResponse.AccessToken.Should().Be(secondResponse.AccessToken);
        }
    }

    [Fact]
    public async Task Expired_access_token_responses_are_refreshed()
    {
        // Arrange
        var fakeAccessTokenProvider = A.Fake<IAccessTokenProvider>();

        var tokenExpiryWindow = TimeSpan.FromSeconds(1);

        A.CallTo(() => fakeAccessTokenProvider.GetAccessTokenAsync(A<string[]>._))
            .ReturnsLazily(() => Task.FromResult(new AccessTokenResponse
            {
                AccessToken = Guid.NewGuid().ToString(), // Repeated calls will generate different values
                ExpiresIn = (int) tokenExpiryWindow.TotalSeconds,
                TokenType = "dummy",
                Scope = "dummy_scope"
            }));

        await ServiceTestHarness<InMemoryCachingAccessTokenProvider>.Create(TestAction)
            .WithServices(sp =>
            {
                sp.AddAccessTokenProvider(builder =>
                {
                    builder.UseCustomProvider(fakeAccessTokenProvider)
                        .WithInMemoryCaching(opts =>
                        {
                            opts.ExpirationBuffer = tokenExpiryWindow / 2;
                        });
                });
            })
            .TestAsync();

        async Task TestAction(IAccessTokenProvider provider)
        {
            // Act
            var firstResponse = await provider.GetAccessTokenAsync();

            await Task.Delay(tokenExpiryWindow);

            var secondResponse = await provider.GetAccessTokenAsync();

            // Assert
            firstResponse.AccessToken.Should().NotBe(secondResponse.AccessToken);
        }
    }

    [Fact]
    public async Task Throws_exception_if_expiration_buffer_is_greater_than_token_expiry()
    {
        // Arrange
        var fakeAccessTokenProvider = A.Fake<IAccessTokenProvider>();

        var tokenExpiryWindow = TimeSpan.FromSeconds(1);

        A.CallTo(() => fakeAccessTokenProvider.GetAccessTokenAsync(A<string[]>._))
            .Returns(Task.FromResult(new AccessTokenResponse { ExpiresIn = (int) tokenExpiryWindow.TotalSeconds }));

        await ServiceTestHarness<InMemoryCachingAccessTokenProvider>.Create(TestAction)
            .WithServices(sp =>
            {
                sp.AddAccessTokenProvider(builder =>
                {
                    builder.UseCustomProvider(fakeAccessTokenProvider)
                        .WithInMemoryCaching(opts =>
                        {
                            opts.ExpirationBuffer = tokenExpiryWindow.Add(TimeSpan.FromTicks(1));
                        });
                });
            })
            .TestAsync();

        async Task TestAction(IAccessTokenProvider provider)
        {
            // Act
            var action = async () => await provider.GetAccessTokenAsync();

            // Assert
            await action.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
