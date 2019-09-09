using Microsoft.EntityFrameworkCore.Migrations;

namespace Domain.Migrations
{
    public partial class SecondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "QRCodeValue",
                table: "Neighbors");

            migrationBuilder.AddColumn<string>(
                name: "BarcodeValue",
                table: "Updates",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasBarcode",
                table: "Updates",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "TotalSpent",
                table: "Subscriptions",
                nullable: false,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarcodeValue",
                table: "Updates");

            migrationBuilder.DropColumn(
                name: "HasBarcode",
                table: "Updates");

            migrationBuilder.AlterColumn<double>(
                name: "TotalSpent",
                table: "Subscriptions",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Subscriptions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "QRCodeValue",
                table: "Neighbors",
                nullable: true);
        }
    }
}
