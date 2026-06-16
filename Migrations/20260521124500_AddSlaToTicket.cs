using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDesk.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSlaToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "estado_sla",
                table: "tickets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_limite_sla",
                table: "tickets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_pausa_sla",
                table: "tickets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "sla_vencido",
                table: "tickets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "tiempo_acumulado_pausa_minutos",
                table: "tickets",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estado_sla",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "fecha_limite_sla",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "fecha_pausa_sla",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "sla_vencido",
                table: "tickets");

            migrationBuilder.DropColumn(
                name: "tiempo_acumulado_pausa_minutos",
                table: "tickets");
        }
    }
}
