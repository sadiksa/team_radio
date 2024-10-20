# TeamRadio

TeamRadio is a web application that allows users to share and control a playlist of YouTube videos in real-time. The application uses SignalR for real-time communication and the YouTube IFrame API for video playback.

### Docker Hub Page
[https://hub.docker.com/repository/docker/sadiksa/teamradio](https://hub.docker.com/repository/docker/sadiksa/teamradio)

## Features

- **Real-time Video Sharing**: Users can add videos to a shared playlist.
- **Video Control**: Play, pause, mute, and unmute videos.
- **Video Synchronization**: Ensure all users are watching the same video at the same time.
- **Automatic Video Progress Tracking**: Track and synchronize the current playback time of the video.
- **Background Services**: Use background services to manage video playback and synchronization.

## Technologies Used

- **Frontend**: Vue.js
- **Backend**: ASP.NET Core with SignalR
- **YouTube API**: For fetching and controlling YouTube videos
- **Docker**: For containerization
- **MemoryCache**: For caching data

## Getting Started

### Prerequisites

- Docker
- .NET 8.0 SDK

### Installation

1. **Clone the repository**:
    ```sh
    git clone https://github.com/yourusername/TeamRadio.git
    cd TeamRadio
    ```

2. **Build and run the Docker container**:
    ```sh
    docker build -t teamradio .
    docker run -e YOUTUBE_API_KEY=your_youtube_api_key \
               -e PLAYER_COLOR=#1f5ea780 \
               -e CHAT_COLOR=#80217d80 \
               -e LOGO_URL=https://www.ist-pay.com/assets/contents/logo-istpay.svg \
               -e COMPANY_NAME=Ist-Pay \
               -p 5100:5100 teamradio
    ```


## Contributing

1. Fork the repository.
2. Create a new branch (`git checkout -b feature-branch`).
3. Make your changes.
4. Commit your changes (`git commit -am 'Add new feature'`).
5. Push to the branch (`git push origin feature-branch`).
6. Create a new Pull Request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
