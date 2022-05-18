using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ACI.Products.Migrations
{
    public partial class UpdateCatalogPos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Products_CatalogPosition",
                table: "Products",
                column: "CatalogPosition",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_CatalogPosition",
                table: "Products");
        }
    }
}
