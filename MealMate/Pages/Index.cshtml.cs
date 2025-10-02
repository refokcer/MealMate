using MealMate.Data;
using MealMate.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MealMate.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<MealGroup> MealGroups { get; private set; } = new List<MealGroup>();
    public IList<Dish> HighlightedDishes { get; private set; } = new List<Dish>();
    public IList<Product> PantryProducts { get; private set; } = new List<Product>();
    public IList<Dish> UngroupedDishes { get; private set; } = new List<Dish>();

    public async Task OnGetAsync()
    {
        MealGroups = await _context.MealGroups
            .Include(g => g.MealGroupDishes)
                .ThenInclude(mgd => mgd.Dish)
            .OrderBy(g => g.Name)
            .ToListAsync();

        HighlightedDishes = await _context.Dishes
            .Include(d => d.MealGroupDishes)
                .ThenInclude(mgd => mgd.MealGroup)
            .Include(d => d.DishProducts)
                .ThenInclude(dp => dp.Product)
            .OrderBy(d => d.PreparationMinutes ?? int.MaxValue)
            .ThenBy(d => d.Name)
            .Take(6)
            .ToListAsync();

        PantryProducts = await _context.Products
            .OrderBy(p => p.Name)
            .ToListAsync();

        UngroupedDishes = await _context.Dishes
            .Where(d => !d.MealGroupDishes.Any())
            .Include(d => d.DishProducts)
                .ThenInclude(dp => dp.Product)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }
}
