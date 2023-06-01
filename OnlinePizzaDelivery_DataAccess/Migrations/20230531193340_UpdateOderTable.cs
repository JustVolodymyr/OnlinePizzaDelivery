using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlinePizzaDelivery_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Pizza_ProductId",
                table: "OrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetail_ProductId",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "OrderDetail");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_PizzaId",
                table: "OrderDetail",
                column: "PizzaId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Pizza_PizzaId",
                table: "OrderDetail",
                column: "PizzaId",
                principalTable: "Pizza",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Pizza_PizzaId",
                table: "OrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetail_PizzaId",
                table: "OrderDetail");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "OrderDetail",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ProductId",
                table: "OrderDetail",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Pizza_ProductId",
                table: "OrderDetail",
                column: "ProductId",
                principalTable: "Pizza",
                principalColumn: "Id");
        }
    }
}
