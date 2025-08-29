using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GovElec.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    Equipe = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordSalt = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Demandes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Projet = table.Column<string>(type: "TEXT", nullable: false),
                    DemandeurId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Batiment = table.Column<string>(type: "TEXT", nullable: false),
                    Statut = table.Column<string>(type: "TEXT", nullable: false),
                    StatusTechnicien = table.Column<string>(type: "TEXT", nullable: false),
                    Equipement = table.Column<string>(type: "TEXT", nullable: false),
                    TypeDemande = table.Column<string>(type: "TEXT", nullable: false),
                    Priorite = table.Column<string>(type: "TEXT", nullable: false),
                    Contexte = table.Column<string>(type: "TEXT", nullable: false),
                    VenantDe = table.Column<string>(type: "TEXT", nullable: false),
                    Disjoncteur = table.Column<string>(type: "TEXT", nullable: false),
                    Tension = table.Column<string>(type: "TEXT", nullable: false),
                    Regime = table.Column<string>(type: "TEXT", nullable: false),
                    TypeReseau = table.Column<string>(type: "TEXT", nullable: false),
                    Puissance = table.Column<float>(type: "REAL", nullable: false),
                    CosPhi = table.Column<float>(type: "REAL", nullable: false),
                    Ks = table.Column<float>(type: "REAL", nullable: false),
                    Ku = table.Column<float>(type: "REAL", nullable: false),
                    CommentaireTechnicien = table.Column<string>(type: "TEXT", nullable: false),
                    CommentaireAdministrateur = table.Column<string>(type: "TEXT", nullable: false),
                    CommentaireUtilisateur = table.Column<string>(type: "TEXT", nullable: false),
                    TechnicienId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DisjoncteurPrincipal = table.Column<string>(type: "TEXT", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateDemandeApprobation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateReponseAttendue = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateReponse = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateReponseFinale = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateDebutTravaux = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateCloture = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demandes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Demandes_Users_DemandeurId",
                        column: x => x.DemandeurId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Demandes_Users_TechnicienId",
                        column: x => x.TechnicienId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Demandes_DemandeurId",
                table: "Demandes",
                column: "DemandeurId");

            migrationBuilder.CreateIndex(
                name: "IX_Demandes_TechnicienId",
                table: "Demandes",
                column: "TechnicienId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Demandes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
