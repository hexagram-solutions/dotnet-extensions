using System.Net;
using System.Net.Http.Json;
using Hexagrams.Extensions.Common.Http;
using Hexagrams.Extensions.Common.Serialization;
using Hexagrams.Extensions.Testing.Http;

namespace Hexagrams.Extensions.Common.Tests.Http;

public class HttpClientExtensionsTests
{
    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record RequestType(string Value);

    // ReSharper disable once NotAccessedPositionalProperty.Local
    private record ResponseType(string Value);

    [Fact]
    public async Task PostAsJson_posts_json_request_and_deserializes_response()
    {
        // Arrange
        var requestValue = new RequestType("foo");

        var expectedResponseValue = new ResponseType("bar");

        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Created,
            Content = new StringContent(expectedResponseValue.ToJson())
        };

        var actualRequest = new HttpRequestMessage();

        var handlerFake = HttpTestUtilities.GetFakeHttpMessageHandler(httpResponse, (req, _) => actualRequest = req);

        var httpClient = new HttpClient(handlerFake);

        // Act
        var response = await httpClient
            .PostAsJsonAsync<RequestType, ResponseType>("http://example.com", requestValue);

        // Assert
        actualRequest.Method.Should().Be(HttpMethod.Post);

        var actualRequestValue = await actualRequest.Content!.ReadFromJsonAsync<RequestType>();
        actualRequestValue.Should().BeEquivalentTo(requestValue);

        response.Should().BeEquivalentTo(expectedResponseValue);
    }

    [Fact]
    public async Task PostAsJson_throws_exception_for_unsuccessful_request()
    {
        // Arrange
        var handlerFake = HttpTestUtilities.GetFakeHttpMessageHandler(
            new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest });

        var httpClient = new HttpClient(handlerFake);

        // Act
        var action = () =>
            httpClient.PostAsJsonAsync<RequestType, ResponseType>("http://example.com", new RequestType("foo"));

        // Assert
        await action.Should().ThrowAsync<HttpRequestException>();
    }


    [Fact]
    public async Task PutAsJson_posts_json_request_and_deserializes_response()
    {
        // Arrange
        var requestValue = new RequestType("foo");

        var expectedResponseValue = new ResponseType("bar");

        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(expectedResponseValue.ToJson())
        };

        var actualRequest = new HttpRequestMessage();

        var handlerFake = HttpTestUtilities.GetFakeHttpMessageHandler(httpResponse, (req, _) => actualRequest = req);

        var httpClient = new HttpClient(handlerFake);

        // Act
        var response = await httpClient
            .PutAsJsonAsync<RequestType, ResponseType>("http://example.com", requestValue);

        // Assert
        actualRequest.Method.Should().Be(HttpMethod.Put);

        response.Should().BeEquivalentTo(expectedResponseValue);

        var actualRequestValue = await actualRequest.Content!.ReadFromJsonAsync<RequestType>();
        actualRequestValue.Should().BeEquivalentTo(requestValue);
    }

    [Fact]
    public async Task PutAsJson_throws_exception_for_unsuccessful_request()
    {
        // Arrange
        var handlerFake = HttpTestUtilities.GetFakeHttpMessageHandler(
            new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest });

        var httpClient = new HttpClient(handlerFake);

        // Act
        var action = () =>
            httpClient.PutAsJsonAsync<RequestType, ResponseType>("http://example.com", new RequestType("foo"));

        // Assert
        await action.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task DeleteAsJson_posts_json_request_and_deserializes_response()
    {
        // Arrange
        var requestValue = new RequestType("foo");

        var httpResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.NoContent };

        var actualRequestContent = string.Empty;

#pragma warning disable xUnit1031 // Do not use blocking task operations in test method
        var handlerFake = HttpTestUtilities.GetFakeHttpMessageHandler(httpResponse,
            (req, ct) => actualRequestContent = req.Content!.ReadAsStringAsync(ct).Result);
#pragma warning restore xUnit1031 // Do not use blocking task operations in test method

        var httpClient = new HttpClient(handlerFake);

        // Act
        var response = await httpClient.DeleteAsJsonAsync("http://example.com", requestValue);

        // Assert
        var actualRequest = response!.RequestMessage;
        actualRequest!.Method.Should().Be(HttpMethod.Delete);

        var actualRequestValue = actualRequestContent.FromJson<RequestType>();
        actualRequestValue.Should().BeEquivalentTo(requestValue);
    }

    [Fact]
    public async Task DeleteAsJson_throws_exception_for_unsuccessful_request()
    {
        // Arrange
        var handlerFake = HttpTestUtilities.GetFakeHttpMessageHandler(
            new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest });

        var httpClient = new HttpClient(handlerFake);

        // Act
        var action = () => httpClient.DeleteAsJsonAsync("http://example.com", new RequestType("foo"));

        // Assert
        await action.Should().ThrowAsync<HttpRequestException>();
    }
}
