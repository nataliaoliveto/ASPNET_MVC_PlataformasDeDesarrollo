using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Final.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alojamiento",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    aCodigo = table.Column<int>(type: "int", nullable: false),
                    aCiudad = table.Column<string>(type: "varchar(100)", nullable: false),
                    aBarrio = table.Column<string>(type: "varchar(100)", nullable: false),
                    aEstrellas = table.Column<int>(type: "int", nullable: false),
                    aCantPersonas = table.Column<int>(type: "int", nullable: false),
                    aTV = table.Column<bool>(type: "bit", nullable: false),
                    Tipo = table.Column<string>(type: "varchar(20)", nullable: false),
                    cPrecioxDia = table.Column<double>(type: "float", nullable: false),
                    cHabitaciones = table.Column<int>(type: "int", nullable: false),
                    cbanios = table.Column<int>(type: "int", nullable: false),
                    hPrecioxPersona = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alojamiento", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DNI = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    mail = table.Column<string>(type: "varchar(100)", nullable: false),
                    password = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    esADMIN = table.Column<bool>(type: "bit", nullable: false),
                    bloqueado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Reserva",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fDesde = table.Column<DateTime>(type: "datetime", nullable: false),
                    fHasta = table.Column<DateTime>(type: "datetime", nullable: false),
                    precio = table.Column<decimal>(type: "decimal(16,2)", nullable: false),
                    cantPersonas = table.Column<int>(type: "int", nullable: false),
                    propiedadId = table.Column<int>(type: "int", nullable: false),
                    personaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reserva", x => x.id);
                    table.ForeignKey(
                        name: "FK_Reserva_Usuario_personaId",
                        column: x => x.personaId,
                        principalTable: "Usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reserva_Alojamiento_propiedadId",
                        column: x => x.propiedadId,
                        principalTable: "Alojamiento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Alojamiento",
                columns: new[] { "id", "Tipo", "aBarrio", "aCantPersonas", "aCiudad", "aCodigo", "aEstrellas", "aTV", "cHabitaciones", "cPrecioxDia", "cbanios", "hPrecioxPersona" },
                values: new object[,]
                {
                    { 1, "Hotel", "Palermo", 300, "CABA", 1, 4, true, 0, 0.0, 0, 150.0 },
                    { 2, "Hotel", "Palermo", 200, "CABA", 2, 5, true, 0, 0.0, 0, 200.0 },
                    { 3, "Hotel", "Belgrano", 200, "CABA", 3, 4, true, 0, 0.0, 0, 150.0 },
                    { 4, "Hotel", "Retiro", 400, "CABA", 4, 3, true, 0, 0.0, 0, 100.0 },
                    { 5, "Hotel", "San Nicolas", 350, "CABA", 5, 3, true, 0, 0.0, 0, 90.0 },
                    { 6, "Cabania", "La Falda", 5, "Cordoba", 6, 3, true, 2, 50.0, 1, 0.0 },
                    { 7, "Cabania", "La Falda", 10, "Cordoba", 7, 3, true, 4, 50.0, 1, 0.0 },
                    { 8, "Cabania", "Mina Clavero", 6, "Cordoba", 8, 2, false, 4, 30.0, 2, 0.0 },
                    { 9, "Cabania", "Mina Clavero", 8, "Cordoba", 9, 2, true, 3, 35.0, 2, 0.0 },
                    { 10, "Cabania", "Nono", 4, "Cordoba", 10, 2, false, 1, 55.0, 1, 0.0 }
                });

            migrationBuilder.InsertData(
                table: "Usuario",
                columns: new[] { "id", "DNI", "bloqueado", "esADMIN", "mail", "nombre", "password" },
                values: new object[,]
                {
                    { 1, 11111111, false, true, "admin@admin.com", "admin", "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918" },
                    { 2, 12121212, false, false, "luisjose@hotmail.com", "Luis", "9250e222c4c71f0c58d4c54b50a880a312e9f9fed55d5c3aa0b0e860ded99165" },
                    { 3, 13131313, false, false, "pedropablo@hotmail.com", "Pedro", "9250e222c4c71f0c58d4c54b50a880a312e9f9fed55d5c3aa0b0e860ded99165" },
                    { 4, 14141414, false, false, "leoparedes@hotmail.com", "Leo", "9250e222c4c71f0c58d4c54b50a880a312e9f9fed55d5c3aa0b0e860ded99165" },
                    { 5, 22222222, false, false, "juanperez@hotmail.com", "Juan", "9250e222c4c71f0c58d4c54b50a880a312e9f9fed55d5c3aa0b0e860ded99165" },
                    { 6, 33333333, false, true, "juancarlosbatman@hotmail.com", "Batman", "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918" }
                });

            migrationBuilder.InsertData(
                table: "Reserva",
                columns: new[] { "id", "cantPersonas", "fDesde", "fHasta", "personaId", "precio", "propiedadId" },
                values: new object[,]
                {
                    { 2, 4, new DateTime(2021, 11, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2160m, 5 },
                    { 6, 3, new DateTime(2021, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 12, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 6600m, 2 },
                    { 3, 2, new DateTime(2021, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 300m, 6 },
                    { 4, 5, new DateTime(2021, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 4950m, 5 },
                    { 7, 3, new DateTime(2021, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 10, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 930m, 8 },
                    { 1, 3, new DateTime(2021, 12, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 12, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 4200m, 2 },
                    { 5, 2, new DateTime(2021, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 4400m, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_personaId",
                table: "Reserva",
                column: "personaId");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_propiedadId",
                table: "Reserva",
                column: "propiedadId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reserva");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Alojamiento");
        }
    }
}
