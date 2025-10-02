namespace MealMate.Models;

public class MealGroupDish
{
    public int MealGroupId { get; set; }
    public MealGroup MealGroup { get; set; } = default!;

    public int DishId { get; set; }
    public Dish Dish { get; set; } = default!;
}
