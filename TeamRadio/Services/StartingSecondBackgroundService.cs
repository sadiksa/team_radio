using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace TeamRadio.Services;

public class StartingSecondBackgroundService : BackgroundService
{
    private readonly ILogger<StartingSecondBackgroundService> _logger;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IMemoryCache _cache;
    const string OriginUserConnectionIdCacheKey = "OriginUserConnectionId";

    public StartingSecondBackgroundService(ILogger<StartingSecondBackgroundService> logger,
        IHubContext<ChatHub> hubContext, IMemoryCache cache)
    {
        _logger = logger;
        _hubContext = hubContext;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Checking current second at: {time}", DateTimeOffset.Now);
            var originUserConnectionId = _cache.Get<string>(OriginUserConnectionIdCacheKey);
            if (originUserConnectionId != null)
            {
                await _hubContext.Clients.Client(originUserConnectionId).SendAsync("RequestCurrentSecond", cancellationToken: stoppingToken);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}