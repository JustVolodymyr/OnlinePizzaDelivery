using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlinePizzaDelivery_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateTablOrderDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "OrderHeader");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "OrderHeader",
                newName: "HouseNumber");

            migrationBuilder.AddColumn<int>(
                name: "Apartment",
                table: "OrderHeader",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Entrance",
                table: "OrderHeader",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Apartment",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "Entrance",
                table: "OrderHeader");

            migrationBuilder.RenameColumn(
                name: "HouseNumber",
                table: "OrderHeader",
                newName: "State");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "OrderHeader",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
