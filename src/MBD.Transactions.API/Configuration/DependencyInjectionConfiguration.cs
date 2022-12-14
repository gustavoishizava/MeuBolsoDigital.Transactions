using System.Collections.Generic;
using System.Reflection;
using MBD.Transactions.Application.Response;
using MBD.Transactions.Domain.Events;
using MBD.Transactions.Domain.Interfaces.Repositories;
using MBD.Transactions.Infrastructure;
using MBD.Transactions.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MeuBolsoDigital.Core.Interfaces.Identity;
using MBD.Transactions.API.Identity;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;
using System.Diagnostics.CodeAnalysis;
using MeuBolsoDigital.RabbitMQ.Extensions;
using MBD.Transactions.API.Workers;
using MBD.Transactions.Application.IntegrationEvents.Consumed.BankAccounts.Created;
using MBD.Transactions.Application.IntegrationEvents.Consumed.BankAccounts.DescriptionChanged;
using MBD.Transactions.Application.DomainEventHandlers.Transactions;
using MBD.Transactions.Application.DomainEventHandlers.Categories;
using MBD.Transactions.Application.Commands.Categories.Create;
using MBD.Transactions.Application.Commands.Categories.Update;
using MBD.Transactions.Application.Commands.Categories.Delete;
using MBD.Transactions.Application.Commands.Transactions.Create;
using MBD.Transactions.Application.Commands.Transactions.Update;
using MBD.Transactions.Application.Commands.Transactions.Delete;
using MBD.Transactions.Application.Queries.Categories.GetById;
using MBD.Transactions.Application.Queries.Categories.GetAllByType;
using MBD.Transactions.Application.Queries.Categories.GetAll;
using MBD.Transactions.Application.Queries.Transactions.GetById;
using MBD.Transactions.Application.Queries.Transactions.GetAll;
using MeuBolsoDigital.IntegrationEventLog.Extensions;

namespace MBD.Transactions.API.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjectionConfiguration
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddRepositories()
                .AddCommands()
                .AddQueries()
                .AddDomainEvents()
                .AddIntegrationEvents()
                .AddConsumers()
                .AddRabbitMqConnection(configuration)
                .AddIntegrationEventLog<IntegrationEventLogRepository>()
                .AddOutBoxTransaction();

            services.AddHttpContextAccessor();
            services.AddScoped<ILoggedUser, WebAppUser>();
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
            services.AddScoped<IRequestHandler<GetTransactionByIdQuery, IResult<TransactionResponse>>, GetTransactionByIdQueryHandler>();
            services.AddScoped<IRequestHandler<GetAllTransactionsQuery, IEnumerable<TransactionResponse>>, GetAllTransactionsQueryHandler>();

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

        private static IServiceCollection AddConsumers(this IServiceCollection services)
        {
            services.AddHostedService<RabbitMqWorker>();

            return services;
        }

        private static IServiceCollection AddOutBoxTransaction(this IServiceCollection services)
        {
            services.AddHostedService<IntegrationEventWorker>();

            return services;
        }
    }
}