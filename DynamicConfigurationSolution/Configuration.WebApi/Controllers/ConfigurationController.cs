using ConfigurationLibrary;
using ConfigurationLibrary.DBConnection;
using ConfigurationLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace Configuration.WebApi.Controllers;

/// <summary>
/// Konfigürasyon verilerini dış dünyaya açan API controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly string _applicationName;
    private readonly string _connectionString;
    private readonly int _refreshIntervalMs;

    private readonly ConfigurationReader _configurationReader;

    /// <summary>
    /// Controller'a bağımlı ConfigurationReader örneği enjekte edilir.
    /// </summary>
    public ConfigurationController()
    {
        // Bu değerler doğrudan constructor'dan girildi; production için appsettings tercih edilir.
        _applicationName = "SERVICE-A";
        _connectionString = "Server=.;Database=ConfigurationDb;Trusted_Connection=True;TrustServerCertificate=True;";
        _refreshIntervalMs = 30000;

        _configurationReader = new ConfigurationReader(_applicationName, _connectionString, _refreshIntervalMs);
    }

    /// <summary>
    /// Verilen key'e göre konfigürasyon değeri döner.
    /// </summary>
    /// <param name="key">Konfigürasyon anahtarı (örnek: SiteName)</param>
    /// <returns>Konfigürasyon değeri string olarak</returns>
    [HttpGet("{key}")]
    public IActionResult GetValue(string key)
    {
        try
        {
            var value = _configurationReader.GetValue<string>(key);
            return Ok(value);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Anahtar bulunamadı: {key}");
        }
        catch (InvalidCastException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// İlgili uygulamaya ait tüm konfigürasyon kayıtlarını döner.
    /// </summary>
    [HttpGet("all")]
    public IActionResult GetAll()
    {
        using var context = new ConfigurationDbContext(_connectionString);

        var list = context.ConfigurationRecords
            .Where(c => c.ApplicationName == _applicationName && c.IsActive)
            .ToList();

        return Ok(list);
    }

    [HttpGet("item/{id}")]
    public IActionResult GetById(int id)
    {
        using var context = new ConfigurationDbContext(_connectionString);
        var item = context.ConfigurationRecords.FirstOrDefault(c => c.Id == id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public IActionResult Create(ConfigurationItem item)
    {
        using var context = new ConfigurationDbContext(_connectionString);
        item.LastUpdated = DateTime.Now;
        context.ConfigurationRecords.Add(item);
        context.SaveChanges();
        return Ok();
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, ConfigurationItem item)
    {
        using var context = new ConfigurationDbContext(_connectionString);
        var existing = context.ConfigurationRecords.FirstOrDefault(c => c.Id == id);
        if (existing is null) return NotFound();

        existing.Name = item.Name;
        existing.Type = item.Type;
        existing.Value = item.Value;
        existing.IsActive = item.IsActive;
        existing.ApplicationName = item.ApplicationName;
        existing.LastUpdated = DateTime.Now;

        context.SaveChanges();
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        using var context = new ConfigurationDbContext(_connectionString);
        var item = context.ConfigurationRecords.FirstOrDefault(c => c.Id == id);
        if (item is null) return NotFound();

        context.ConfigurationRecords.Remove(item);
        context.SaveChanges();
        return Ok();
    }



}
