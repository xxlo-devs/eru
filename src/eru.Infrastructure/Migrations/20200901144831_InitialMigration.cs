using Microsoft.EntityFrameworkCore.Migrations;

namespace eru.Infrastructure.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 255, nullable: false),
                    Year = table.Column<int>(nullable: false),
                    Section = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscribers",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 255, nullable: false),
                    Platform = table.Column<string>(maxLength: 255, nullable: false),
                    PreferredLanguage = table.Column<string>(maxLength: 255, nullable: true),
                    Class = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => new { x.Id, x.Platform });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Subscribers");
        }
    }
}
