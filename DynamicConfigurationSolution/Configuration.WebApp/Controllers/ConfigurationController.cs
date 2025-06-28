using Configuration.WebApp.Models;
using Configuration.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace Configuration.WebApp.Controllers;

/// <summary>
/// Konfigürasyonların UI üzerinden yönetimini sağlayan controller.
/// </summary>
public class ConfigurationController : Controller
{
    private readonly ConfigurationApiService _apiService;

    public ConfigurationController(ConfigurationApiService apiService)
    {
        _apiService = apiService;
    }

    /// <summary>
    /// Tüm konfigürasyonları listeleyen ekran.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var configurations = await _apiService.GetAllAsync();
        return View(configurations);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ConfigurationItem());
    }

    [HttpPost]
    public async Task<IActionResult> Create(ConfigurationItem item)
    {
        var result = await _apiService.AddAsync(item);
        if (result)
        {
            TempData["Success"] = "Kayıt başarıyla eklendi.";
            return RedirectToAction("Index");
        }

        TempData["Error"] = "Kayıt eklenemedi.";
        return View(item);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _apiService.GetByIdAsync(id);
        if (item == null)
            return NotFound();

        return View(item);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ConfigurationItem item)
    {
        var result = await _apiService.UpdateAsync(item);
        if (result)
        {
            TempData["Success"] = "Kayıt başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        TempData["Error"] = "Güncelleme başarısız.";
        return View(item);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _apiService.DeleteAsync(id);
        if (result)
        {
            TempData["Success"] = "Kayıt silindi.";
        }
        else
        {
            TempData["Error"] = "Silme işlemi başarısız.";
        }

        return RedirectToAction("Index");
    }


}
