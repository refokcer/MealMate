using System.ComponentModel.DataAnnotations;

namespace MealMate.Models;

public class MealGroup
{
    public int Id { get; set; }

    [Required]
    [MaxLength(60)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    [MaxLength(30)]
    public string? AccentColor { get; set; }

    public ICollection<MealGroupDish> MealGroupDishes { get; set; } = new List<MealGroupDish>();
}
