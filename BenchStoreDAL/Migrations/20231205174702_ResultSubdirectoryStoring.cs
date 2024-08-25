using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BenchStoreDAL.Migrations
{
    /// <inheritdoc />
    public partial class ResultSubdirectoryStoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Result_ResultEntry_ResultEntryID",
                table: "Result");

            migrationBuilder.DropIndex(
                name: "IX_Result_ResultEntryID",
                table: "Result");

            migrationBuilder.DropColumn(
                name: "ResultEntryID",
                table: "Result");

            migrationBuilder.AddColumn<int>(
                name: "ResultID",
                table: "ResultEntry",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ResultSubdirectoryName",
                table: "ResultEntry",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ResultEntry_ResultID",
                table: "ResultEntry",
                column: "ResultID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ResultEntry_Result_ResultID",
                table: "ResultEntry",
                column: "ResultID",
                principalTable: "Result",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultEntry_Result_ResultID",
                table: "ResultEntry");

            migrationBuilder.DropIndex(
                name: "IX_ResultEntry_ResultID",
                table: "ResultEntry");

            migrationBuilder.DropColumn(
                name: "ResultID",
                table: "ResultEntry");

            migrationBuilder.DropColumn(
                name: "ResultSubdirectoryName",
                table: "ResultEntry");

            migrationBuilder.AddColumn<int>(
                name: "ResultEntryID",
                table: "Result",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Result_ResultEntryID",
                table: "Result",
                column: "ResultEntryID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Result_ResultEntry_ResultEntryID",
                table: "Result",
                column: "ResultEntryID",
                principalTable: "ResultEntry",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
