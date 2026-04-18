using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace microservices.identity.Migrations.SamlArtifact
{
    /// <inheritdoc />
    public partial class SamlArtifacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SamlArtifacts",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", maxLength: 50000, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SamlArtifacts", x => x.Key);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SamlArtifacts_Expiration",
                table: "SamlArtifacts",
                column: "Expiration");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SamlArtifacts");
        }
    }
}
