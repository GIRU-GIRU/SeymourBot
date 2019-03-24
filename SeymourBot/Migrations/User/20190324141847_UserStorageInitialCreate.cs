using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SeymourBot.Migrations.User
{
    public partial class UserStorageInitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlackListedTable",
                columns: table => new
                {
                    UserID = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(nullable: true),
                    ModeratorID = table.Column<ulong>(nullable: false),
                    DateInserted = table.Column<DateTime>(nullable: false),
                    DateToRemove = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlackListedTable", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "UserDisciplinaryEventArchiveTable",
                columns: table => new
                {
                    ArchiveID = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<ulong>(nullable: false),
                    DisciplineType = table.Column<int>(nullable: false),
                    ModeratorID = table.Column<ulong>(nullable: false),
                    DateInserted = table.Column<DateTime>(nullable: false),
                    DateToRemove = table.Column<DateTime>(nullable: false),
                    DateArchived = table.Column<DateTime>(nullable: false),
                    Reason = table.Column<string>(nullable: true),
                    DisciplineEventID = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDisciplinaryEventArchiveTable", x => x.ArchiveID);
                });

            migrationBuilder.CreateTable(
                name: "UserDisciplinaryEventStorageTable",
                columns: table => new
                {
                    DisciplineEventID = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<ulong>(nullable: false),
                    DiscipinaryEventType = table.Column<int>(nullable: false),
                    ModeratorID = table.Column<ulong>(nullable: false),
                    DateInserted = table.Column<DateTime>(nullable: false),
                    DateToRemove = table.Column<DateTime>(nullable: false),
                    Reason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDisciplinaryEventStorageTable", x => x.DisciplineEventID);
                });

            migrationBuilder.CreateTable(
                name: "UserDisciplinaryPermanentStorageTable",
                columns: table => new
                {
                    DisciplineEventID = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<ulong>(nullable: false),
                    DiscipinaryEventType = table.Column<int>(nullable: false),
                    ModeratorID = table.Column<ulong>(nullable: false),
                    DateInserted = table.Column<DateTime>(nullable: false),
                    Reason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDisciplinaryPermanentStorageTable", x => x.DisciplineEventID);
                });

            migrationBuilder.CreateTable(
                name: "UserRoleStorageTable",
                columns: table => new
                {
                    UserID = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleStorageTable", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "UserStorageTable",
                columns: table => new
                {
                    UserID = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStorageTable", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "GuildRoleStorage",
                columns: table => new
                {
                    RoleID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleName = table.Column<string>(nullable: true),
                    UserRoleStorageUserID = table.Column<ulong>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildRoleStorage", x => x.RoleID);
                    table.ForeignKey(
                        name: "FK_GuildRoleStorage_UserRoleStorageTable_UserRoleStorageUserID",
                        column: x => x.UserRoleStorageUserID,
                        principalTable: "UserRoleStorageTable",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuildRoleStorage_UserRoleStorageUserID",
                table: "GuildRoleStorage",
                column: "UserRoleStorageUserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlackListedTable");

            migrationBuilder.DropTable(
                name: "GuildRoleStorage");

            migrationBuilder.DropTable(
                name: "UserDisciplinaryEventArchiveTable");

            migrationBuilder.DropTable(
                name: "UserDisciplinaryEventStorageTable");

            migrationBuilder.DropTable(
                name: "UserDisciplinaryPermanentStorageTable");

            migrationBuilder.DropTable(
                name: "UserStorageTable");

            migrationBuilder.DropTable(
                name: "UserRoleStorageTable");
        }
    }
}
