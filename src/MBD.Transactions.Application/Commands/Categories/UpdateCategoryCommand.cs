using System;
using FluentValidation;
using FluentValidation.Results;
using MBD.Transactions.Application.Commands.Common;
using MBD.Transactions.Domain.Enumerations;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;

namespace MBD.Transactions.Application.Commands.Categories
{
    public class UpdateCategoryCommand : Command<IResult>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }

        public override ValidationResult Validate()
        {
            return new UpdateCategoryCommandValidation().Validate(this);
        }

        public class UpdateCategoryCommandValidation : AbstractValidator<UpdateCategoryCommand>
        {
            public UpdateCategoryCommandValidation()
            {
                RuleFor(x => x.Id)
                    .NotEmpty();

                RuleFor(x => x.Name)
                    .NotEmpty()
                    .MaximumLength(100);

                RuleFor(x => x.Status)
                    .IsInEnum();
            }
        }
    }
}