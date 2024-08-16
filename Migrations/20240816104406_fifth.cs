using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace qms_pure.Migrations
{
    /// <inheritdoc />
    public partial class fifth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "propStatus",
                table: "FileCats_tbl",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "propText",
                table: "FileCats_tbl",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "propStatus",
                table: "FileCats_tbl");

            migrationBuilder.DropColumn(
                name: "propText",
                table: "FileCats_tbl");
        }
    }
}
