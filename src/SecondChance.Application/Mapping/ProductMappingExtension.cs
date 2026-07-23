using SecondChance.Application.DTOs.Product;
using SecondChance.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SecondChance.Application.Mapping
{
    public static class ProductMappingExtension
    {
        public static ProudctDto ToDto(this Product product)
        {
            if (product == null) return null;

            return new ProudctDto
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                Condition = product.Condition.ToString(),
                CategoryId = product.CategoryId,
                SellerId = product.SellerId
            };
        }
    }
}
