using SecondChance.Domain.Entities;

namespace SecondChance.Application.DTOs.Product;

public sealed class UpdateProductDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ProductCondition Condition { get; set; }
}
