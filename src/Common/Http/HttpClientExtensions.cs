using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Hexagrams.Extensions.Common.Serialization;

namespace Hexagrams.Extensions.Common.Http;

/// <summary>
/// Extension methods for <see cref="HttpClient" />.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Sends a POST request to the specified Uri containing the <paramref name="value" /> serialized as JSON in the
    /// request body and deserializes the response from JSON to a new instance of <typeparamref name="TResponse" />.
    /// </summary>
    /// <typeparam name="TRequest">The type of the value to serialize.</typeparam>
    /// <typeparam name="TResponse">The type of the response to deserialize.</typeparam>
    /// <param name="client">The client used to send the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="options">
    /// Options to control the behavior during serialization. The default options are those specified by
    /// <see cref="JsonSerializerDefaults.Web" />.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
    /// </param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="client" /> is null.</exception>
    /// <exception cref="HttpRequestException">The request was unsuccessful.</exception>
    public static Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(this HttpClient client, string? requestUri,
        TRequest value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        return client.PostAsJsonAsync<TRequest, TResponse>(CreateUri(requestUri), value, options, cancellationToken);
    }

    /// <summary>
    /// Sends a POST request to the specified Uri containing the <paramref name="value" /> serialized as JSON in the
    /// request body and deserializes the response from JSON to a new instance of <typeparamref name="TResponse" />.
    /// </summary>
    /// <typeparam name="TRequest">The type of the value to serialize.</typeparam>
    /// <typeparam name="TResponse">The type of the response to deserialize.</typeparam>
    /// <param name="client">The client used to send the request.</param>
    /// <param name="requestUri">The <see cref="Uri" /> the request is sent to.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="options">
    /// Options to control the behavior during serialization. The default options are those specified by
    /// <see cref="JsonSerializerDefaults.Web" />.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
    /// </param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="client" /> is null.</exception>
    /// <exception cref="HttpRequestException">The request was unsuccessful.</exception>
    public static async Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(this HttpClient client, Uri? requestUri,
        TRequest value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        var response = await client.PostAsJsonAsync(requestUri, value, options, cancellationToken)
            .ConfigureAwait(false);

        return await response
            .EnsureSuccessStatusCode()
            .Content.ReadFromJsonAsync<TResponse>(options, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a PUT request to the specified Uri containing the <paramref name="value" /> serialized as JSON in the
    /// request body and deserializes the response from JSON to a new instance of <typeparamref name="TResponse" />.
    /// </summary>
    /// <typeparam name="TRequest">The type of the value to serialize.</typeparam>
    /// <typeparam name="TResponse">The type of the response to deserialize.</typeparam>
    /// <param name="client">The client used to send the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="options">
    /// Options to control the behavior during serialization. The default options are those specified by
    /// <see cref="JsonSerializerDefaults.Web" />.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
    /// </param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="client" /> is null.</exception>
    /// <exception cref="HttpRequestException">The request was unsuccessful.</exception>
    public static Task<TResponse?> PutAsJsonAsync<TRequest, TResponse>(this HttpClient client, string? requestUri,
        TRequest value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        return client.PutAsJsonAsync<TRequest, TResponse>(CreateUri(requestUri), value, options, cancellationToken);
    }

    /// <summary>
    /// Sends a PUT request to the specified Uri containing the <paramref name="value" /> serialized as JSON in the
    /// request body and deserializes the response from JSON to a new instance of <typeparamref name="TResponse" />.
    /// </summary>
    /// <typeparam name="TRequest">The type of the value to serialize.</typeparam>
    /// <typeparam name="TResponse">The type of the response to deserialize.</typeparam>
    /// <param name="client">The client used to send the request.</param>
    /// <param name="requestUri">The <see cref="Uri" /> the request is sent to.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="options">
    /// Options to control the behavior during serialization. The default options are those specified by
    /// <see cref="JsonSerializerDefaults.Web" />.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
    /// </param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="client" /> is null.</exception>
    /// <exception cref="HttpRequestException">The request was unsuccessful.</exception>
    public static async Task<TResponse?> PutAsJsonAsync<TRequest, TResponse>(this HttpClient client, Uri? requestUri,
        TRequest value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        var response = await client.PutAsJsonAsync(requestUri, value, options, cancellationToken).ConfigureAwait(false);

        return await response
            .EnsureSuccessStatusCode()
            .Content.ReadFromJsonAsync<TResponse>(options, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a DELETE request to the specified Uri containing the <paramref name="value" /> serialized as JSON in the
    /// request body.
    /// </summary>
    /// <typeparam name="TRequest">The type of the value to serialize.</typeparam>
    /// <param name="client">The client used to send the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="options">
    /// Options to control the behavior during serialization. The default options are those specified by
    /// <see cref="JsonSerializerDefaults.Web" />.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
    /// </param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="client" /> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="value" /> is null.</exception>
    /// <exception cref="HttpRequestException">The request was unsuccessful.</exception>
    public static Task<HttpResponseMessage?> DeleteAsJsonAsync<TRequest>(this HttpClient client, string? requestUri,
        TRequest value, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        return client.DeleteAsJsonAsync(CreateUri(requestUri), value, options, cancellationToken);
    }

    /// <summary>
    /// Sends a DELETE request to the specified Uri containing the <paramref name="value" /> serialized as JSON in the
    /// request body.
    /// </summary>
    /// <typeparam name="TRequest">The type of the value to serialize.</typeparam>
    /// <param name="client">The client used to send the request.</param>
    /// <param name="requestUri">The <see cref="Uri" /> the request is sent to.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="options">
    /// Options to control the behavior during serialization. The default options are those specified by
    /// <see cref="JsonSerializerDefaults.Web" />.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
    /// </param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="client" /> is null.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="value" /> is null.</exception>
    /// <exception cref="HttpRequestException">The request was unsuccessful.</exception>
    public static async Task<HttpResponseMessage?> DeleteAsJsonAsync<TRequest>(this HttpClient client,
        Uri? requestUri, TRequest value, JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(value);

        options ??= new JsonSerializerOptions(JsonSerializerDefaults.Web);

        using var request = new HttpRequestMessage();

        request.Method = HttpMethod.Delete;
        request.RequestUri = requestUri;
        request.Content = new StringContent(value.ToJson(options), Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

        response.RequestMessage = request;

        return response.EnsureSuccessStatusCode();
    }

    private static Uri? CreateUri(string? uri)
    {
        return string.IsNullOrEmpty(uri) ? null : new Uri(uri, UriKind.RelativeOrAbsolute);
    }
}
