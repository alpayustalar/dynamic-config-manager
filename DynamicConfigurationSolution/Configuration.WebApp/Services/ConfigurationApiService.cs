using System.Net.Http.Json;
using Configuration.WebApp.Models;

namespace Configuration.WebApp.Services;

/// <summary>
/// API ile iletişim kurarak konfigürasyon verilerini yöneten servis.
/// </summary>
public class ConfigurationApiService
{
    private readonly HttpClient _httpClient;

    public ConfigurationApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:7137/api/");
    }

    /// <summary>
    /// Belirtilen key'e ait değeri getirir.
    /// </summary>
    public async Task<string?> GetValueAsync(string key)
    {
        return await _httpClient.GetFromJsonAsync<string>($"configuration/{key}");
    }

    /// <summary>
    /// Tüm konfigürasyon kayıtlarını getirir. (Henüz hazır değil ama WebAPI'ye ekleyeceğiz)
    /// </summary>
    public async Task<List<ConfigurationItem>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<ConfigurationItem>>("configuration/all") ?? new();
    }

    public async Task<bool> AddAsync(ConfigurationItem item)
    {
        var response = await _httpClient.PostAsJsonAsync("configuration", item);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(ConfigurationItem item)
    {
        var response = await _httpClient.PutAsJsonAsync($"configuration/{item.Id}", item);
        return response.IsSuccessStatusCode;
    }

    public async Task<ConfigurationItem?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<ConfigurationItem>($"configuration/item/{id}");
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"configuration/{id}");
        return response.IsSuccessStatusCode;
    }


}
