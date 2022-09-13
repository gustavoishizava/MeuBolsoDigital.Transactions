using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MBD.Core.Extensions;
using MBD.IntegrationEventLog.Services;
using MBD.MessageBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace MBD.Transactions.Application.BackgroundServices
{
    public class PublishIntegrationEventsService : BackgroundService
    {
        private readonly IMessageBus _messageBus;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PublishIntegrationEventsService> _logger;
        private const string ExchangeName = "transactions.topic";

        public PublishIntegrationEventsService(IMessageBus messageBus, IServiceProvider serviceProvider, ILogger<PublishIntegrationEventsService> logger)
        {
            _messageBus = messageBus;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço de publicação de mensagens iniciado.");

            SetupChannel();

            while (!stoppingToken.IsCancellationRequested)
            {
                await PublishEventsAsync();

                await Task.Delay(5000, stoppingToken);
            }

            _logger.LogInformation("Serviço de publicação de mensagens parando...");
        }

        private void SetupChannel()
        {
            _messageBus.TryConnect();

            string queueBankAccounts = "transactions.bank_accounts";

            string[] routingKeys = new[] { "realized_payment", "reversed_payment", "value_changed", "deleted" };

            _messageBus.Channel.ExchangeDeclare(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: false,
                autoDelete: false,
                arguments: null);

            _messageBus.Channel.QueueDeclare(
                queue: queueBankAccounts,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            for (int i = 0; i < routingKeys.Length; i++)
            {
                _messageBus.Channel.QueueBind(
                    queue: queueBankAccounts,
                    exchange: ExchangeName,
                    routingKey: routingKeys[i],
                    arguments: null);
            }
        }

        private async Task PublishEventsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var integrationEventLogService = scope.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();

            var events = await integrationEventLogService.RetrieveEventLogsPendingToPublishAsync();
            if (events.IsNullOrEmpty())
                return;

            foreach (var @event in events)
            {
                try
                {
                    var message = JsonSerializer.Deserialize<object>(@event.Content);
                    if (message is null)
                        continue;

                    _messageBus.Publish(message, @event.EventTypeName, ExchangeName);

                    await integrationEventLogService.RemoveEventAsync(@event);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}