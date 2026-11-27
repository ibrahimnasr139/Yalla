using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(AppDbContext dbContext)
        {
            _context = dbContext;
        }
        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction is null)
            {
                _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            }
        }
        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction is null) return;

            await SaveAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction is null) return;

            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
        public async Task SaveAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}