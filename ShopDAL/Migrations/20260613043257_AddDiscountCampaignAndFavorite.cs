using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopDAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountCampaignAndFavorite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscountCampaign",
                columns: table => new
                {
                    DiscountCampaignId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCampaign", x => x.DiscountCampaignId);
                });

            migrationBuilder.CreateTable(
                name: "DiscountCampaignProduct",
                columns: table => new
                {
                    DiscountCampaignProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscountCampaignId = table.Column<int>(type: "int", nullable: false),
                    FoodItemId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCampaignProduct", x => x.DiscountCampaignProductId);
                    table.ForeignKey(
                        name: "FK_DiscountCampaignProduct_DiscountCampaign_DiscountCampaignId",
                        column: x => x.DiscountCampaignId,
                        principalTable: "DiscountCampaign",
                        principalColumn: "DiscountCampaignId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscountCampaignProduct_FoodItems_FoodItemId",
                        column: x => x.FoodItemId,
                        principalTable: "FoodItems",
                        principalColumn: "FoodItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCampaignProduct_DiscountCampaignId",
                table: "DiscountCampaignProduct",
                column: "DiscountCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCampaignProduct_FoodItemId",
                table: "DiscountCampaignProduct",
                column: "FoodItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscountCampaignProduct");

            migrationBuilder.DropTable(
                name: "DiscountCampaign");
        }
    }
}
