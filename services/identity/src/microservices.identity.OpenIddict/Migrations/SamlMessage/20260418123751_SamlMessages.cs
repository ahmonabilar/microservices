using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace microservices.identity.Migrations.SamlMessage
{
    /// <inheritdoc />
    public partial class SamlMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpenIddctSamlMessages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EntityId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", maxLength: 50000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenIddctSamlMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddctSamlMessages_CreationTime",
                table: "OpenIddctSamlMessages",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddctSamlMessages_Expiration",
                table: "OpenIddctSamlMessages",
                column: "Expiration");

            migrationBuilder.CreateIndex(
                name: "IX_OpenIddctSamlMessages_RequestId",
                table: "OpenIddctSamlMessages",
                column: "RequestId",
                unique: true,
                filter: "[RequestId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpenIddctSamlMessages");
        }
    }
}
