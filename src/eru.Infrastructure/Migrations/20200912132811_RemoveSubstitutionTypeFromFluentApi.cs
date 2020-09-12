using Microsoft.EntityFrameworkCore.Migrations;

namespace eru.Infrastructure.Migrations
{
    public partial class RemoveSubstitutionTypeFromFluentApi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Substitution_SubstitutionId",
                table: "Classes");

            migrationBuilder.DropTable(
                name: "Substitution");

            migrationBuilder.DropIndex(
                name: "IX_Classes_SubstitutionId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "SubstitutionId",
                table: "Classes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubstitutionId",
                table: "Classes",
                type: "character varying(255)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Substitution",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Cancelled = table.Column<bool>(type: "boolean", nullable: false),
                    Groups = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Lesson = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Room = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Substituting = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Teacher = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Substitution", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_SubstitutionId",
                table: "Classes",
                column: "SubstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Substitution_SubstitutionId",
                table: "Classes",
                column: "SubstitutionId",
                principalTable: "Substitution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
