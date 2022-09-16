using System;
using System.Collections.Generic;
using MBD.Transactions.Domain.Entities.Common;
using MBD.Transactions.Domain.Enumerations;
using MBD.Transactions.Domain.Events;
using MeuBolsoDigital.Core.Assertions;
using MeuBolsoDigital.Core.Interfaces.Entities;

namespace MBD.Transactions.Domain.Entities
{
    public class Category : BaseEntityWithEvent, IAggregateRoot
    {
        private List<Category> _subCategories = new();

        public Guid TenantId { get; private init; }
        public Guid? ParentCategoryId { get; private set; }
        public string Name { get; private set; }
        public TransactionType Type { get; private init; }
        public Status Status { get; private set; }

        public IReadOnlyList<Category> SubCategories => _subCategories.AsReadOnly();

        public Category(Guid tenantId, string name, TransactionType type)
        {
            DomainAssertions.IsNotNullOrEmpty(name, "É necessário informar um nome.");
            DomainAssertions.HasMaxLength(name, 100, "O nome deve conter no máximo 100 caracteres.");
            DomainAssertions.IsNotEmpty(tenantId, "Id de usuário inválido.");

            TenantId = tenantId;
            Name = name;
            Type = type;
            Activate();
        }

        /// <summary>
        /// Construtor para criação de subcategorias.
        /// </summary>
        private Category(Guid userId, Guid parentCategoryId, string name, TransactionType type)
            : this(userId, name, type)
        {
            DomainAssertions.IsNotEmpty(parentCategoryId, "Id de categoria pai inválido.");

            ParentCategoryId = parentCategoryId;
        }

        protected Category()
        {
        }

        public void ChangeName(string name)
        {
            DomainAssertions.IsNotNullOrEmpty(name, "É necessário informar um nome.");
            DomainAssertions.HasMaxLength(name, 100, "O nome deve conter no máximo 100 caracteres.");

            AddDomainEvent(new CategoryNameChangedDomainEvent(id: Id, newName: name, oldName: Name));
            Name = name;
        }

        public void Activate()
        {
            Status = Status.Active;
        }

        public void Deactivate()
        {
            Status = Status.Inactive;
        }

        public Category AddSubCategory(string name)
        {
            DomainAssertions.IsTrue(CanHaveSubCategories(), "Não é permitido adicionar subcategorias à uma categoria filha.");

            var subCategory = new Category(TenantId, Id, name, Type);
            _subCategories.Add(subCategory);

            return subCategory;
        }

        public bool CanHaveSubCategories()
        {
            return ParentCategoryId == null;
        }
    }
}