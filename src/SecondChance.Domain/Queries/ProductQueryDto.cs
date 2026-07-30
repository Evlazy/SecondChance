using System.ComponentModel.DataAnnotations;

namespace SecondChance.Domain.Queries;

public sealed class ProductQueryDto
{
    [StringLength(200)]
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
    [RegularExpression("^(price_asc|price_desc)?$")]
    public string? SortBy { get; set; }
}
