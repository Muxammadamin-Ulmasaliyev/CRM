using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class CartAndChequeChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmountUsd",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SubTotalUsd",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "TotalAmountUzs",
                table: "Orders",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "SubTotalUzs",
                table: "OrderDetails",
                newName: "SubTotal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Orders",
                newName: "TotalAmountUzs");

            migrationBuilder.RenameColumn(
                name: "SubTotal",
                table: "OrderDetails",
                newName: "SubTotalUzs");

            migrationBuilder.AddColumn<double>(
                name: "TotalAmountUsd",
                table: "Orders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SubTotalUsd",
                table: "OrderDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
