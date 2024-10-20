namespace TeamRadio.Models;

public class UsernameRequest
{
    public string username { get; set; } = string.Empty;
}
public class VideoRequest : UsernameRequest
{
    public Video videoItem { get; set; }
}
public class PlayNextRequest : VideoRequest
{
    public bool isAutoNext { get; set; }
}
public class VideoListRequest : UsernameRequest
{
    public List<Video> videoList { get; set; }
}
public class MessageRequest : UsernameRequest
{
    public string message { get; set; } = string.Empty;
}

public class Video
{
    public string videoId { get; set; }
    public string title { get; set; }
}

public class SendStartingSecondRequest
{
    public int startingSecond { get; set; }
}