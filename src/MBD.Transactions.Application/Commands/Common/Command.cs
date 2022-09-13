using FluentValidation.Results;
using MediatR;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;

namespace MBD.Transactions.Application.Commands.Common
{
    public abstract class Command<TCommandResult> : IRequest<TCommandResult> where TCommandResult : IResult
    {
        public abstract ValidationResult Validate();
    }
}