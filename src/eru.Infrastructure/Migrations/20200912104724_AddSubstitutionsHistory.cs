using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace eru.Infrastructure.Migrations
{
    public partial class AddSubstitutionsHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PreferredLanguage",
                table: "Subscribers",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Class",
                table: "Subscribers",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Platform",
                table: "Subscribers",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Subscribers",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "Classes",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Section",
                table: "Classes",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Classes",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "SubstitutionId",
                table: "Classes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubstitutionsRecords",
                columns: table => new
                {
                    UploadDateTime = table.Column<DateTime>(nullable: false),
                    SubstitutionsDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubstitutionsRecords", x => x.UploadDateTime);
                });

            migrationBuilder.CreateTable(
                name: "Substitution",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 255, nullable: false),
                    Teacher = table.Column<string>(maxLength: 255, nullable: true),
                    Lesson = table.Column<int>(nullable: false),
                    Subject = table.Column<string>(maxLength: 255, nullable: true),
                    Groups = table.Column<string>(maxLength: 255, nullable: true),
                    Cancelled = table.Column<bool>(nullable: false),
                    Substituting = table.Column<string>(maxLength: 255, nullable: true),
                    Note = table.Column<string>(maxLength: 255, nullable: true),
                    Room = table.Column<string>(maxLength: 255, nullable: true),
                    SubstitutionsRecordUploadDateTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Substitution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Substitution_SubstitutionsRecords_SubstitutionsRecordUpload~",
                        column: x => x.SubstitutionsRecordUploadDateTime,
                        principalTable: "SubstitutionsRecords",
                        principalColumn: "UploadDateTime",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_SubstitutionId",
                table: "Classes",
                column: "SubstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Substitution_SubstitutionsRecordUploadDateTime",
                table: "Substitution",
                column: "SubstitutionsRecordUploadDateTime");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Substitution_SubstitutionId",
                table: "Classes",
                column: "SubstitutionId",
                principalTable: "Substitution",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Substitution_SubstitutionId",
                table: "Classes");

            migrationBuilder.DropTable(
                name: "Substitution");

            migrationBuilder.DropTable(
                name: "SubstitutionsRecords");

            migrationBuilder.DropIndex(
                name: "IX_Classes_SubstitutionId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "SubstitutionId",
                table: "Classes");

            migrationBuilder.AlterColumn<string>(
                name: "PreferredLanguage",
                table: "Subscribers",
                type: "TEXT",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Class",
                table: "Subscribers",
                type: "TEXT",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Platform",
                table: "Subscribers",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Subscribers",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "Classes",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Section",
                table: "Classes",
                type: "TEXT",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Classes",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 255);
        }
    }
}
