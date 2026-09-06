using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OtoServisApp.Migrations
{
    /// <inheritdoc />
    public partial class StokHareketleri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockMovement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StokKalemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Miktar = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Sebep = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Referans = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Zaman = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovement", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovement_StokKalemId",
                table: "StockMovement",
                column: "StokKalemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockMovement");
        }
    }
}
