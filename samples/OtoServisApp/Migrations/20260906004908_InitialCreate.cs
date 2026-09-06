using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OtoServisApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AdSoyad = table.Column<string>(type: "TEXT", nullable: false),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Isletmeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Isletmeler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Musteriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    AdSoyad = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Telefon = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    VergiKimlikNo = table.Column<string>(type: "TEXT", maxLength: 11, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Musteriler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Parcalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    StokKodu = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    StokAdedi = table.Column<int>(type: "INTEGER", nullable: false),
                    AlisFiyati = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    SatisFiyati = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcalar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personeller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    AdSoyad = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Telefon = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Uzmanlik = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personeller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServisKanallari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Aktif = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServisKanallari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Araclar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    MusteriId = table.Column<int>(type: "INTEGER", nullable: false),
                    Plaka = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Marka = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ModelYili = table.Column<int>(type: "INTEGER", nullable: false),
                    Kilometre = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Araclar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Araclar_Musteriler_MusteriId",
                        column: x => x.MusteriId,
                        principalTable: "Musteriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IsEmirleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    AracId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServisKanaliId = table.Column<int>(type: "INTEGER", nullable: true),
                    AtananPersonelId = table.Column<int>(type: "INTEGER", nullable: true),
                    GelisTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PlanlananBaslangic = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PlanlananBitis = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Durum = table.Column<int>(type: "INTEGER", nullable: false),
                    MusteriSikayeti = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    YapilanIsler = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    GelisKilometresi = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IsEmirleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IsEmirleri_Araclar_AracId",
                        column: x => x.AracId,
                        principalTable: "Araclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IsEmirleri_Personeller_AtananPersonelId",
                        column: x => x.AtananPersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IsEmirleri_ServisKanallari_ServisKanaliId",
                        column: x => x.ServisKanaliId,
                        principalTable: "ServisKanallari",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IsEmriKalemleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TenantId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsEmriId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tur = table.Column<int>(type: "INTEGER", nullable: false),
                    ParcaId = table.Column<int>(type: "INTEGER", nullable: true),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Adet = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    KdvOrani = table.Column<decimal>(type: "decimal(18,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IsEmriKalemleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IsEmriKalemleri_IsEmirleri_IsEmriId",
                        column: x => x.IsEmriId,
                        principalTable: "IsEmirleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IsEmriKalemleri_Parcalar_ParcaId",
                        column: x => x.ParcaId,
                        principalTable: "Parcalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Araclar_MusteriId",
                table: "Araclar",
                column: "MusteriId");

            migrationBuilder.CreateIndex(
                name: "IX_Araclar_TenantId",
                table: "Araclar",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Araclar_TenantId_Plaka",
                table: "Araclar",
                columns: new[] { "TenantId", "Plaka" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_AracId",
                table: "IsEmirleri",
                column: "AracId");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_AtananPersonelId",
                table: "IsEmirleri",
                column: "AtananPersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_ServisKanaliId",
                table: "IsEmirleri",
                column: "ServisKanaliId");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_TenantId",
                table: "IsEmirleri",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_TenantId_GelisTarihi",
                table: "IsEmirleri",
                columns: new[] { "TenantId", "GelisTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_TenantId_PlanlananBaslangic",
                table: "IsEmirleri",
                columns: new[] { "TenantId", "PlanlananBaslangic" });

            migrationBuilder.CreateIndex(
                name: "IX_IsEmriKalemleri_IsEmriId",
                table: "IsEmriKalemleri",
                column: "IsEmriId");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmriKalemleri_ParcaId",
                table: "IsEmriKalemleri",
                column: "ParcaId");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmriKalemleri_TenantId",
                table: "IsEmriKalemleri",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Musteriler_TenantId",
                table: "Musteriler",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Parcalar_TenantId",
                table: "Parcalar",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Parcalar_TenantId_StokKodu",
                table: "Parcalar",
                columns: new[] { "TenantId", "StokKodu" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_TenantId",
                table: "Personeller",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ServisKanallari_TenantId",
                table: "ServisKanallari",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "IsEmriKalemleri");

            migrationBuilder.DropTable(
                name: "Isletmeler");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "IsEmirleri");

            migrationBuilder.DropTable(
                name: "Parcalar");

            migrationBuilder.DropTable(
                name: "Araclar");

            migrationBuilder.DropTable(
                name: "Personeller");

            migrationBuilder.DropTable(
                name: "ServisKanallari");

            migrationBuilder.DropTable(
                name: "Musteriler");
        }
    }
}
