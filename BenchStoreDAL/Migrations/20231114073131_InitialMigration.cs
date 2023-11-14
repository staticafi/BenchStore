using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BenchStoreDAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Label",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Label", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ResultEntry",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    LastAccessTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResultFileName = table.Column<string>(type: "text", nullable: true),
                    LogFilesName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultEntry", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LabelResultEntry",
                columns: table => new
                {
                    LabelsID = table.Column<int>(type: "integer", nullable: false),
                    ResultEntriesID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabelResultEntry", x => new { x.LabelsID, x.ResultEntriesID });
                    table.ForeignKey(
                        name: "FK_LabelResultEntry_Label_LabelsID",
                        column: x => x.LabelsID,
                        principalTable: "Label",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabelResultEntry_ResultEntry_ResultEntriesID",
                        column: x => x.ResultEntriesID,
                        principalTable: "ResultEntry",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Result",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    BenchmarkName = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    Block = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Tool = table.Column<string>(type: "text", nullable: true),
                    ToolModule = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<string>(type: "text", nullable: true),
                    Options = table.Column<string>(type: "text", nullable: true),
                    MemLimit = table.Column<long>(type: "bigint", nullable: false),
                    TimeLimit = table.Column<long>(type: "bigint", nullable: false),
                    CPUCores = table.Column<long>(type: "bigint", nullable: false),
                    Generator = table.Column<string>(type: "text", nullable: true),
                    Error = table.Column<string>(type: "text", nullable: true),
                    ResultEntryID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Result", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Result_ResultEntry_ResultEntryID",
                        column: x => x.ResultEntryID,
                        principalTable: "ResultEntry",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabelResultEntry_ResultEntriesID",
                table: "LabelResultEntry",
                column: "ResultEntriesID");

            migrationBuilder.CreateIndex(
                name: "IX_Result_ResultEntryID",
                table: "Result",
                column: "ResultEntryID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabelResultEntry");

            migrationBuilder.DropTable(
                name: "Result");

            migrationBuilder.DropTable(
                name: "Label");

            migrationBuilder.DropTable(
                name: "ResultEntry");
        }
    }
}
