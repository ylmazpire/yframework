using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuaforApp.Migrations
{
    /// <inheritdoc />
    public partial class MuhasebeModulu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MuhasebeKategorileri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", nullable: false),
                    Tur = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuhasebeKategorileri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MuhasebeKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KategoriId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tutar = table.Column<decimal>(type: "TEXT", nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", nullable: false),
                    Tarih = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RandevuId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuhasebeKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MuhasebeKayitlari_MuhasebeKategorileri_KategoriId",
                        column: x => x.KategoriId,
                        principalTable: "MuhasebeKategorileri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MuhasebeKayitlari_Randevular_RandevuId",
                        column: x => x.RandevuId,
                        principalTable: "Randevular",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MuhasebeKayitlari_KategoriId",
                table: "MuhasebeKayitlari",
                column: "KategoriId");

            migrationBuilder.CreateIndex(
                name: "IX_MuhasebeKayitlari_RandevuId",
                table: "MuhasebeKayitlari",
                column: "RandevuId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MuhasebeKayitlari");

            migrationBuilder.DropTable(
                name: "MuhasebeKategorileri");
        }
    }
}
