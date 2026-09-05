using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuaforApp.Migrations
{
    /// <inheritdoc />
    public partial class PerformansIndeksleri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Randevular_TenantId",
                table: "Randevular",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_TenantId_BaslangicZamani",
                table: "Randevular",
                columns: new[] { "TenantId", "BaslangicZamani" });

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_TenantId",
                table: "Personeller",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Musteriler_TenantId",
                table: "Musteriler",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MuhasebeKayitlari_TenantId",
                table: "MuhasebeKayitlari",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MuhasebeKayitlari_TenantId_Tarih",
                table: "MuhasebeKayitlari",
                columns: new[] { "TenantId", "Tarih" });

            migrationBuilder.CreateIndex(
                name: "IX_MuhasebeKategorileri_TenantId",
                table: "MuhasebeKategorileri",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Hizmetler_TenantId",
                table: "Hizmetler",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Randevular_TenantId",
                table: "Randevular");

            migrationBuilder.DropIndex(
                name: "IX_Randevular_TenantId_BaslangicZamani",
                table: "Randevular");

            migrationBuilder.DropIndex(
                name: "IX_Personeller_TenantId",
                table: "Personeller");

            migrationBuilder.DropIndex(
                name: "IX_Musteriler_TenantId",
                table: "Musteriler");

            migrationBuilder.DropIndex(
                name: "IX_MuhasebeKayitlari_TenantId",
                table: "MuhasebeKayitlari");

            migrationBuilder.DropIndex(
                name: "IX_MuhasebeKayitlari_TenantId_Tarih",
                table: "MuhasebeKayitlari");

            migrationBuilder.DropIndex(
                name: "IX_MuhasebeKategorileri_TenantId",
                table: "MuhasebeKategorileri");

            migrationBuilder.DropIndex(
                name: "IX_Hizmetler_TenantId",
                table: "Hizmetler");
        }
    }
}
