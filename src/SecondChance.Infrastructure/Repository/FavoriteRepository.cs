using Microsoft.EntityFrameworkCore;
using SecondChance.Application.Interfaces;
using SecondChance.Domain.Entities;
using SecondChance.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Infrastructure.Repository
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly ApplicationDbContext _context;
        public FavoriteRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Favorite favorite)
        {
            await _context.Favorites.AddAsync(favorite);
        }

        public async Task<Favorite?> GetAsync(string userId, Guid productId)
        {
            return await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
        }

        public async Task<IEnumerable<Favorite>> GetUserFavoritesAsync(string userId)
        {
            return await _context.Favorites
                .Include(f => f.Product)
                    .ThenInclude(p => p.Images)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public void Remove(Favorite favorite)
        {
            _context.Favorites.Remove(favorite);
        }
    }
}
