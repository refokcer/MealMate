using Microsoft.EntityFrameworkCore.Migrations;

namespace MealMate.Data.Migrations
{
    public partial class CreateMealPlannerSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MealGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 60, nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    AccentColor = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 80, nullable: false),
                    Category = table.Column<string>(maxLength: 40, nullable: true),
                    Notes = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dishes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 300, nullable: true),
                    Instructions = table.Column<string>(maxLength: 2000, nullable: true),
                    PreparationMinutes = table.Column<int>(nullable: true),
                    ImageUrl = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dishes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DishProducts",
                columns: table => new
                {
                    DishId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    Quantity = table.Column<string>(maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishProducts", x => new { x.DishId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_DishProducts_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealGroupDishes",
                columns: table => new
                {
                    MealGroupId = table.Column<int>(nullable: false),
                    DishId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealGroupDishes", x => new { x.MealGroupId, x.DishId });
                    table.ForeignKey(
                        name: "FK_MealGroupDishes_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealGroupDishes_MealGroups_MealGroupId",
                        column: x => x.MealGroupId,
                        principalTable: "MealGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MealGroups",
                columns: new[] { "Id", "AccentColor", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "#F97316", "Сочное утро", "Завтрак" },
                    { 2, "#2563EB", "Сытные идеи", "Обед" },
                    { 3, "#7C3AED", "Теплые семейные вечера", "Ужин" },
                    { 4, "#10B981", "Быстрые перекусы", "Закуски" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Name", "Notes" },
                values: new object[,]
                {
                    { 1, "Основные", "Яйца", "Свежие куриные" },
                    { 2, "Молочные", "Молоко", null },
                    { 3, "Крупы", "Овсяные хлопья", null },
                    { 4, "Овощи", "Авокадо", null },
                    { 5, "Выпечка", "Цельнозерновой хлеб", null },
                    { 6, "Белки", "Куриное филе", null },
                    { 7, "Овощи", "Сладкий перец", null },
                    { 8, "Крупы", "Рис басмати", null },
                    { 9, "Свежая", "Зелень", null }
                });

            migrationBuilder.InsertData(
                table: "Dishes",
                columns: new[] { "Id", "Description", "ImageUrl", "Instructions", "Name", "PreparationMinutes" },
                values: new object[,]
                {
                    { 1, "Классический полезный завтрак", "https://images.unsplash.com/photo-1504674900247-0877df9cc836", "Смешайте овсянку с молоком и варите 5 минут. Добавьте любимые ягоды.", "Овсяная каша с молоком", 10 },
                    { 2, "Лёгкий вариант для бодрого утра", "https://images.unsplash.com/photo-1499636136210-6f4ee915583e", "Поджарьте хлеб, разомните авокадо, добавьте яйцо-пашот и зелень.", "Тост с авокадо и яйцом", 12 },
                    { 3, "Простой обед для всей семьи", "https://images.unsplash.com/photo-1604908177636-01c02c9936a1", "Обжарьте курицу, добавьте перец и варёный рис. Приправьте по вкусу.", "Курица с рисом и перцем", 25 }
                });

            migrationBuilder.InsertData(
                table: "DishProducts",
                columns: new[] { "DishId", "ProductId", "Quantity" },
                values: new object[,]
                {
                    { 1, 2, "200 мл" },
                    { 1, 3, "60 г" },
                    { 2, 1, "1 шт" },
                    { 2, 4, "1/2 шт" },
                    { 2, 5, "1 ломтик" },
                    { 2, 9, "по вкусу" },
                    { 3, 6, "200 г" },
                    { 3, 7, "1 шт" },
                    { 3, 8, "150 г" },
                    { 3, 9, "по вкусу" }
                });

            migrationBuilder.InsertData(
                table: "MealGroupDishes",
                columns: new[] { "MealGroupId", "DishId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 3 },
                    { 3, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishProducts_ProductId",
                table: "DishProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_Name",
                table: "Dishes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealGroupDishes_DishId",
                table: "MealGroupDishes",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_MealGroups_Name",
                table: "MealGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                table: "Products",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DishProducts");

            migrationBuilder.DropTable(
                name: "MealGroupDishes");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "MealGroups");

            migrationBuilder.DropTable(
                name: "Dishes");
        }
    }
}
