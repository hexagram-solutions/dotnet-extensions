using System.Net.Http.Json;
using Hexagrams.Extensions.Authentication.OAuth.Internal;
using Microsoft.Extensions.Options;

namespace Hexagrams.Extensions.Authentication.OAuth;

/// <summary>
/// The options values for acquiring access tokens using the
/// <see href="https://datatracker.ietf.org/doc/html/rfc6749#section-1.3.3">resource owner password credentials</see>
/// flow.
/// </summary>
public record ResourceOwnerPasswordProviderOptions : AccessTokenProviderOptions,
    IOptions<ResourceOwnerPasswordProviderOptions>
{
    /// <summary>
    /// The resource owner username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The resource owner password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The client secret.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// The default configured M<see cref="ResourceOwnerPasswordProviderOptions" /> instance.
    /// </summary>
    public ResourceOwnerPasswordProviderOptions Value => this;
}

/// <summary>
/// Acquires an OAuth 2.0 access token using the
/// <see href="https://datatracker.ietf.org/doc/html/rfc6749#section-1.3.3">resource owner password credentials</see>
/// flow.
/// </summary>
/// <remarks>
/// This flow is <b>NOT RECOMMENDED</b>. In most scenarios, more secure alternatives are available. This flow requires
/// a very high degree of trust in the application, and carries risks that are not present in other flows. This flow
/// should <b>only</b> be used when other more secure flows aren't viable.
/// </remarks>
/// <param name="httpClient">The <see cref="HttpClient" /> used to make request.</param>
/// <param name="options">The configuration options for this provider.</param>
public class ResourceOwnerPasswordAccessTokenProvider(HttpClient httpClient,
        IOptions<ResourceOwnerPasswordProviderOptions> options)
    : IAccessTokenProvider
{
    /// <inheritdoc />
    public async Task<AccessTokenResponse> GetAccessTokenAsync(params string[] scopes)
    {
        var scopesValue = string.Join(" ", scopes);

        var request = new ResourceOwnerPasswordAccessTokenRequest
        {
            Username = options.Value.Username,
            Password = options.Value.Password,
            ClientId = options.Value.ClientId,
            ClientSecret = options.Value.ClientSecret,
            Scope = scopesValue,
            AdditionalProperties =
                options.Value.AdditionalProperties?.ToDictionary(k => k.Key, v => (object) v.Value)
        };

        using var response = await httpClient.PostAsJsonAsync(options.Value.TokenEndpoint, request)
            .ConfigureAwait(false);

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            throw new HttpRequestException($"Failed to retrieve access token from {options.Value.TokenEndpoint} " +
                $"for client {options.Value.ClientId} using grant type {request.GrantType}", ex);
        }

        var accessTokenResponse = await response.Content.ReadFromJsonAsync<AccessTokenResponse>().ConfigureAwait(false);

        return accessTokenResponse!;
    }
}
