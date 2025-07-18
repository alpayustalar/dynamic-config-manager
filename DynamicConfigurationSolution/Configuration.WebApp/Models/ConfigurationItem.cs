﻿namespace Configuration.WebApp.Models;

/// <summary>
/// Konfigürasyon UI'sinde kullanılan model sınıfıdır.
/// </summary>
public class ConfigurationItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string ApplicationName { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}
