using System.Collections.Generic;
using System.Reflection;
using MBD.Application.Core.Response;
using MBD.Core.Data;
using MBD.Core.Identity;
using MBD.IntegrationEventLog.Services;
using MBD.MessageBus;
using MBD.Transactions.API.Configuration.HttpClient;
using MBD.Transactions.Application.BackgroundServices;
using MBD.Transactions.Application.DomainEventHandlers;
using MBD.Transactions.Application.IntegrationEvents.EventHandling;
using MBD.Transactions.Application.IntegrationEvents.Events;
using MBD.Transactions.Application.Queries.Transactions.Handlers;
using MBD.Transactions.Application.Queries.Transactions.Queries;
using MBD.Transactions.Application.Response;
using MBD.Transactions.Application.Response.Models;
using MBD.Transactions.Domain.Events;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MBD.Transactions.Infrastructure;
using MBD.Transactions.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MBD.Transactions.Application.Queries.Categories.Queries;
using MBD.Transactions.Application.Queries.Categories.Handlers;
using MBD.Transactions.Application.Commands.Transactions;
using MBD.Transactions.Application.Commands.Categories;

namespace MBD.Transactions.API.Configuration
{
    public static class DependencyInjectionConfiguration
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddRepositories()
                .AddHttpClients(configuration)
                .AddCommands()
                .AddQueries()
                .AddDomainEvents()
                .AddIntegrationEvents()
                .AddConfigurations(configuration)
                .AddMessageBus()
                .AddConsumers()
                .AddIntegrationEventLogsService()
                .AddOutBoxTransaction();

            services.AddHttpContextAccessor();
            services.AddScoped<IAspNetUser, AspNetUser>();
            services.AddAutoMapper(Assembly.Load("MBD.Transactions.Application"));

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        private static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            return services;
        }

        private static IServiceCollection AddCommands(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetCallingAssembly());

            services.AddScoped<IRequestHandler<CreateTransactionCommand, IResult<TransactionResponse>>, CreateTransactionCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateTransactionCommand, IResult>, UpdateTransactionCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteTransactionCommand, IResult>, DeleteTransactionCommandHandler>();

            services.AddScoped<IRequestHandler<CreateCategoryCommand, IResult<CategoryResponse>>, CreateCategoryCommandHandler>();
            services.AddScoped<IRequestHandler<UpdateCategoryCommand, IResult>, UpdateCategoryCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteCategoryCommand, IResult>, DeleteCategoryCommandHandler>();

            return services;
        }

        private static IServiceCollection AddQueries(this IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<GetTransactionByIdQuery, IResult<TransactionModel>>, GetTransactionByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetAllTransactionsQuery, IEnumerable<TransactionModel>>, GetAllTransactionsQueryHandler>();

            services.AddScoped<IRequestHandler<GetCategoryByIdQuery, IResult<CategoryResponse>>, GetCategoryByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetAllCategoriesQuery, CategoryByTypeResponse>, GetAllCategoriesQueryHandler>();
            services.AddScoped<IRequestHandler<GetAllCategoriesByTypeQuery, IEnumerable<CategoryWithSubCategoriesResponse>>, GetAllCategoriesByTypeQueryHandler>();

            return services;
        }

        private static IServiceCollection AddDomainEvents(this IServiceCollection services)
        {
            services.AddScoped<INotificationHandler<TransactionCreatedDomainEvent>, TransactionCreatedDomainEventHandler>();
            services.AddScoped<INotificationHandler<TransactionUpdatedDomainEvent>, TransactionUpdatedDomainEventHandler>();
            services.AddScoped<INotificationHandler<TransactionDeletedDomainEvent>, TransactionDeletedDomainEventHandler>();
            services.AddScoped<INotificationHandler<RealizedPaymentDomainEvent>, RealizedPaymentDomainEventHandler>();
            services.AddScoped<INotificationHandler<ReversedPaymentDomainEvent>, ReversedPaymentDomainEventHandler>();
            services.AddScoped<INotificationHandler<ValueChangedDomainEvent>, ValueChangedDomainEventHandler>();

            services.AddScoped<INotificationHandler<CategoryNameChangedDomainEvent>, CategoryNameChangedDomainEventHandler>();

            return services;
        }

        private static IServiceCollection AddIntegrationEvents(this IServiceCollection services)
        {
            services.AddScoped<INotificationHandler<BankAccountCreatedIntegrationEvent>, BankAccountCreatedIntegrationEventHandler>();
            services.AddScoped<INotificationHandler<BankAccountDescriptionChangedIntegrationEvent>, BankAccountDescriptionChangedIntegrationEventHandler>();

            return services;
        }

        private static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqConfiguration>(configuration.GetSection(nameof(RabbitMqConfiguration)));

            return services;
        }

        private static IServiceCollection AddMessageBus(this IServiceCollection services)
        {
            services.AddSingleton<IMessageBus, MBD.MessageBus.MessageBus>();

            return services;
        }

        private static IServiceCollection AddConsumers(this IServiceCollection services)
        {
            services.AddHostedService<BankAccountConsumerService>();

            return services;
        }

        private static IServiceCollection AddIntegrationEventLogsService(this IServiceCollection services)
        {
            services.AddScoped<IIntegrationEventLogService, IntegrationEventLogService>();

            return services;
        }

        private static IServiceCollection AddOutBoxTransaction(this IServiceCollection services)
        {
            services.AddHostedService<PublishIntegrationEventsService>();

            return services;
        }
    }
}