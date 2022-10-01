using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MBD.Transactions.Application.IntegrationEvents.Consumed.BankAccounts.Created;
using MBD.Transactions.Application.IntegrationEvents.Consumed.BankAccounts.DescriptionChanged;
using MediatR;
using MeuBolsoDigital.CrossCutting.Extensions;
using MeuBolsoDigital.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;

namespace MBD.Transactions.API.Workers
{
    [ExcludeFromCodeCoverage]
    public class RabbitMqWorker : BackgroundService
    {
        private readonly ILogger<RabbitMqWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRabbitMqConnection _rabbitMqConnection;

        private readonly string _queueName;
        private readonly string _bankAccountTopic;

        public RabbitMqWorker(ILogger<RabbitMqWorker> logger, IServiceProvider serviceProvider, IRabbitMqConnection rabbitMqConnection, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _rabbitMqConnection = rabbitMqConnection;

            _queueName = configuration["RabbitMqConfiguration:ConsumerQueue"];
            _bankAccountTopic = configuration["RabbitMqConfiguration:BankAccountTopic"];
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{GetType().Name} started.");

            _rabbitMqConnection.TryConnect(stoppingToken);
            SetupQueue();

            var consumer = new EventingBasicConsumer(_rabbitMqConnection.Channel);
            consumer.Received += async (object sender, BasicDeliverEventArgs args) =>
            {
                if (await ProcessMessageAsync(args))
                    _rabbitMqConnection.Channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
                else
                    _rabbitMqConnection.Channel.BasicNack(deliveryTag: args.DeliveryTag, multiple: false, requeue: true);
            };

            _rabbitMqConnection.Channel.BasicConsume(queue: _queueName,
                                                     autoAck: false,
                                                     consumerTag: string.Empty,
                                                     noLocal: true,
                                                     exclusive: false,
                                                     arguments: null,
                                                     consumer: consumer);

            return Task.CompletedTask;
        }

        private void SetupQueue()
        {
            _rabbitMqConnection.Channel.QueueDeclare(queue: _queueName,
                                                     durable: true,
                                                     exclusive: false,
                                                     autoDelete: false,
                                                     arguments: null);

            _rabbitMqConnection.Channel.QueueBind(queue: _queueName,
                                                  exchange: _bankAccountTopic,
                                                  routingKey: "#",
                                                  arguments: null);
        }

        private async Task<bool> ProcessMessageAsync(BasicDeliverEventArgs args)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetService<IMediator>();

                var filter = $"{args.Exchange}.{args.RoutingKey}";

                switch (filter)
                {
                    case RabbitMqConstants.BANK_ACCOUNT_CREATED:
                        await mediator.Publish(args.Body.Deserialize<BankAccountCreatedIntegrationEvent>());
                        return true;

                    case RabbitMqConstants.BANK_ACCOUNT_DESCRIPTION_CHANGED:
                        await mediator.Publish(args.Body.Deserialize<BankAccountDescriptionChangedIntegrationEvent>());
                        return true;

                    default:
                        return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{GetType().Name} error.");
                return false;
            }
        }
    }
}