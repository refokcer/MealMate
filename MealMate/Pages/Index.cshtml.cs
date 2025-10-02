using MealMate.Data;
using MealMate.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MealMate.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<MealGroup> MealGroups { get; private set; } = new List<MealGroup>();
    public MealGroup? SelectedGroup { get; private set; }
    public IList<Dish> UngroupedDishes { get; private set; } = new List<Dish>();
    public IList<string> HighlightedIngredients { get; private set; } = new List<string>();
    public int TotalMealGroups { get; private set; }
    public int TotalDishes { get; private set; }
    public int TotalUniqueIngredients { get; private set; }

    public async Task OnGetAsync(int? focusGroup)
    {
        MealGroups = await _context.MealGroups
            .Include(g => g.MealGroupDishes)
                .ThenInclude(mgd => mgd.Dish)
                    .ThenInclude(d => d.DishProducts)
                        .ThenInclude(dp => dp.Product)
            .OrderBy(g => g.Name)
            .ToListAsync();

        TotalMealGroups = MealGroups.Count;
        TotalDishes = await _context.Dishes.CountAsync();
        TotalUniqueIngredients = await _context.DishProducts
            .Select(dp => dp.ProductId)
            .Distinct()
            .CountAsync();

        HighlightedIngredients = await _context.DishProducts
            .Include(dp => dp.Product)
            .Where(dp => dp.Product != null)
            .GroupBy(dp => dp.Product!.Name)
            .Select(g => new { g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ThenBy(g => g.Key)
            .Take(12)
            .Select(g => g.Key)
            .ToListAsync();

        SelectedGroup = focusGroup.HasValue
            ? MealGroups.FirstOrDefault(g => g.Id == focusGroup.Value)
            : MealGroups.FirstOrDefault();

        SelectedGroup ??= MealGroups.FirstOrDefault();


        UngroupedDishes = await _context.Dishes
            .Where(d => !d.MealGroupDishes.Any())
            .Include(d => d.DishProducts)
                .ThenInclude(dp => dp.Product)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }
}
