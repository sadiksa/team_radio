using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using TeamRadio.Models;
using TeamRadio.Services;

namespace TeamRadio.Controllers;

public class QueueController(ILogger<QueueController> logger, RabbitMQService rabbitMqService) : Controller
{
    private readonly ILogger<QueueController> _logger = logger;
    private readonly RabbitMQService _rabbitMQService = rabbitMqService;

    public IActionResult Index()
    {
        return View();
    }
    
    public async Task<IActionResult> SendMessages()
    {
        await _rabbitMQService.PublishMessageAsync("Hello, World!");
        await _rabbitMQService.PublishMessageAsync("Hello, RabbitMQ!");
        return RedirectToAction("Index");
    }
}