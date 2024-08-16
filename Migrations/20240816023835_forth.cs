using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace qms_pure.Migrations
{
    /// <inheritdoc />
    public partial class forth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isRead",
                table: "FileCats_tbl",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isRead",
                table: "FileCats_tbl");
        }
    }
}
