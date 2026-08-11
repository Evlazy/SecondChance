using SecondChance.Domain.Entities;
using System.Text.Json.Serialization;

namespace SecondChance.Application.DTOs.Product;

public sealed class UpdateProductDto
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("condition")]
    public ProductCondition Condition { get; set; }

    [JsonPropertyName("categoryId")]
    public Guid CategoryId { get; set; }
}
