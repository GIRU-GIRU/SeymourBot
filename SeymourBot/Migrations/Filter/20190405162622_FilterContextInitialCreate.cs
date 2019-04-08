using Microsoft.EntityFrameworkCore.Migrations;

namespace SeymourBot.Migrations.Filter
{
    public partial class FilterContextInitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "filterTables",
                columns: table => new
                {
                    FilterId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FilterPattern = table.Column<string>(nullable: true),
                    FilterName = table.Column<string>(nullable: true),
                    FilterType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_filterTables", x => x.FilterId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "filterTables");
        }
    }
}
