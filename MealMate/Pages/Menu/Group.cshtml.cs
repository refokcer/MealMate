using MealMate.Data;
using MealMate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MealMate.Pages.Menu;

public class GroupModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public GroupModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public MealGroup? Group { get; private set; }
    public IList<Dish> Dishes { get; private set; } = new List<Dish>();
    public IList<string> UniqueIngredients { get; private set; } = new List<string>();
    public IList<MealGroup> OtherGroups { get; private set; } = new List<MealGroup>();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Group = await _context.MealGroups
            .Include(g => g.MealGroupDishes)
                .ThenInclude(link => link.Dish)
                    .ThenInclude(d => d.DishProducts)
                        .ThenInclude(dp => dp.Product)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (Group is null)
        {
            await LoadOtherGroupsAsync(id);
            return Page();
        }

        Dishes = Group.MealGroupDishes
            .Select(link => link.Dish)
            .OrderBy(dish => dish.Name)
            .ToList();

        UniqueIngredients = Dishes
            .SelectMany(dish => dish.DishProducts)
            .Select(dp => dp.Product?.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .Distinct()
            .OrderBy(name => name)
            .ToList();

        await LoadOtherGroupsAsync(id);

        return Page();
    }

    private async Task LoadOtherGroupsAsync(int currentId)
    {
        OtherGroups = await _context.MealGroups
            .Where(group => group.Id != currentId)
            .OrderBy(group => group.Name)
            .ToListAsync();
    }
}
