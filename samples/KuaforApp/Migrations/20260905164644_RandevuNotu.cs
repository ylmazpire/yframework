using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuaforApp.Migrations
{
    /// <inheritdoc />
    public partial class RandevuNotu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Not",
                table: "Randevular",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Not",
                table: "Randevular");
        }
    }
}
