using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlinePizzaDelivery_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateTablePizza : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Pizza");

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "Pizza",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "Pizza");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Pizza",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
