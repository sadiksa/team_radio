using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using TeamRadio.Models;
using TeamRadio.Services;

namespace TeamRadio.Controllers;

public class CacheController(ILogger<CacheController> logger, IDistributedCache cache) : Controller
{
    private readonly ILogger<CacheController> _logger = logger;
    private const string CacheKey = "Enabled";

    public async Task<IActionResult> Index()
    {
        var value = await cache.GetStringAsync(CacheKey) ?? "false";
        return View(bool.Parse(value));
    }
    
    public IActionResult Toggle()
    {
        var value = cache.Get(CacheKey);
        var newValue = value == null || !BitConverter.ToBoolean(value, 0);
        cache.Set(CacheKey, Encoding.UTF8.GetBytes(newValue.ToString().ToLower()));
        return RedirectToAction("Index");
    }
}