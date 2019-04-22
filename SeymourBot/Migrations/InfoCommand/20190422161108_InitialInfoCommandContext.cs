using Microsoft.EntityFrameworkCore.Migrations;

namespace SeymourBot.Migrations.InfoCommand
{
    public partial class InitialInfoCommandContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InfoCommandTable",
                columns: table => new
                {
                    CommandID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommandName = table.Column<string>(nullable: true),
                    CommandContent = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoCommandTable", x => x.CommandID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InfoCommandTable");
        }
    }
}
