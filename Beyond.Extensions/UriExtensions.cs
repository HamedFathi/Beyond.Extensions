// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Beyond.Extensions.UriExtensions;

public static class UriExtensions
{
    public static byte[] ReadAsBytesArray(this Uri uri)
    {
        var httpClient = new HttpClient();

        var response = httpClient.GetAsync(uri).GetAwaiter().GetResult();
        var content = response.Content.ReadAsStream();

        using var memoryStream = new MemoryStream();
        content.CopyTo(memoryStream);

        return memoryStream.ToArray();
    }

    public static async Task<byte[]> ReadAsBytesArrayAsync(this Uri uri, CancellationToken cancellationToken = default)
    {
        var httpClient = new HttpClient();

        var response = await httpClient.GetAsync(uri, cancellationToken);
        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    public static Stream ReadAsStream(this Uri uri)
    {
        var httpClient = new HttpClient();

        var response = httpClient.GetAsync(uri).GetAwaiter().GetResult();
        return response.Content.ReadAsStream();
    }

    public static async Task<Stream> ReadAsStreamAsync(this Uri uri, CancellationToken cancellationToken = default)
    {
        var httpClient = new HttpClient();

        var response = await httpClient.GetAsync(uri, cancellationToken);
        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }

    public static string ReadAsString(this Uri uri)
    {
        var httpClient = new HttpClient();

        var response = httpClient.GetAsync(uri).GetAwaiter().GetResult();
        var content = response.Content.ReadAsStream();
        var reader = new StreamReader(content);
        return reader.ReadToEnd();
    }

    public static async Task<string> ReadAsStringAsync(this Uri uri, CancellationToken cancellationToken = default)
    {
        var httpClient = new HttpClient();

        var response = await httpClient.GetAsync(uri, cancellationToken);
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}