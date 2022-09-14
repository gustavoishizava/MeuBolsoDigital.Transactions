using System.Collections.Generic;
using System.Reflection;
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
using MeuBolsoDigital.Core.Interfaces.Identity;
using MBD.Transactions.API.Identity;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;
using System.Diagnostics.CodeAnalysis;

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

        private static IServiceCollection AddConsumers(this IServiceCollection services)
        {
            return services;
        }

        private static IServiceCollection AddOutBoxTransaction(this IServiceCollection services)
        {
            return services;
        }
    }
}