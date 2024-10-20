using Microsoft.Extensions.Caching.Memory;
using TeamRadio.Models;

namespace TeamRadio.Services;

using Microsoft.AspNetCore.SignalR;

public class ChatHub(IMemoryCache cache) : Hub
{
    const string VideoListCacheKey = "VideoList";
    const string StartingSecondCacheKey = "StartingSecond";
    const string OriginUserConnectionIdCacheKey = "OriginUserConnectionId";
    private static HashSet<User> _connectedUsers = new();

    public override async Task OnConnectedAsync()
    {
        var originUserConnectionId = cache.Get<string>(OriginUserConnectionIdCacheKey);
        if (originUserConnectionId == null)
        {
            originUserConnectionId = Context.ConnectionId;
            cache.Set(OriginUserConnectionIdCacheKey, originUserConnectionId);
            await Clients.Client(originUserConnectionId).SendAsync("AmIOrigin");
        }
        _connectedUsers.Add(new User { ConnectionId = Context.ConnectionId });
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _connectedUsers.RemoveWhere(x => x.ConnectionId == Context.ConnectionId);
        var originUserConnectionId = cache.Get<string>(OriginUserConnectionIdCacheKey);
        if (Context.ConnectionId == originUserConnectionId)
        {
            cache.Remove(OriginUserConnectionIdCacheKey);
            cache.Remove(StartingSecondCacheKey);
            if (_connectedUsers.Count > 0)
            {
                var newConnectionId = _connectedUsers.First().ConnectionId;
                cache.Set(OriginUserConnectionIdCacheKey, newConnectionId);
                await Clients.Client(newConnectionId).SendAsync("AmIOrigin");
            }
        }
        
        
        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task SendVideoToAll(VideoRequest request)
    {
        var videoList = cache.Get<List<Video>>(VideoListCacheKey) ?? new List<Video>();
        // if video is already in the list do nothing
        if (videoList.Any(x => x.videoId == request.videoItem.videoId))
        {
            await Clients.All.SendAsync("ReceiveMessage", new MessageRequest
            {
                username = "System",
                message = "Video already in the list."
            });
            return;
        }
        videoList.Add(request.videoItem);
        cache.Set(VideoListCacheKey, videoList);
        await Clients.All.SendAsync("ReceiveVideoId", request);
    }

    public async Task SendVideoListToAll(VideoListRequest request)
    {
        var videoList = cache.Get<List<Video>>(VideoListCacheKey) ?? new List<Video>();
        // if video is already in the list do nothing
        foreach (var video in request.videoList)
        {
            if (videoList.Any(x => x.videoId == video.videoId))
            {
                await Clients.All.SendAsync("ReceiveMessage", new MessageRequest
                {
                    username = "System",
                    message = "Video already in the list."
                });
                continue;
            }
            videoList.Add(video);
        }
        cache.Set(VideoListCacheKey, videoList);
        foreach (var video in request.videoList)
        {
            await Clients.All.SendAsync("ReceiveVideoId", new VideoRequest
            {
                username = request.username,
                videoItem = video
            });
        }
    }

    public async Task PlayNextToAll(PlayNextRequest request)
    {
        var originUserConnectionId = cache.Get<string>(OriginUserConnectionIdCacheKey);
        if (request.isAutoNext && originUserConnectionId != Context.ConnectionId)
        {
            return;
        }
        var videoList = cache.Get<List<Video>>(VideoListCacheKey) ?? new List<Video>();
        var videoIndex = videoList.FindIndex(x => x.videoId == request.videoItem.videoId);
        // if video is last in the list do nothing
        if (videoIndex == videoList.Count - 1)
        {
            await Clients.All.SendAsync("ReceiveMessage", new MessageRequest
            {
                username = "System",
                message = "Last video in the list. Please add more videos."
            });
            await DeleteAllToAll(new UsernameRequest { username = "System"});
            return;
        }
        if (videoIndex != -1)
        {
            //remove until the current video
            videoList.RemoveRange(0, videoIndex + 1);
        }
        cache.Set(VideoListCacheKey, videoList);
        cache.Remove(StartingSecondCacheKey);
        var nextVideo = videoList.FirstOrDefault();
        await Clients.All.SendAsync("PlayNext", new VideoRequest
        {
            username = request.username,
            videoItem = nextVideo!
        });
    }
    
    public async Task DeleteAllToAll(UsernameRequest request)
    {
        cache.Remove(VideoListCacheKey);
        cache.Remove(StartingSecondCacheKey);
        await Clients.All.SendAsync("DeleteAll", request);
    }
    public async Task SendMessageToAll(MessageRequest request)
    {
        await Clients.All.SendAsync("ReceiveMessage", request);
    }
    public async Task SendStartingSecond(SendStartingSecondRequest request)
    {
        var originUserConnectionId = cache.Get<string>(OriginUserConnectionIdCacheKey);
        if (originUserConnectionId != Context.ConnectionId)
        {
            return;
        }

        if (request.startingSecond < 0)
        {
            return;
        }
        cache.Set(StartingSecondCacheKey, request.startingSecond);
    }
}