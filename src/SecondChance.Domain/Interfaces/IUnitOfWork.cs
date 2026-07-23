using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void ForceEntryAdded<TEntity>(TEntity entity) where TEntity : class;
    }
}
