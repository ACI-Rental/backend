using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PDFService.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pdfs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Blob = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    LinkedKey = table.Column<int>(type: "int", nullable: false),
                    LinkedTableType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pdfs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pdfs");
        }
    }
}
