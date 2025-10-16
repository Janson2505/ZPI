using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class UseValueObjectsForNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Contacts",
                newName: "LastName_Value");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Contacts",
                newName: "FirstName_Value");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastName_Value",
                table: "Contacts",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "FirstName_Value",
                table: "Contacts",
                newName: "FirstName");
        }
    }
}
