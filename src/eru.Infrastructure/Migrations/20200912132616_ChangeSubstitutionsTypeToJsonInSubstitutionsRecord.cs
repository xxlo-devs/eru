using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using eru.Domain.Entity;

namespace eru.Infrastructure.Migrations
{
    public partial class ChangeSubstitutionsTypeToJsonInSubstitutionsRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Substitution_SubstitutionsRecords_SubstitutionsRecordUpload~",
                table: "Substitution");

            migrationBuilder.DropIndex(
                name: "IX_Substitution_SubstitutionsRecordUploadDateTime",
                table: "Substitution");

            migrationBuilder.DropColumn(
                name: "SubstitutionsRecordUploadDateTime",
                table: "Substitution");

            migrationBuilder.AddColumn<IEnumerable<Substitution>>(
                name: "Substitutions",
                table: "SubstitutionsRecords",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Substitutions",
                table: "SubstitutionsRecords");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubstitutionsRecordUploadDateTime",
                table: "Substitution",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Substitution_SubstitutionsRecordUploadDateTime",
                table: "Substitution",
                column: "SubstitutionsRecordUploadDateTime");

            migrationBuilder.AddForeignKey(
                name: "FK_Substitution_SubstitutionsRecords_SubstitutionsRecordUpload~",
                table: "Substitution",
                column: "SubstitutionsRecordUploadDateTime",
                principalTable: "SubstitutionsRecords",
                principalColumn: "UploadDateTime",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
