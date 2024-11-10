using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client.Events;

namespace TeamRadio.Services;

using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

public class RabbitMQService(IConfiguration configuration)
{
    public async Task PublishMessageAsync(string message)
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
        var props = new BasicProperties
        {
            Headers = new Dictionary<string, object>()!
        };
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync(exchange: "exc1", routingKey: "hello", mandatory: true, basicProperties:props, body:body);
    }
}
