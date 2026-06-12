using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIOutfitStylist.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    PurchaseLink = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ProductImage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Retailer = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    ClothingSize = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    PreferredStyle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    BudgetMin = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    BudgetMax = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    BlobName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    BlobUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPhotos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "PhotoAnalysis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserPhotoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BodyType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    SkinTone = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Style = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    RecommendedColorsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecommendationsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecommendedCategoriesJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RawAiResponseJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoAnalysis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoAnalysis_UserPhotos_UserPhotoId",
                        column: x => x.UserPhotoId,
                        principalTable: "UserPhotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Outfits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhotoAnalysisId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Occasion = table.Column<int>(type: "int", nullable: false),
                    Budget = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Weather = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    StylePreference = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    EstimatedCost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    StylingExplanation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    GeneratedImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outfits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Outfits_PhotoAnalysis_PhotoAnalysisId",
                        column: x => x.PhotoAnalysisId,
                        principalTable: "PhotoAnalysis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Outfits_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "OutfitItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OutfitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EstimatedPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutfitItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutfitItems_Outfits_OutfitId",
                        column: x => x.OutfitId,
                        principalTable: "Outfits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutfitItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SavedOutfits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OutfitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedOutfits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedOutfits_Outfits_OutfitId",
                        column: x => x.OutfitId,
                        principalTable: "Outfits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SavedOutfits_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutfitItems_OutfitId",
                table: "OutfitItems",
                column: "OutfitId");

            migrationBuilder.CreateIndex(
                name: "IX_OutfitItems_ProductId",
                table: "OutfitItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Outfits_PhotoAnalysisId",
                table: "Outfits",
                column: "PhotoAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_Outfits_UserId",
                table: "Outfits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoAnalysis_UserPhotoId",
                table: "PhotoAnalysis",
                column: "UserPhotoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedOutfits_OutfitId",
                table: "SavedOutfits",
                column: "OutfitId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedOutfits_UserId_OutfitId",
                table: "SavedOutfits",
                columns: new[] { "UserId", "OutfitId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPhotos_UserId",
                table: "UserPhotos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutfitItems");

            migrationBuilder.DropTable(
                name: "SavedOutfits");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Outfits");

            migrationBuilder.DropTable(
                name: "PhotoAnalysis");

            migrationBuilder.DropTable(
                name: "UserPhotos");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
