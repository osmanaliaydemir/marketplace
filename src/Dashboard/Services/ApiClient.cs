using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Dashboard.Services;

public sealed class ApiClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _cfg;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiClient(HttpClient http, IConfiguration cfg, IHttpContextAccessor httpContextAccessor)
    {
        _http = http;
        _cfg = cfg;
        _httpContextAccessor = httpContextAccessor;
        
        // BaseAddress zaten Program.cs'de set ediliyor
        // _http.BaseAddress = new Uri(_cfg["ApiBaseUrl"]!);
    }

    private void EnsureAuthHeader()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["API_TOKEN"];
        if (!string.IsNullOrWhiteSpace(token))
        {
            if (_http.DefaultRequestHeaders.Authorization == null || _http.DefaultRequestHeaders.Authorization.Parameter != token)
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }

    public void SetBearer(string token)
    {
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<T?> GetAsync<T>(string path)
    {
        EnsureAuthHeader();
        var res = await _http.GetAsync(path);
        res.EnsureSuccessStatusCode();
        return await JsonSerializer.DeserializeAsync<T>(await res.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<TOut?> PostAsync<TIn, TOut>(string path, TIn body)
    {
        EnsureAuthHeader();
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
        EnsureAuthHeader();
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
        EnsureAuthHeader();
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
        EnsureAuthHeader();
        var res = await _http.DeleteAsync(path);
        res.EnsureSuccessStatusCode();
    }
}


