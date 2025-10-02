using MealMate.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MealMate.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Dish> Dishes => Set<Dish>();
    public DbSet<DishProduct> DishProducts => Set<DishProduct>();
    public DbSet<MealGroup> MealGroups => Set<MealGroup>();
    public DbSet<MealGroupDish> MealGroupDishes => Set<MealGroupDish>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<DishProduct>().HasKey(x => new { x.DishId, x.ProductId });
        builder.Entity<DishProduct>()
            .HasOne(x => x.Dish)
            .WithMany(d => d.DishProducts)
            .HasForeignKey(x => x.DishId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<DishProduct>()
            .HasOne(x => x.Product)
            .WithMany(p => p.DishProducts)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<MealGroupDish>().HasKey(x => new { x.MealGroupId, x.DishId });
        builder.Entity<MealGroupDish>()
            .HasOne(x => x.MealGroup)
            .WithMany(g => g.MealGroupDishes)
            .HasForeignKey(x => x.MealGroupId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<MealGroupDish>()
            .HasOne(x => x.Dish)
            .WithMany(d => d.MealGroupDishes)
            .HasForeignKey(x => x.DishId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Product>().HasIndex(p => p.Name).IsUnique();
        builder.Entity<Dish>().HasIndex(d => d.Name).IsUnique();
        builder.Entity<MealGroup>().HasIndex(g => g.Name).IsUnique();

        SeedInitialData(builder);
    }

    private static void SeedInitialData(ModelBuilder builder)
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Яйца", Category = "Основные", Notes = "Свежие куриные" },
            new() { Id = 2, Name = "Молоко", Category = "Молочные" },
            new() { Id = 3, Name = "Овсяные хлопья", Category = "Крупы" },
            new() { Id = 4, Name = "Авокадо", Category = "Овощи" },
            new() { Id = 5, Name = "Цельнозерновой хлеб", Category = "Выпечка" },
            new() { Id = 6, Name = "Куриное филе", Category = "Белки" },
            new() { Id = 7, Name = "Сладкий перец", Category = "Овощи" },
            new() { Id = 8, Name = "Рис басмати", Category = "Крупы" },
            new() { Id = 9, Name = "Зелень", Category = "Свежая" }
        };

        var mealGroups = new List<MealGroup>
        {
            new() { Id = 1, Name = "Завтрак", Description = "Сочное утро", AccentColor = "#F97316" },
            new() { Id = 2, Name = "Обед", Description = "Сытные идеи", AccentColor = "#2563EB" },
            new() { Id = 3, Name = "Ужин", Description = "Теплые семейные вечера", AccentColor = "#7C3AED" },
            new() { Id = 4, Name = "Закуски", Description = "Быстрые перекусы", AccentColor = "#10B981" }
        };

        var dishes = new List<Dish>
        {
            new()
            {
                Id = 1,
                Name = "Овсяная каша с молоком",
                Description = "Классический полезный завтрак",
                Instructions = "Смешайте овсянку с молоком и варите 5 минут. Добавьте любимые ягоды.",
                PreparationMinutes = 10,
                ImageUrl = "https://images.unsplash.com/photo-1504674900247-0877df9cc836"
            },
            new()
            {
                Id = 2,
                Name = "Тост с авокадо и яйцом",
                Description = "Лёгкий вариант для бодрого утра",
                Instructions = "Поджарьте хлеб, разомните авокадо, добавьте яйцо-пашот и зелень.",
                PreparationMinutes = 12,
                ImageUrl = "https://images.unsplash.com/photo-1499636136210-6f4ee915583e"
            },
            new()
            {
                Id = 3,
                Name = "Курица с рисом и перцем",
                Description = "Простой обед для всей семьи",
                Instructions = "Обжарьте курицу, добавьте перец и варёный рис. Приправьте по вкусу.",
                PreparationMinutes = 25,
                ImageUrl = "https://images.unsplash.com/photo-1604908177636-01c02c9936a1"
            }
        };

        var dishProducts = new List<DishProduct>
        {
            new() { DishId = 1, ProductId = 2, Quantity = "200 мл" },
            new() { DishId = 1, ProductId = 3, Quantity = "60 г" },
            new() { DishId = 2, ProductId = 1, Quantity = "1 шт" },
            new() { DishId = 2, ProductId = 4, Quantity = "1/2 шт" },
            new() { DishId = 2, ProductId = 5, Quantity = "1 ломтик" },
            new() { DishId = 2, ProductId = 9, Quantity = "по вкусу" },
            new() { DishId = 3, ProductId = 6, Quantity = "200 г" },
            new() { DishId = 3, ProductId = 7, Quantity = "1 шт" },
            new() { DishId = 3, ProductId = 8, Quantity = "150 г" },
            new() { DishId = 3, ProductId = 9, Quantity = "по вкусу" }
        };

        var mealGroupDishes = new List<MealGroupDish>
        {
            new() { MealGroupId = 1, DishId = 1 },
            new() { MealGroupId = 1, DishId = 2 },
            new() { MealGroupId = 2, DishId = 3 },
            new() { MealGroupId = 3, DishId = 3 }
        };

        builder.Entity<Product>().HasData(products);
        builder.Entity<MealGroup>().HasData(mealGroups);
        builder.Entity<Dish>().HasData(dishes);
        builder.Entity<DishProduct>().HasData(dishProducts);
        builder.Entity<MealGroupDish>().HasData(mealGroupDishes);
    }
}
