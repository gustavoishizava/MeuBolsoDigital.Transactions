using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MBD.Transactions.Infrastructure.Context;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.Transactions.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TransactionContext _context;

        public UnitOfWork(TransactionContext context)
        {
            _context = context;
            _context.StartTransaction();
        }

        public async Task<bool> CommitAsync()
        {
            await _context.CommitAsync();
            return true;
        }
    }
}