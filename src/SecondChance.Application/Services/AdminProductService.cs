using SecondChance.Application.Interfaces;
using SecondChance.Domain.Interfaces;
using SecondChance.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecondChance.Application.Services
{
    public class AdminProductService : IAdminProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdminProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> FreezeProductAsync(Guid productId, string reason)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null) return false;

            product.IsAvailable = false;
            //product.Status = ProductStatus.Unavailable;
            product.AuditReason = reason;
            product.LastModifiedAt = DateTime.UtcNow;

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        public async Task<bool> UnFreezeProductAsync(Guid productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null) return false;

            product.IsAvailable = true;
            product.LastModifiedAt = DateTime.UtcNow;

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
