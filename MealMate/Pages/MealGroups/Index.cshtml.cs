using System.ComponentModel.DataAnnotations;
using MealMate.Data;
using MealMate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MealMate.Pages.MealGroups;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<MealGroup> Groups { get; private set; } = new List<MealGroup>();
    public MealGroupInput NewGroup { get; private set; } = new();
    public string? EditError { get; private set; }

    public int? FocusId { get; private set; }

    public async Task OnGetAsync(int? focus)
    {
        FocusId = focus;
        await LoadAsync();
        NewGroup = new MealGroupInput();
    }

    public async Task<IActionResult> OnPostAddAsync([FromForm] MealGroupInput group)
    {
        ModelState.Clear();
        group.AccentColor = string.IsNullOrWhiteSpace(group.AccentColor) ? null : group.AccentColor.Trim();
        group.Name = group.Name.Trim();
        group.Description = string.IsNullOrWhiteSpace(group.Description) ? null : group.Description.Trim();
        NewGroup = group;

        if (!TryValidateModel(NewGroup, nameof(NewGroup)))
        {
            await LoadAsync();
            return Page();
        }

        var duplicate = await _context.MealGroups
            .AnyAsync(g => g.Name.ToLower() == group.Name.ToLower());

        if (duplicate)
        {
            ModelState.AddModelError("NewGroup.Name", "Группа с таким названием уже существует.");
            await LoadAsync();
            return Page();
        }

        var entity = new MealGroup
        {
            Name = group.Name,
            Description = group.Description,
            AccentColor = group.AccentColor ?? "#2563EB"
        };

        _context.MealGroups.Add(entity);
        await _context.SaveChangesAsync();

        return RedirectToPage(new { focus = entity.Id });
    }

    public async Task<IActionResult> OnPostUpdateAsync([FromForm] MealGroupEditInput group)
    {
        ModelState.Clear();
        EditError = null;
        group.AccentColor = string.IsNullOrWhiteSpace(group.AccentColor) ? null : group.AccentColor.Trim();
        group.Name = group.Name.Trim();
        group.Description = string.IsNullOrWhiteSpace(group.Description) ? null : group.Description.Trim();

        if (group.Id <= 0)
        {
            return RedirectToPage();
        }

        if (!TryValidateModel(group))
        {
            EditError = "Проверьте правильность данных.";
            FocusId = group.Id;
            await LoadAsync();
            NewGroup = new MealGroupInput();
            return Page();
        }

        var entity = await _context.MealGroups.FindAsync(group.Id);
        if (entity is null)
        {
            return RedirectToPage();
        }

        var duplicate = await _context.MealGroups
            .AnyAsync(g => g.Id != group.Id && g.Name.ToLower() == group.Name.ToLower());

        if (duplicate)
        {
            EditError = "Другая группа уже использует такое название.";
            FocusId = group.Id;
            await LoadAsync();
            NewGroup = new MealGroupInput();
            return Page();
        }

        entity.Name = group.Name;
        entity.Description = group.Description;
        entity.AccentColor = group.AccentColor;

        await _context.SaveChangesAsync();
        return RedirectToPage(new { focus = entity.Id });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var group = await _context.MealGroups.FindAsync(id);
        if (group is null)
        {
            return RedirectToPage();
        }

        _context.MealGroups.Remove(group);
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        Groups = await _context.MealGroups
            .Include(g => g.MealGroupDishes)
                .ThenInclude(mgd => mgd.Dish)
            .OrderBy(g => g.Name)
            .ToListAsync();
    }

    public class MealGroupInput
    {
        [Required(ErrorMessage = "Введите название группы")]
        [StringLength(60)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [Display(Name = "Цвет акцента (#RRGGBB)")]
        [RegularExpression("^#(?:[0-9a-fA-F]{3}){1,2}$", ErrorMessage = "Используйте HEX формат, например #F97316.")]
        public string? AccentColor { get; set; }
    }

    public class MealGroupEditInput : MealGroupInput
    {
        public int Id { get; set; }
    }
}
