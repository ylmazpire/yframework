using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OtoServisApp.Migrations
{
    /// <inheritdoc />
    public partial class GiderlerVeMuhasebe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Giderler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Kategori = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Giderler", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Faturalar_TenantId_Tarih",
                table: "Faturalar",
                columns: new[] { "TenantId", "Tarih" });

            migrationBuilder.CreateIndex(
                name: "IX_Giderler_TenantId",
                table: "Giderler",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Giderler_TenantId_Tarih",
                table: "Giderler",
                columns: new[] { "TenantId", "Tarih" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Giderler");

            migrationBuilder.DropIndex(
                name: "IX_Faturalar_TenantId_Tarih",
                table: "Faturalar");
        }
    }
}
