using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class changedOrderDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Orders",
                newName: "TotalAmountUzs");

            migrationBuilder.RenameColumn(
                name: "SubTotal",
                table: "OrderDetails",
                newName: "SubTotalUzs");

            migrationBuilder.AddColumn<DateTime>(
                name: "StoredAt",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoredAt",
                table: "Products");

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
    }
}
