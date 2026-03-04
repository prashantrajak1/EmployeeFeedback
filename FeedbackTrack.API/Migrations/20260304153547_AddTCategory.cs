using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FeedbackTrack.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TCategories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Excellent Work" },
                    { 2, "Team Player" },
                    { 3, "Problem Solver" },
                    { 4, "Innovation" },
                    { 5, "Customer Focus" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TCategories");
        }
    }
}
