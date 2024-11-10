using System.Text;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TeamRadio.Models;

namespace TeamRadio.Services;

public class MessageConsumerBackgroundService(IConfiguration configuration, IHubContext<ChatHub> hubContext) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var hostname = configuration["RabbitMQ:HostName"];
        if (hostname is null)
        {
            throw new ArgumentNullException(nameof(hostname), "RabbitMQ:HostName is required");
        }
        var port = configuration["RabbitMQ:Port"];
        if (port is null)
        {
            throw new ArgumentNullException(nameof(port), "RabbitMQ:Port is required");
        }
        var username = configuration["RabbitMQ:UserName"];
        if (username is null)
        {
            throw new ArgumentNullException(nameof(username), "RabbitMQ:UserName is required");
        }
        var password = configuration["RabbitMQ:Password"];
        if (password is null)
        {
            throw new ArgumentNullException(nameof(password), "RabbitMQ:Password is required");
        }
        var factory = new ConnectionFactory()
        {
            HostName = hostname,
            Port = Convert.ToInt32(port),
            UserName = username,
            Password = password
        };
        
        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();
        await channel.ExchangeDeclareAsync("exc1", ExchangeType.Direct);
        await channel.QueueDeclareAsync("messages", false, false, false, null);
        await channel.QueueBindAsync("messages", "exc1", "hello");
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            await hubContext.Clients.All.SendAsync("ReceiveMessage", new MessageRequest
            {
                username = "System",
                message = message
            });
        };
        await channel.BasicConsumeAsync("messages", true, consumer, cancellationToken: stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}