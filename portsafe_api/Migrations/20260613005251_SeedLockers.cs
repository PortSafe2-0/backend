using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PortSafe.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedLockers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Lockers",
                columns: new[] { "Id", "Code", "CreatedAt", "IsActive", "Location", "Status" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000001"), "A01", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Portaria - Bloco A", 0 },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000002"), "A02", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Portaria - Bloco A", 0 },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000003"), "A03", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Portaria - Bloco A", 0 },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000004"), "B01", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Portaria - Bloco B", 0 },
                    { new Guid("a1b2c3d4-0001-0001-0001-000000000005"), "B02", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Portaria - Bloco B", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Lockers",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0001-0001-0001-000000000001"));

            migrationBuilder.DeleteData(
                table: "Lockers",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0001-0001-0001-000000000002"));

            migrationBuilder.DeleteData(
                table: "Lockers",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0001-0001-0001-000000000003"));

            migrationBuilder.DeleteData(
                table: "Lockers",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0001-0001-0001-000000000004"));

            migrationBuilder.DeleteData(
                table: "Lockers",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-0001-0001-0001-000000000005"));
        }
    }
}
