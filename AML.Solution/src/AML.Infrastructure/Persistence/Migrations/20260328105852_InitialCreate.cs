using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AML.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    IntentKey = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ServiceType = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientServices_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAuths",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthType = table.Column<int>(type: "int", nullable: false),
                    ApiKeyHeaderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EncryptedApiKey = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EncryptedUsername = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EncryptedPassword = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EncryptedClientId = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EncryptedClientSecret = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TokenUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Scope = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAuths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceAuths_ClientServices_ClientServiceId",
                        column: x => x.ClientServiceId,
                        principalTable: "ClientServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceEndpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    HttpMethod = table.Column<int>(type: "int", nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceEndpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceEndpoints_ClientServices_ClientServiceId",
                        column: x => x.ClientServiceId,
                        principalTable: "ClientServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceFieldMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceField = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TargetField = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceFieldMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceFieldMappings_ClientServices_ClientServiceId",
                        column: x => x.ClientServiceId,
                        principalTable: "ClientServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceHeaders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HeaderKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HeaderValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsSensitive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceHeaders_ClientServices_ClientServiceId",
                        column: x => x.ClientServiceId,
                        principalTable: "ClientServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    RequestedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: true),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    DurationMs = table.Column<long>(type: "bigint", nullable: false),
                    RequestPayload = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ResponsePayload = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceLogs_ClientServices_ClientServiceId",
                        column: x => x.ClientServiceId,
                        principalTable: "ClientServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Code",
                table: "Clients",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientServices_ClientId_IntentKey",
                table: "ClientServices",
                columns: new[] { "ClientId", "IntentKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAuths_ClientServiceId",
                table: "ServiceAuths",
                column: "ClientServiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEndpoints_ClientServiceId",
                table: "ServiceEndpoints",
                column: "ClientServiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceFieldMappings_ClientServiceId_SourceField_TargetField",
                table: "ServiceFieldMappings",
                columns: new[] { "ClientServiceId", "SourceField", "TargetField" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceHeaders_ClientServiceId_HeaderKey",
                table: "ServiceHeaders",
                columns: new[] { "ClientServiceId", "HeaderKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLogs_ClientServiceId",
                table: "ServiceLogs",
                column: "ClientServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLogs_CorrelationId",
                table: "ServiceLogs",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceLogs_RequestedAtUtc",
                table: "ServiceLogs",
                column: "RequestedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceAuths");

            migrationBuilder.DropTable(
                name: "ServiceEndpoints");

            migrationBuilder.DropTable(
                name: "ServiceFieldMappings");

            migrationBuilder.DropTable(
                name: "ServiceHeaders");

            migrationBuilder.DropTable(
                name: "ServiceLogs");

            migrationBuilder.DropTable(
                name: "ClientServices");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
