using System.ComponentModel.DataAnnotations;

namespace MealMate.Models;

public class DishProduct
{
    public int DishId { get; set; }
    public Dish Dish { get; set; } = default!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;

    [MaxLength(80)]
    public string? Quantity { get; set; }
}
