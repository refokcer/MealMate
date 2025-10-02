using System.ComponentModel.DataAnnotations;
using MealMate.Data;
using MealMate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MealMate.Pages.Products;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Product> Products { get; private set; } = new List<Product>();
    public ProductCreateInput NewProduct { get; private set; } = new();
    public string? EditError { get; private set; }

    public int? FocusId { get; private set; }

    public async Task OnGetAsync(int? focus)
    {
        FocusId = focus;
        await LoadAsync();
        NewProduct = new ProductCreateInput();
    }

    public async Task<IActionResult> OnPostAddAsync([FromForm] ProductCreateInput newProduct)
    {
        ModelState.Clear();
        newProduct.Name = newProduct.Name?.Trim() ?? string.Empty;
        newProduct.Category = string.IsNullOrWhiteSpace(newProduct.Category) ? null : newProduct.Category.Trim();
        newProduct.Notes = string.IsNullOrWhiteSpace(newProduct.Notes) ? null : newProduct.Notes.Trim();
        NewProduct = newProduct;

        if (!TryValidateModel(NewProduct, nameof(NewProduct)))
        {
            await LoadAsync();
            return Page();
        }

        var duplicate = await _context.Products
            .AnyAsync(p => p.Name.ToLower() == newProduct.Name.ToLower());

        if (duplicate)
        {
            ModelState.AddModelError("NewProduct.Name", "Такой продукт уже есть в списке.");
            await LoadAsync();
            return Page();
        }

        var product = new Product
        {
            Name = newProduct.Name,
            Category = newProduct.Category,
            Notes = newProduct.Notes
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return RedirectToPage(new { focus = product.Id });
    }

    public async Task<IActionResult> OnPostUpdateAsync([FromForm] ProductEditInput product)
    {
        ModelState.Clear();
        EditError = null;
        product.Name = product.Name?.Trim() ?? string.Empty;
        product.Category = string.IsNullOrWhiteSpace(product.Category) ? null : product.Category.Trim();
        product.Notes = string.IsNullOrWhiteSpace(product.Notes) ? null : product.Notes.Trim();

        if (product.Id <= 0)
        {
            return RedirectToPage();
        }

        if (!TryValidateModel(product))
        {
            EditError = "Проверьте введённые данные.";
            FocusId = product.Id;
            await LoadAsync();
            NewProduct = new ProductCreateInput();
            return Page();
        }

        var entity = await _context.Products.FindAsync(product.Id);
        if (entity is null)
        {
            return RedirectToPage();
        }

        var duplicate = await _context.Products
            .AnyAsync(p => p.Id != product.Id && p.Name.ToLower() == product.Name.ToLower());
        if (duplicate)
        {
            EditError = "Продукт с таким названием уже существует.";
            FocusId = product.Id;
            await LoadAsync();
            NewProduct = new ProductCreateInput();
            return Page();
        }

        entity.Name = product.Name;
        entity.Category = product.Category;
        entity.Notes = product.Notes;

        await _context.SaveChangesAsync();
        return RedirectToPage(new { focus = entity.Id });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null)
        {
            return RedirectToPage();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        Products = await _context.Products
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public class ProductCreateInput
    {
        [Required(ErrorMessage = "Введите название продукта")]
        [StringLength(80)]
        public string Name { get; set; } = string.Empty;

        [StringLength(40)]
        [Display(Name = "Категория")]
        public string? Category { get; set; }

        [StringLength(200)]
        [Display(Name = "Примечания")]
        public string? Notes { get; set; }
    }

    public class ProductEditInput : ProductCreateInput
    {
        public int Id { get; set; }
    }
}
