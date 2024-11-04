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
            migrationBuilder.AlterColumn<string>(
                name: "ResultFileName",
                table: "ResultEntry",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResultSubdirectoryName",
                table: "ResultEntry",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultSubdirectoryName",
                table: "ResultEntry");

            migrationBuilder.AlterColumn<string>(
                name: "ResultFileName",
                table: "ResultEntry",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
