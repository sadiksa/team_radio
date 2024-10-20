using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TeamRadio.Models;
using TeamRadio.Services;

namespace TeamRadio.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    const string VideoListCacheKey = "VideoList";
    const string StartingSecondCacheKey = "StartingSecond";
    private readonly IMemoryCache _cache;

    public HomeController(ILogger<HomeController> logger, IMemoryCache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public IActionResult Index()
    {
        var videoList = _cache.Get<List<Video>>(VideoListCacheKey) ?? new List<Video>();
        var startingSecond = _cache.Get<int>(StartingSecondCacheKey);
        return View((videoList, startingSecond));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}