using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mvc.Identity101.Migrations
{
    /// <inheritdoc />
    public partial class imgpath1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "imgPath",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imgPath",
                table: "AspNetUsers");
        }
    }
}
