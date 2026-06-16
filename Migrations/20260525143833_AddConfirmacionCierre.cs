using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HelpDesk.API.Migrations
{
    /// <inheritdoc />
    public partial class AddConfirmacionCierre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaSolicitudCierre",
                table: "Tickets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaConfirmacionCierre",
                table: "Tickets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoRechazo",
                table: "Tickets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CerradoPorUsuarioId",
                table: "Tickets",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HistorialTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TicketId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    FechaAccion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialTickets_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistorialTickets_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CerradoPorUsuarioId",
                table: "Tickets",
                column: "CerradoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialTickets_TicketId",
                table: "HistorialTickets",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialTickets_UsuarioId",
                table: "HistorialTickets",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Usuarios_CerradoPorUsuarioId",
                table: "Tickets",
                column: "CerradoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Usuarios_CerradoPorUsuarioId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "HistorialTickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_CerradoPorUsuarioId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CerradoPorUsuarioId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "MotivoRechazo",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "FechaConfirmacionCierre",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "FechaSolicitudCierre",
                table: "Tickets");
        }
    }
}
