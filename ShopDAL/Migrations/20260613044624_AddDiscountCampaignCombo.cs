using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopDAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountCampaignCombo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscountCampaignProduct_DiscountCampaign_DiscountCampaignId",
                table: "DiscountCampaignProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscountCampaignProduct_FoodItems_FoodItemId",
                table: "DiscountCampaignProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiscountCampaignProduct",
                table: "DiscountCampaignProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiscountCampaign",
                table: "DiscountCampaign");

            migrationBuilder.RenameTable(
                name: "DiscountCampaignProduct",
                newName: "DiscountCampaignProducts");

            migrationBuilder.RenameTable(
                name: "DiscountCampaign",
                newName: "DiscountCampaigns");

            migrationBuilder.RenameIndex(
                name: "IX_DiscountCampaignProduct_FoodItemId",
                table: "DiscountCampaignProducts",
                newName: "IX_DiscountCampaignProducts_FoodItemId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscountCampaignProduct_DiscountCampaignId",
                table: "DiscountCampaignProducts",
                newName: "IX_DiscountCampaignProducts_DiscountCampaignId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiscountCampaignProducts",
                table: "DiscountCampaignProducts",
                column: "DiscountCampaignProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiscountCampaigns",
                table: "DiscountCampaigns",
                column: "DiscountCampaignId");

            migrationBuilder.CreateTable(
                name: "DiscountCampaignCombos",
                columns: table => new
                {
                    DiscountCampaignComboId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscountCampaignId = table.Column<int>(type: "int", nullable: false),
                    ComboId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCampaignCombos", x => x.DiscountCampaignComboId);
                    table.ForeignKey(
                        name: "FK_DiscountCampaignCombos_Combos_ComboId",
                        column: x => x.ComboId,
                        principalTable: "Combos",
                        principalColumn: "ComboId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscountCampaignCombos_DiscountCampaigns_DiscountCampaignId",
                        column: x => x.DiscountCampaignId,
                        principalTable: "DiscountCampaigns",
                        principalColumn: "DiscountCampaignId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCampaignCombos_ComboId",
                table: "DiscountCampaignCombos",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCampaignCombos_DiscountCampaignId",
                table: "DiscountCampaignCombos",
                column: "DiscountCampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountCampaignProducts_DiscountCampaigns_DiscountCampaignId",
                table: "DiscountCampaignProducts",
                column: "DiscountCampaignId",
                principalTable: "DiscountCampaigns",
                principalColumn: "DiscountCampaignId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountCampaignProducts_FoodItems_FoodItemId",
                table: "DiscountCampaignProducts",
                column: "FoodItemId",
                principalTable: "FoodItems",
                principalColumn: "FoodItemId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscountCampaignProducts_DiscountCampaigns_DiscountCampaignId",
                table: "DiscountCampaignProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscountCampaignProducts_FoodItems_FoodItemId",
                table: "DiscountCampaignProducts");

            migrationBuilder.DropTable(
                name: "DiscountCampaignCombos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiscountCampaigns",
                table: "DiscountCampaigns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiscountCampaignProducts",
                table: "DiscountCampaignProducts");

            migrationBuilder.RenameTable(
                name: "DiscountCampaigns",
                newName: "DiscountCampaign");

            migrationBuilder.RenameTable(
                name: "DiscountCampaignProducts",
                newName: "DiscountCampaignProduct");

            migrationBuilder.RenameIndex(
                name: "IX_DiscountCampaignProducts_FoodItemId",
                table: "DiscountCampaignProduct",
                newName: "IX_DiscountCampaignProduct_FoodItemId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscountCampaignProducts_DiscountCampaignId",
                table: "DiscountCampaignProduct",
                newName: "IX_DiscountCampaignProduct_DiscountCampaignId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiscountCampaign",
                table: "DiscountCampaign",
                column: "DiscountCampaignId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiscountCampaignProduct",
                table: "DiscountCampaignProduct",
                column: "DiscountCampaignProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountCampaignProduct_DiscountCampaign_DiscountCampaignId",
                table: "DiscountCampaignProduct",
                column: "DiscountCampaignId",
                principalTable: "DiscountCampaign",
                principalColumn: "DiscountCampaignId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountCampaignProduct_FoodItems_FoodItemId",
                table: "DiscountCampaignProduct",
                column: "FoodItemId",
                principalTable: "FoodItems",
                principalColumn: "FoodItemId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
