using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuaforApp.Migrations
{
    /// <inheritdoc />
    public partial class Personel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonelId",
                table: "Randevular",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Personeller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    AdSoyad = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personeller", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_PersonelId",
                table: "Randevular",
                column: "PersonelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Randevular_Personeller_PersonelId",
                table: "Randevular",
                column: "PersonelId",
                principalTable: "Personeller",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Randevular_Personeller_PersonelId",
                table: "Randevular");

            migrationBuilder.DropTable(
                name: "Personeller");

            migrationBuilder.DropIndex(
                name: "IX_Randevular_PersonelId",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "PersonelId",
                table: "Randevular");
        }
    }
}
