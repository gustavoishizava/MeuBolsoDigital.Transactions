using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MeuBolsoDigital.CrossCutting.Extensions;
using MeuBolsoDigital.IntegrationEventLog.Services;
using MeuBolsoDigital.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace MBD.Transactions.API.Workers
{
    public class IntegrationEventWorker : BackgroundService
    {
        private readonly ILogger<IntegrationEventWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRabbitMqConnection _rabbitMqConnection;
        private readonly string _publicationTopic;

        public IntegrationEventWorker(ILogger<IntegrationEventWorker> logger, IServiceProvider serviceProvider, IRabbitMqConnection rabbitMqConnection, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _rabbitMqConnection = rabbitMqConnection;
            _publicationTopic = configuration["RabbitMqConfiguration:PublicationTopic"];
        }

        [ExcludeFromCodeCoverage]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{GetType().Name} started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessIntegrationEventsAsync(stoppingToken);
                await Task.Delay(5000);
            }
        }

        protected async Task ProcessIntegrationEventsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var integrationEventLogService = scope.ServiceProvider.GetService<IIntegrationEventLogService>();

            var integrationEventLogs = await integrationEventLogService.RetrieveEventLogsPendingToPublishAsync();
            if (integrationEventLogs.IsNullOrEmpty())
                return;

            SetupExchange(cancellationToken);

            foreach (var integrationEventLogEntry in integrationEventLogs)
            {
                PublishMessage(integrationEventLogEntry.Content, integrationEventLogEntry.EventTypeName, cancellationToken);
                await integrationEventLogService.SetEventToPublishedAsync(integrationEventLogEntry);
            }
        }

        #region RabbitMQ

        private void SetupExchange(CancellationToken cancellationToken)
        {
            _rabbitMqConnection.TryConnect(cancellationToken);

            _rabbitMqConnection.Channel.ExchangeDeclare(
                exchange: _publicationTopic,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null);
        }

        private void PublishMessage(string content, string routingKey, CancellationToken cancellationToken)
        {
            _rabbitMqConnection.TryConnect(cancellationToken);

            var messageBytes = Encoding.UTF8.GetBytes(content);

            _rabbitMqConnection.Channel.BasicPublish(
                exchange: _publicationTopic,
                routingKey: routingKey,
                basicProperties: null,
                body: messageBytes);

            _logger.LogInformation("Published message.");
        }

        #endregion
    }
}