using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddedSetTypesSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_SetType_SetTypeId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SetType",
                table: "SetType");

            migrationBuilder.RenameTable(
                name: "SetType",
                newName: "SetTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SetTypes",
                table: "SetTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SetTypes_SetTypeId",
                table: "Products",
                column: "SetTypeId",
                principalTable: "SetTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_SetTypes_SetTypeId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SetTypes",
                table: "SetTypes");

            migrationBuilder.RenameTable(
                name: "SetTypes",
                newName: "SetType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SetType",
                table: "SetType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SetType_SetTypeId",
                table: "Products",
                column: "SetTypeId",
                principalTable: "SetType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
