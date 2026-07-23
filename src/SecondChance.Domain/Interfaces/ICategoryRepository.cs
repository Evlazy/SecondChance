using SecondChance.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Domain.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<Category>> GetAllCategories();

    }
}
