using SecondChance.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.Interfaces
{
    public interface IFavoriteRepository
    {
        Task<Favorite?> GetAsync(string userId, Guid productId);
        Task AddAsync(Favorite favorite);
        void Remove(Favorite favorite);
        Task<IEnumerable<Favorite>> GetUserFavoritesAsync(string userId);
    }
}
