using Microsoft.EntityFrameworkCore;
using SecondChance.Domain.Entities;
using SecondChance.Domain.Interfaces;
using SecondChance.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Infrastructure.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbSet.AnyAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
