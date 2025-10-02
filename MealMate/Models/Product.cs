using System.ComponentModel.DataAnnotations;

namespace MealMate.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    [MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(40)]
    public string? Category { get; set; }

    [MaxLength(200)]
    public string? Notes { get; set; }

    public ICollection<DishProduct> DishProducts { get; set; } = new List<DishProduct>();
}
