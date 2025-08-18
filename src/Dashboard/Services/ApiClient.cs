using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Dashboard.Services;

public sealed class ApiClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _cfg;

    public ApiClient(HttpClient http, IConfiguration cfg)
    {
        _http = http;
        _cfg = cfg;
        _http.BaseAddress = new Uri(_cfg["ApiBaseUrl"]!);
    }

    public void SetBearer(string token)
    {
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<T?> GetAsync<T>(string path)
    {
        var res = await _http.GetAsync(path);
        res.EnsureSuccessStatusCode();
        return await JsonSerializer.DeserializeAsync<T>(await res.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<TOut?> PostAsync<TIn, TOut>(string path, TIn body)
    {
        var json = JsonSerializer.Serialize(body);
        var res = await _http.PostAsync(path, new StringContent(json, Encoding.UTF8, "application/json"));
        res.EnsureSuccessStatusCode();
        return await JsonSerializer.DeserializeAsync<TOut>(await res.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}


