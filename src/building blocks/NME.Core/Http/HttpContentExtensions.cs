using System.Text.Json;

namespace NME.Core.Http;

public static class HttpContentExtensions
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<T?> DeserializarObjetoResponseAsync<T>(this HttpResponseMessage responseMessage)
    {
        var stringData = await responseMessage.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(stringData))
            return default;

        return JsonSerializer.Deserialize<T>(stringData, DefaultOptions);
    }

    public static StringContent ObterConteudoString(object dado)
    {
        var json = JsonSerializer.Serialize(dado, DefaultOptions);
        return new StringContent(json, System.Text.Encoding.UTF8, "application/json");
    }
}