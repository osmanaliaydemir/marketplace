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
        if (res.Content.Headers.ContentLength == 0)
        {
            return default;
        }
        return await JsonSerializer.DeserializeAsync<TOut>(await res.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<TOut?> PutAsync<TIn, TOut>(string path, TIn body)
    {
        var json = JsonSerializer.Serialize(body);
        var res = await _http.PutAsync(path, new StringContent(json, Encoding.UTF8, "application/json"));
        res.EnsureSuccessStatusCode();
        if (res.Content.Headers.ContentLength == 0)
        {
            return default;
        }
        return await JsonSerializer.DeserializeAsync<TOut>(await res.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<TOut?> PatchAsync<TIn, TOut>(string path, TIn body)
    {
        var json = JsonSerializer.Serialize(body);
        var req = new HttpRequestMessage(new HttpMethod("PATCH"), path)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        var res = await _http.SendAsync(req);
        res.EnsureSuccessStatusCode();
        if (res.Content.Headers.ContentLength == 0)
        {
            return default;
        }
        return await JsonSerializer.DeserializeAsync<TOut>(await res.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task DeleteAsync(string path)
    {
        var res = await _http.DeleteAsync(path);
        res.EnsureSuccessStatusCode();
    }
}


