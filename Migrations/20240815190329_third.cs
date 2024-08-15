using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace qms_pure.Migrations
{
    /// <inheritdoc />
    public partial class third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileCats_tbl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderUserId = table.Column<int>(type: "int", nullable: false),
                    CatId = table.Column<int>(type: "int", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileCats_tbl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileCats_tbl_Categories_tbl_CatId",
                        column: x => x.CatId,
                        principalTable: "Categories_tbl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileCats_tbl_Users_tbl_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "Users_tbl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Files_tbl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileCatId = table.Column<int>(type: "int", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files_tbl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_tbl_FileCats_tbl_FileCatId",
                        column: x => x.FileCatId,
                        principalTable: "FileCats_tbl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileCats_tbl_CatId",
                table: "FileCats_tbl",
                column: "CatId");

            migrationBuilder.CreateIndex(
                name: "IX_FileCats_tbl_SenderUserId",
                table: "FileCats_tbl",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_tbl_FileCatId",
                table: "Files_tbl",
                column: "FileCatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files_tbl");

            migrationBuilder.DropTable(
                name: "FileCats_tbl");
        }
    }
}
