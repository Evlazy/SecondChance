using SecondChance.Domain.Interfaces;
using SecondChance.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
            {
                var failedEntry = ex.Entries.FirstOrDefault();
                if (failedEntry != null)
                {
                    var entityName = failedEntry.Entity.GetType().Name;
                    var entityState = failedEntry.State;

                    throw new Exception($"🚨 【后台雷达报告】真正的大内鬼在此！报错的表/实体是: {entityName}，它在追踪器里的尴尬状态是: {entityState} (提示: 期望修改1行但实际影响了0行)");
                }
                throw;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void ForceEntryAdded<TEntity>(TEntity entity) where TEntity : class
        {
            _context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Added;
        }
    }
}
