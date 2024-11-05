using ChatRoom.Domain.Models;
using ChatRoom.Domain.Models.Dtos;
using ChatRoom.Domain.Services;
using ChatRoom.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace ChatRoom.Messages.RealTimeFanout.Services;

internal sealed class RealTimeFanoutConsumer : QueueConsumer, ISubscriber, IDisposable
{
    private readonly ILogger<RealTimeFanoutConsumer> _logger;
    private readonly ITokenProviderApiClient _tokenProviderApiClient;
    private readonly ClientWebSocket _clientWebSocket;
    private readonly ConsumerUser _consumerUser;
    private TokenResponse? _tokenResponse;

    public RealTimeFanoutConsumer(
        IConnectionFactory connectionFactory,
        ILogger<QueueConsumer> logger,
        ILogger<RealTimeFanoutConsumer> innerLogger,
        IConfiguration configuration,
        IOptions<ConsumerUser> options,
        ITokenProviderApiClient tokenProviderApiClient) : base(connectionFactory, logger)
    {
        _logger = innerLogger;
        _consumerUser = options.Value;
        var apiServiceHost = new Uri(configuration!["services:apiservice:https:0"]!);
        _clientWebSocket = ClientWebSocket.Create(new Uri($"wss://{apiServiceHost.Authority}/ws"));
        _tokenProviderApiClient = tokenProviderApiClient;
    }

    protected override string QueueName => "chat.realtimefanout.queue";

    protected override async Task HandleMessageAsync(string message)
    {
        var token = await GetAuthenticationToken();

        await _clientWebSocket.ConnectAsync(token.AccessToken!);
        await _clientWebSocket.SendAsync(message);

        _logger.LogInformation("Message successfully sent");
    }

    private async Task<TokenResponse> GetAuthenticationToken()
    {
        _tokenResponse ??= await _tokenProviderApiClient.LoginAsync(_consumerUser.Email!, _consumerUser.Password!);
        if (!_tokenResponse.IsTokenValid)
        {
            _tokenResponse = await _tokenProviderApiClient.RefreshTokenAsync(_tokenResponse);
        }

        return _tokenResponse;
    }
}
