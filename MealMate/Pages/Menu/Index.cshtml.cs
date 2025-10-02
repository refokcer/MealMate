using MealMate.Data;
using MealMate.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MealMate.Pages.Menu;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<MealGroup> MealGroups { get; private set; } = new List<MealGroup>();
    public int TotalGroups { get; private set; }
    public int TotalDishes { get; private set; }
    public int UngroupedDishesCount { get; private set; }

    public async Task OnGetAsync()
    {
        MealGroups = await _context.MealGroups
            .Include(group => group.MealGroupDishes)
                .ThenInclude(link => link.Dish)
                    .ThenInclude(dish => dish.DishProducts)
                        .ThenInclude(dp => dp.Product)
            .OrderBy(group => group.Name)
            .ToListAsync();

        TotalGroups = MealGroups.Count;
        TotalDishes = await _context.Dishes.CountAsync();
        UngroupedDishesCount = await _context.Dishes
            .CountAsync(dish => !dish.MealGroupDishes.Any());
    }
}
