using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OtoServisApp.Migrations
{
    /// <inheritdoc />
    public partial class FaturaVeSiraNumaralari : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Faturalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsEmriId = table.Column<int>(type: "INTEGER", nullable: false),
                    FaturaNo = table.Column<string>(type: "TEXT", nullable: false),
                    Tarih = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AraToplam = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ToplamKdv = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GenelToplam = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faturalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Faturalar_IsEmirleri_IsEmriId",
                        column: x => x.IsEmriId,
                        principalTable: "IsEmirleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SequenceCounter",
                columns: table => new
                {
                    Anahtar = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SonDeger = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SequenceCounter", x => x.Anahtar);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Faturalar_IsEmriId",
                table: "Faturalar",
                column: "IsEmriId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Faturalar_TenantId",
                table: "Faturalar",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Faturalar_TenantId_FaturaNo",
                table: "Faturalar",
                columns: new[] { "TenantId", "FaturaNo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Faturalar");

            migrationBuilder.DropTable(
                name: "SequenceCounter");
        }
    }
}
