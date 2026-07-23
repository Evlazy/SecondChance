using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecondChance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                column: "ConcurrencyStamp",
                value: "1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ba32bf1e-7813-4c91-b3b3-847253b708b7",
                column: "ConcurrencyStamp",
                value: "2b3c4d5e-6f7a-8b9c-0d1e-2f3a4b5c6d7e");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                column: "ConcurrencyStamp",
                value: "e5851f77-d00a-4e94-b7e3-e8e6fbc7f5b6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ba32bf1e-7813-4c91-b3b3-847253b708b7",
                column: "ConcurrencyStamp",
                value: "d7310e54-19f3-4c93-b826-c6383af91b9f");
        }
    }
}
