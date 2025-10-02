using System.ComponentModel.DataAnnotations;

namespace MealMate.Models;

public class Dish
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }

    [MaxLength(2000)]
    public string? Instructions { get; set; }

    public int? PreparationMinutes { get; set; }

    [MaxLength(200)]
    public string? ImageUrl { get; set; }

    public ICollection<DishProduct> DishProducts { get; set; } = new List<DishProduct>();

    public ICollection<MealGroupDish> MealGroupDishes { get; set; } = new List<MealGroupDish>();
}
