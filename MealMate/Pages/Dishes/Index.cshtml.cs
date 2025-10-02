using System.ComponentModel.DataAnnotations;
using MealMate.Data;
using MealMate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MealMate.Pages.Dishes;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Dish> Dishes { get; private set; } = new List<Dish>();
    public IList<SelectListItem> ProductOptions { get; private set; } = new List<SelectListItem>();
    public IList<SelectListItem> MealGroupOptions { get; private set; } = new List<SelectListItem>();
    public int? FocusId { get; private set; }
    [BindProperty]
    public DishInputModel NewDish { get; set; } = new();

    public async Task OnGetAsync(int? focus)
    {
        FocusId = focus;
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync();
            return Page();
        }

        var dish = new Dish
        {
            Name = NewDish.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(NewDish.Description) ? null : NewDish.Description.Trim(),
            Instructions = string.IsNullOrWhiteSpace(NewDish.Instructions) ? null : NewDish.Instructions.Trim(),
            PreparationMinutes = NewDish.PreparationMinutes,
            ImageUrl = string.IsNullOrWhiteSpace(NewDish.ImageUrl) ? null : NewDish.ImageUrl.Trim()
        };

        foreach (var productId in NewDish.SelectedProductIds.Distinct())
        {
            dish.DishProducts.Add(new DishProduct { ProductId = productId });
        }

        foreach (var groupId in NewDish.SelectedMealGroupIds.Distinct())
        {
            dish.MealGroupDishes.Add(new MealGroupDish { MealGroupId = groupId });
        }

        _context.Dishes.Add(dish);
        await _context.SaveChangesAsync();

        return RedirectToPage(new { focus = dish.Id });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var dish = await _context.Dishes.FindAsync(id);
        if (dish is null)
        {
            return RedirectToPage();
        }

        _context.Dishes.Remove(dish);
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        Dishes = await _context.Dishes
            .Include(d => d.DishProducts)
                .ThenInclude(dp => dp.Product)
            .Include(d => d.MealGroupDishes)
                .ThenInclude(mgd => mgd.MealGroup)
            .OrderBy(d => d.Name)
            .ToListAsync();

        ProductOptions = await _context.Products
            .OrderBy(p => p.Name)
            .Select(p => new SelectListItem(p.Name, p.Id.ToString()))
            .ToListAsync();

        MealGroupOptions = await _context.MealGroups
            .OrderBy(g => g.Name)
            .Select(g => new SelectListItem(g.Name, g.Id.ToString()))
            .ToListAsync();
    }

    public class DishInputModel
    {
        [Required(ErrorMessage = "Введите название блюда")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        [Display(Name = "Краткое описание")]
        public string? Description { get; set; }

        [StringLength(2000)]
        [Display(Name = "Инструкция приготовления")]
        public string? Instructions { get; set; }

        [Range(1, 360, ErrorMessage = "Укажите время от 1 до 360 минут")]
        [Display(Name = "Время, мин")]
        public int? PreparationMinutes { get; set; }

        [Url]
        [Display(Name = "Ссылка на изображение")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Используемые продукты")]
        public List<int> SelectedProductIds { get; set; } = new();

        [Display(Name = "Группы блюд")]
        public List<int> SelectedMealGroupIds { get; set; } = new();
    }
}
