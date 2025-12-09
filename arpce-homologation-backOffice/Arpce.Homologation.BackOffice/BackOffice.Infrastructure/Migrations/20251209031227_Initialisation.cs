using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackOffice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initialisation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "adminAccess",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Application = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Groupe = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Page = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Inactif = table.Column<byte>(type: "tinyint", nullable: true),
                    Ajouter = table.Column<byte>(type: "tinyint", nullable: true),
                    Valider = table.Column<byte>(type: "tinyint", nullable: true),
                    Supprimer = table.Column<byte>(type: "tinyint", nullable: true),
                    Imprimer = table.Column<byte>(type: "tinyint", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adminAccess", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "adminConnexions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Utilisateur = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    DateConnexion = table.Column<long>(type: "bigint", nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adminConnexions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "adminEvenementsTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adminEvenementsTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JournalActivation = table.Column<byte>(type: "tinyint", nullable: true),
                    EventConnexion = table.Column<byte>(type: "tinyint", nullable: true),
                    EventOuverturePage = table.Column<byte>(type: "tinyint", nullable: true),
                    EventChangementMotPasse = table.Column<byte>(type: "tinyint", nullable: true),
                    EventModificationDonnees = table.Column<byte>(type: "tinyint", nullable: true),
                    EventSuppressionDonnees = table.Column<byte>(type: "tinyint", nullable: true),
                    EventImpression = table.Column<byte>(type: "tinyint", nullable: true),
                    EventValidation = table.Column<byte>(type: "tinyint", nullable: true),
                    JournalLimitation = table.Column<byte>(type: "tinyint", nullable: true),
                    JournalTypeLimitation = table.Column<byte>(type: "tinyint", nullable: true),
                    JournalDureeLimitation = table.Column<int>(type: "int", nullable: true),
                    JournalTypeDureeLimitation = table.Column<byte>(type: "tinyint", nullable: true),
                    JournalTailleLimitation = table.Column<int>(type: "int", nullable: true),
                    LDAPAuthentificationActivation = table.Column<byte>(type: "tinyint", nullable: true),
                    LDAPAuthentificationNomDomaine = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    LDAPCreationAutoActivation = table.Column<byte>(type: "tinyint", nullable: true),
                    LDAPCreationAutoNomServeur = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    LDAPCreationAutoCompte = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    LDAPCreationAutoPassword = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    ReplicationActivation = table.Column<byte>(type: "tinyint", nullable: true),
                    ReplicationNomServeur = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    ReplicationCompte = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    ReplicationPassword = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "adminProfils",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Remarques = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adminProfils", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "adminReporting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Application = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Inactif = table.Column<byte>(type: "tinyint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adminReporting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "adminUtilisateurTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adminUtilisateurTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "categoriesEquipements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    TypeEquipement = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TypeClient = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FormuleHomologation = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    QuantiteReference = table.Column<int>(type: "int", nullable: true),
                    TarifEtude = table.Column<decimal>(type: "money", nullable: true),
                    TarifHomologation = table.Column<byte>(type: "tinyint", nullable: true),
                    FraisHomologationParLot = table.Column<byte>(type: "tinyint", nullable: true),
                    FraisHomologationQuantiteParLot = table.Column<int>(type: "int", nullable: true),
                    TarifControle = table.Column<decimal>(type: "money", nullable: true),
                    Remarques = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoriesEquipements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    RaisonSociale = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    RegistreCommerce = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    MotPasse = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ChangementMotPasse = table.Column<byte>(type: "tinyint", nullable: true),
                    Desactive = table.Column<byte>(type: "tinyint", nullable: true),
                    ContactNom = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    ContactTelephone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ContactFonction = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Adresse = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Bp = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Ville = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Pays = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Remarques = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TypeClient = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Entreprise"),
                    NiveauValidation = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    VerificationCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    VerificationTokenExpiry = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "modesReglements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    MobileBanking = table.Column<byte>(type: "tinyint", nullable: false),
                    Remarques = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modesReglements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "motifsRejets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Remarques = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_motifsRejets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "propositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_propositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "statuts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statuts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "adminProfilsAcces",
                columns: table => new
                {
                    IdProfil = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdAccess = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ajouter = table.Column<byte>(type: "tinyint", nullable: true),
                    Valider = table.Column<byte>(type: "tinyint", nullable: true),
                    Supprimer = table.Column<byte>(type: "tinyint", nullable: true),
                    Imprimer = table.Column<byte>(type: "tinyint", nullable: true),
                    AdminProfilsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adminProfilsAcces", x => new { x.IdProfil, x.IdAccess });
                    table.ForeignKey(
                        name: "FK_adminProfilsAcces_adminAccess_IdAccess",
                        column: x => x.IdAccess,
                        principalTable: "adminAccess",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_adminProfilsAcces_adminProfils_AdminProfilsId",
                        column: x => x.AdminProfilsId,
                        principalTable: "adminProfils",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_adminProfilsAcces_adminProfils_IdProfil",
                        column: x => x.IdProfil,
                        principalTable: "adminProfils",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "adminProfilsUtilisateursLDAP",
                columns: table => new
                {
                    Utilisateur = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    IdProfil = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminProfilsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adminProfilsUtilisateursLDAP", x => new { x.Utilisateur, x.IdProfil });
                    table.ForeignKey(
                        name: "FK_adminProfilsUtilisateursLDAP_adminProfils_AdminProfilsId",
                        column: x => x.AdminProfilsId,
                        principalTable: "adminProfils",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_adminProfilsUtilisateursLDAP_adminProfils_IdProfil",
                        column: x => x.IdProfil,
                        principalTable: "adminProfils",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminUtilisateurs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdProfil = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdUtilisateurType = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Compte = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prenoms = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    MotPasse = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ChangementMotPasse = table.Column<bool>(type: "bit", nullable: false),
                    Desactive = table.Column<bool>(type: "bit", nullable: false),
                    Remarques = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DerniereConnexion = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<long>(type: "bigint", nullable: true),
                    AdminProfilsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUtilisateurs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminUtilisateurs_adminProfils_AdminProfilsId",
                        column: x => x.AdminProfilsId,
                        principalTable: "adminProfils",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AdminUtilisateurs_adminProfils_IdProfil",
                        column: x => x.IdProfil,
                        principalTable: "adminProfils",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AdminUtilisateurs_adminUtilisateurTypes_IdUtilisateurType",
                        column: x => x.IdUtilisateurType,
                        principalTable: "adminUtilisateurTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dossiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdClient = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdStatut = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdModeReglement = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DateOuverture = table.Column<long>(type: "bigint", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    IdAgentInstructeur = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dossiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dossiers_AdminUtilisateurs_IdAgentInstructeur",
                        column: x => x.IdAgentInstructeur,
                        principalTable: "AdminUtilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dossiers_clients_IdClient",
                        column: x => x.IdClient,
                        principalTable: "clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dossiers_modesReglements_IdModeReglement",
                        column: x => x.IdModeReglement,
                        principalTable: "modesReglements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dossiers_statuts_IdStatut",
                        column: x => x.IdStatut,
                        principalTable: "statuts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "adminJournal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdEvenementType = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Application = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    AdresseIP = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Utilisateur = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    DateEvenement = table.Column<long>(type: "bigint", nullable: false),
                    Page = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IdDossier = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DossierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adminJournal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_adminJournal_adminEvenementsTypes_IdEvenementType",
                        column: x => x.IdEvenementType,
                        principalTable: "adminEvenementsTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_adminJournal_dossiers_DossierId",
                        column: x => x.DossierId,
                        principalTable: "dossiers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "commentaires",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDossier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateCommentaire = table.Column<long>(type: "bigint", nullable: false),
                    commentaire = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    NomInstructeur = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Proposition = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commentaires", x => x.Id);
                    table.ForeignKey(
                        name: "FK_commentaires_dossiers_IdDossier",
                        column: x => x.IdDossier,
                        principalTable: "dossiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "demandes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDossier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdCategorie = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdMotifRejet = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdProposition = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NumeroDemande = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    Equipement = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Modele = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Marque = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Fabricant = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    QuantiteEquipements = table.Column<int>(type: "int", nullable: true),
                    ContactNom = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    PrixUnitaire = table.Column<decimal>(type: "money", nullable: true),
                    EstHomologable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Remise = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_demandes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_demandes_categoriesEquipements_IdCategorie",
                        column: x => x.IdCategorie,
                        principalTable: "categoriesEquipements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_demandes_dossiers_IdDossier",
                        column: x => x.IdDossier,
                        principalTable: "dossiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_demandes_motifsRejets_IdMotifRejet",
                        column: x => x.IdMotifRejet,
                        principalTable: "motifsRejets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_demandes_propositions_IdProposition",
                        column: x => x.IdProposition,
                        principalTable: "propositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "devis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDossier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<long>(type: "bigint", nullable: false),
                    MontantEtude = table.Column<decimal>(type: "money", nullable: false),
                    MontantHomologation = table.Column<decimal>(type: "money", nullable: true),
                    MontantControle = table.Column<decimal>(type: "money", nullable: true),
                    PaiementOk = table.Column<byte>(type: "tinyint", nullable: true),
                    PaiementMobileId = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_devis_dossiers_IdDossier",
                        column: x => x.IdDossier,
                        principalTable: "dossiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "documentsDossiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDossier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Type = table.Column<byte>(type: "tinyint", nullable: true),
                    Donnees = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentsDossiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_documentsDossiers_dossiers_IdDossier",
                        column: x => x.IdDossier,
                        principalTable: "dossiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attestations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDemande = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateDelivrance = table.Column<long>(type: "bigint", nullable: false),
                    DateExpiration = table.Column<long>(type: "bigint", nullable: false),
                    Donnees = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attestations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_attestations_demandes_IdDemande",
                        column: x => x.IdDemande,
                        principalTable: "demandes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "documentsDemandes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDemande = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Donnees = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentsDemandes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_documentsDemandes_demandes_IdDemande",
                        column: x => x.IdDemande,
                        principalTable: "demandes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "adminEvenementsTypes",
                columns: new[] { "Id", "Code", "Libelle" },
                values: new object[,]
                {
                    { new Guid("2955437d-103c-44bb-a47e-ad59b8637450"), "MODIFICATION", "Connexion utilisateur" },
                    { new Guid("5b717876-c345-4399-9adb-85230c9f6deb"), "COMMUNICATION", "Envoi de communication" },
                    { new Guid("5e55129d-98e8-43d5-b90a-8caad34f4fa3"), "VALIDATION", "Validation de processus" },
                    { new Guid("5e6fb52f-febb-495f-b9b5-5198b9fec121"), "SECURITE", "Action de sécurité" },
                    { new Guid("8a749a85-a248-4671-b6cb-5a399e7b92b1"), "CREATION", "Création de données" },
                    { new Guid("a80eabd9-f0ea-4d58-b3bd-5aeb0f6ac9b4"), "SUPPRESSION", "Suppression de données" },
                    { new Guid("b6ea1763-7045-4aa6-8d2d-2bfdb8aadd43"), "ATTRIBUTION", "Attribution de droits/profils" },
                    { new Guid("cbb297fa-56e3-44fe-9c79-7fc07d632e47"), "CONNEXION", "Modification" },
                    { new Guid("ec2c74ec-e9fd-41b7-b960-bb9013c4df85"), "MODIFICATION", "Modification de données" },
                    { new Guid("f8499e5b-3bf6-4fb2-b2f3-a15d569a8cfe"), "QUALIFICATION", "Qualification de données" }
                });

            migrationBuilder.InsertData(
                table: "adminUtilisateurTypes",
                columns: new[] { "Id", "Libelle" },
                values: new object[,]
                {
                    { new Guid("5cdd3f85-4ea4-44ff-8efc-0d1fa17b5362"), "Auditeur" },
                    { new Guid("7e5b7d94-4f5d-4eff-9983-c8f846d3cee6"), "Administrateur" },
                    { new Guid("c58fe069-9f47-49df-97b4-0bc2c6a82d57"), "Utilisateur Standard" }
                });

            migrationBuilder.InsertData(
                table: "modesReglements",
                columns: new[] { "Id", "Code", "DateCreation", "DateModification", "Libelle", "MobileBanking", "Remarques", "UtilisateurCreation", "UtilisateurModification" },
                values: new object[,]
                {
                    { new Guid("5cdf7e8e-024e-47a7-a864-f698baa45c26"), "Virement", null, null, "Virement bancaire", (byte)0, null, null, null },
                    { new Guid("813dbf7d-3a12-4ec7-8b69-a0dc63d20013"), "Especes", null, null, "Espèces", (byte)0, null, null, null },
                    { new Guid("8ef5fcaa-25b3-466b-89a8-9606dd9f2d9b"), "Cheque", null, null, "Chèque", (byte)0, null, null, null },
                    { new Guid("bc12430d-7126-4c06-a424-e844570c0d71"), "MobileBanking", null, null, "Paiement mobile", (byte)1, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "statuts",
                columns: new[] { "Id", "Code", "Libelle" },
                values: new object[,]
                {
                    { new Guid("153f2f2f-3dba-4d7b-8b04-b0694fe56b84"), "InstructionApprouve", "Instruction Approuvée" },
                    { new Guid("29d70d7e-e979-4662-bf5c-840f808cff31"), "DossierPayer", "Paiement effectué" },
                    { new Guid("39a78829-cb4e-450d-a8f4-23ea0dd97518"), "DevisValide", "Devis validé par client" },
                    { new Guid("4928f3cd-d765-4df8-b8f4-28c158568f4c"), "DevisCreer", "Devis créé" },
                    { new Guid("59f06244-7a09-4217-a180-d2d69846036e"), "DevisEmit", "Devis émis" },
                    { new Guid("6b32d0f1-e83d-4902-b7be-d8e03e330653"), "RefusDossier", "Refus de la demande" },
                    { new Guid("6b7ae6ac-a6a0-481f-8876-0c03fb5011b4"), "DevisValideSC", "Devis validé par Chef Service" },
                    { new Guid("6dc44468-4905-4126-9b35-426fb9ac1c02"), "DevisValideTr", "Devis validé par Trésorerie" },
                    { new Guid("7b0e1093-6f07-400e-8314-4e50c908e2bd"), "ApprobationInstruction", "Envoyé pour approbation" },
                    { new Guid("8f58b0e3-8be4-4fbc-a03e-232137c008f9"), "Instruction", "En cours d'instruction" },
                    { new Guid("b358fcd4-4b06-45e5-94ef-4533635f9ca7"), "PaiementExpirer", "Paiement expiré" },
                    { new Guid("c185544c-129b-4e7a-901f-76bfb6eaaa93"), "DossierSigner", "Attestation signée" },
                    { new Guid("c6cde884-a9b4-4344-969d-00e2a2ca4466"), "NouveauDossier", "Nouvelle demande" },
                    { new Guid("ca74bf48-e5e6-4c8e-9f21-f53e09f5fdaf"), "DossierSignature", "Attestation en signature" },
                    { new Guid("ee4e2878-23c5-4188-918c-3e3bae271b3e"), "DevisPaiement", "En attente de paiement" },
                    { new Guid("f638942a-9601-4bbf-9f63-90c23d019a8e"), "DevisRefuser", "Devis refusé par client" },
                    { new Guid("f7446050-3bcd-4ab2-b203-16cc37e6419b"), "PaiementRejete", "Paiement non accepté" }
                });

            migrationBuilder.InsertData(
                table: "AdminUtilisateurs",
                columns: new[] { "Id", "AdminProfilsId", "ChangementMotPasse", "Compte", "DateCreation", "DateModification", "DerniereConnexion", "Desactive", "IdProfil", "IdUtilisateurType", "MotPasse", "Nom", "Prenoms", "Remarques", "UtilisateurCreation", "UtilisateurModification" },
                values: new object[] { new Guid("09cd7f49-c536-4f7c-9f8a-c77442d071c3"), null, true, "admin", 1765249942993L, null, null, false, null, new Guid("7e5b7d94-4f5d-4eff-9983-c8f846d3cee6"), "$2a$11$9kpMlakEzMzwyUUGazvE1u7OZs7KShBxlF3uhn.lLXoyTlNCcSZ7C", "Administrateur", "ARPCE", null, "SYSTEM_SEED", null });

            migrationBuilder.CreateIndex(
                name: "IX_adminJournal_DossierId",
                table: "adminJournal",
                column: "DossierId");

            migrationBuilder.CreateIndex(
                name: "IX_adminJournal_IdEvenementType",
                table: "adminJournal",
                column: "IdEvenementType");

            migrationBuilder.CreateIndex(
                name: "IX_adminProfilsAcces_AdminProfilsId",
                table: "adminProfilsAcces",
                column: "AdminProfilsId");

            migrationBuilder.CreateIndex(
                name: "IX_adminProfilsAcces_IdAccess",
                table: "adminProfilsAcces",
                column: "IdAccess");

            migrationBuilder.CreateIndex(
                name: "IX_adminProfilsUtilisateursLDAP_AdminProfilsId",
                table: "adminProfilsUtilisateursLDAP",
                column: "AdminProfilsId");

            migrationBuilder.CreateIndex(
                name: "IX_adminProfilsUtilisateursLDAP_IdProfil",
                table: "adminProfilsUtilisateursLDAP",
                column: "IdProfil");

            migrationBuilder.CreateIndex(
                name: "IX_AdminUtilisateurs_AdminProfilsId",
                table: "AdminUtilisateurs",
                column: "AdminProfilsId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminUtilisateurs_Compte",
                table: "AdminUtilisateurs",
                column: "Compte",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminUtilisateurs_IdProfil",
                table: "AdminUtilisateurs",
                column: "IdProfil");

            migrationBuilder.CreateIndex(
                name: "IX_AdminUtilisateurs_IdUtilisateurType",
                table: "AdminUtilisateurs",
                column: "IdUtilisateurType");

            migrationBuilder.CreateIndex(
                name: "IX_attestations_IdDemande",
                table: "attestations",
                column: "IdDemande");

            migrationBuilder.CreateIndex(
                name: "IX_commentaires_IdDossier",
                table: "commentaires",
                column: "IdDossier");

            migrationBuilder.CreateIndex(
                name: "IX_demandes_IdCategorie",
                table: "demandes",
                column: "IdCategorie");

            migrationBuilder.CreateIndex(
                name: "IX_demandes_IdDossier",
                table: "demandes",
                column: "IdDossier");

            migrationBuilder.CreateIndex(
                name: "IX_demandes_IdMotifRejet",
                table: "demandes",
                column: "IdMotifRejet");

            migrationBuilder.CreateIndex(
                name: "IX_demandes_IdProposition",
                table: "demandes",
                column: "IdProposition");

            migrationBuilder.CreateIndex(
                name: "IX_devis_IdDossier",
                table: "devis",
                column: "IdDossier");

            migrationBuilder.CreateIndex(
                name: "IX_documentsDemandes_IdDemande",
                table: "documentsDemandes",
                column: "IdDemande");

            migrationBuilder.CreateIndex(
                name: "IX_documentsDossiers_IdDossier",
                table: "documentsDossiers",
                column: "IdDossier");

            migrationBuilder.CreateIndex(
                name: "IX_dossiers_IdAgentInstructeur",
                table: "dossiers",
                column: "IdAgentInstructeur");

            migrationBuilder.CreateIndex(
                name: "IX_dossiers_IdClient",
                table: "dossiers",
                column: "IdClient");

            migrationBuilder.CreateIndex(
                name: "IX_dossiers_IdModeReglement",
                table: "dossiers",
                column: "IdModeReglement");

            migrationBuilder.CreateIndex(
                name: "IX_dossiers_IdStatut",
                table: "dossiers",
                column: "IdStatut");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adminConnexions");

            migrationBuilder.DropTable(
                name: "adminJournal");

            migrationBuilder.DropTable(
                name: "AdminOptions");

            migrationBuilder.DropTable(
                name: "adminProfilsAcces");

            migrationBuilder.DropTable(
                name: "adminProfilsUtilisateursLDAP");

            migrationBuilder.DropTable(
                name: "adminReporting");

            migrationBuilder.DropTable(
                name: "attestations");

            migrationBuilder.DropTable(
                name: "commentaires");

            migrationBuilder.DropTable(
                name: "devis");

            migrationBuilder.DropTable(
                name: "documentsDemandes");

            migrationBuilder.DropTable(
                name: "documentsDossiers");

            migrationBuilder.DropTable(
                name: "adminEvenementsTypes");

            migrationBuilder.DropTable(
                name: "adminAccess");

            migrationBuilder.DropTable(
                name: "demandes");

            migrationBuilder.DropTable(
                name: "categoriesEquipements");

            migrationBuilder.DropTable(
                name: "dossiers");

            migrationBuilder.DropTable(
                name: "motifsRejets");

            migrationBuilder.DropTable(
                name: "propositions");

            migrationBuilder.DropTable(
                name: "AdminUtilisateurs");

            migrationBuilder.DropTable(
                name: "clients");

            migrationBuilder.DropTable(
                name: "modesReglements");

            migrationBuilder.DropTable(
                name: "statuts");

            migrationBuilder.DropTable(
                name: "adminProfils");

            migrationBuilder.DropTable(
                name: "adminUtilisateurTypes");
        }
    }
}
