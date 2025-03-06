using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bulky.DataAcces.Migrations
{
    /// <inheritdoc />
    public partial class productsToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    ListPrice50 = table.Column<double>(type: "float", nullable: false),
                    ListPrice100 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "Id", "Author", "Description", "ISBN", "ListPrice", "ListPrice100", "ListPrice50", "Title" },
                values: new object[,]
                {
                    { 1, "Giovanni Rana", "Un libro di merda", "1234567890123", 10.5, 8.0, 9.9900000000000002, "Dev Manual" },
                    { 2, "Beetlejuice", "Un altro libro di merda", "32109876543210", 100.0, 8.0, 12.0, "Tante Parole" },
                    { 3, "Anastasia", "Che ne sà la disney", "5234687512345", 11.0, 6.0, 8.0, "La Storia dei Romanov" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "products");
        }
    }
}
