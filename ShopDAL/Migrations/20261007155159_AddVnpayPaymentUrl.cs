using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopDAL.Migrations
{
    /// <inheritdoc />
    public partial class AddVnpayPaymentUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentUrl",
                table: "VnpayPaymentAttempts",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentUrl",
                table: "VnpayPaymentAttempts");
        }
    }
}
