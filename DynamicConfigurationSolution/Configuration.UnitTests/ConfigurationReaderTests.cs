using ConfigurationLibrary;
using ConfigurationLibrary.Models;
using Microsoft.Extensions.Caching.Memory;
using Xunit;
using System;

namespace Configuration.UnitTests;

public class ConfigurationReaderTests
{
    private readonly ConfigurationReader _reader;

    public ConfigurationReaderTests()
    {
        // Test için ConfigurationReader yarat
        // Ama gerçek baðlantý yerine test sonrasý manuel cache set eder
        _reader = new ConfigurationReader("TEST-SERVICE", "fake-connection", 60000);

        // Test cache'e örnek veri atar
        var cacheField = typeof(ConfigurationReader)
            .GetField("_cache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var cache = (MemoryCache?)cacheField?.GetValue(_reader);

        cache?.Set("TestKeyString", new ConfigurationItem
        {
            Name = "TestKeyString",
            Type = "string",
            Value = "test-value",
            IsActive = true,
            ApplicationName = "TEST-SERVICE"
        });

        cache?.Set("TestKeyInt", new ConfigurationItem
        {
            Name = "TestKeyInt",
            Type = "int",
            Value = "42",
            IsActive = true,
            ApplicationName = "TEST-SERVICE"
        });
    }

    [Fact]
    public void GetValue_String_ReturnsCorrectValue()
    {
        var result = _reader.GetValue<string>("TestKeyString");
        Assert.Equal("test-value", result);
    }

    [Fact]
    public void GetValue_Int_ReturnsCorrectValue()
    {
        var result = _reader.GetValue<int>("TestKeyInt");
        Assert.Equal(42, result);
    }

    [Fact]
    public void GetValue_InvalidKey_ThrowsKeyNotFoundException()
    {
        Assert.Throws<KeyNotFoundException>(() => _reader.GetValue<string>("NonExistingKey"));
    }

    [Fact]
    public void GetValue_InvalidCast_ThrowsInvalidCastException()
    {
        Assert.Throws<InvalidCastException>(() => _reader.GetValue<DateTime>("TestKeyString"));
    }
}
