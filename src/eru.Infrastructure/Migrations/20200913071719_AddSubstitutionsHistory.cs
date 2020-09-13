using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using eru.Domain.Entity;

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

            migrationBuilder.CreateTable(
                name: "SubstitutionsRecords",
                columns: table => new
                {
                    UploadDateTime = table.Column<DateTime>(nullable: false),
                    SubstitutionsDate = table.Column<DateTime>(nullable: false),
                    Substitutions = table.Column<IEnumerable<Substitution>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubstitutionsRecords", x => x.UploadDateTime);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubstitutionsRecords");

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
