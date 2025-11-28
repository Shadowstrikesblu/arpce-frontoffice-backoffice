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
                    Type = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Inactif = table.Column<byte>(type: "tinyint", nullable: true),
                    Ajouter = table.Column<byte>(type: "tinyint", nullable: true),
                    Valider = table.Column<byte>(type: "tinyint", nullable: true),
                    Supprimer = table.Column<byte>(type: "tinyint", nullable: true),
                    Imprimer = table.Column<byte>(type: "tinyint", nullable: true)
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
                    DateConnexion = table.Column<DateTime>(type: "datetime", nullable: false),
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
                    Libelle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
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
                    DateCreation = table.Column<DateTime>(type: "datetime", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime", nullable: true)
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
                    TarifHomologation = table.Column<decimal>(type: "money", nullable: true),
                    TarifHomologationParLot = table.Column<byte>(type: "tinyint", nullable: true),
                    TarifHomologationQuantiteParLot = table.Column<int>(type: "int", nullable: true),
                    TarifControle = table.Column<decimal>(type: "money", nullable: true),
                    Remarques = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime", nullable: true)
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
                    Adresse = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Bp = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Ville = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Pays = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Remarques = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    VerificationCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    VerificationTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime", nullable: true)
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
                    DateCreation = table.Column<DateTime>(type: "datetime", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime", nullable: true)
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
                    DateCreation = table.Column<DateTime>(type: "datetime", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime", nullable: true)
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
                    DateCreation = table.Column<DateTime>(type: "datetime", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime", nullable: true)
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
                name: "adminJournal",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdEvenementType = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Application = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    AdresseIP = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Utilisateur = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    DateEvenement = table.Column<DateTime>(type: "datetime", nullable: false),
                    Page = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
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
                    Imprimer = table.Column<byte>(type: "tinyint", nullable: true)
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
                    IdProfil = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adminProfilsUtilisateursLDAP", x => new { x.Utilisateur, x.IdProfil });
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
                    Compte = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prenoms = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    MotPasse = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ChangementMotPasse = table.Column<bool>(type: "bit", nullable: false),
                    Desactive = table.Column<bool>(type: "bit", nullable: false),
                    DerniereConnexion = table.Column<DateTime>(type: "datetime", nullable: true),
                    Remarques = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdProfil = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProfilId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUtilisateurs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminUtilisateurs_adminProfils_ProfilId",
                        column: x => x.ProfilId,
                        principalTable: "adminProfils",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "dossiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdClient = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdStatut = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdModeReglement = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DateOuverture = table.Column<DateTime>(type: "date", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    IdAgentInstructeur = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime", nullable: true)
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
                name: "commentaires",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDossier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateCommentaire = table.Column<DateTime>(type: "datetime", nullable: false),
                    commentaire = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    NomInstructeur = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Proposition = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime", nullable: true)
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
                    DateCreation = table.Column<DateTime>(type: "datetime", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime", nullable: true)
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
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    MontantEtude = table.Column<decimal>(type: "money", nullable: false),
                    MontantHomologation = table.Column<decimal>(type: "money", nullable: true),
                    MontantControle = table.Column<decimal>(type: "money", nullable: true),
                    PaiementOk = table.Column<byte>(type: "tinyint", nullable: true),
                    PaiementMobileId = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime", nullable: true)
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
                    DateCreation = table.Column<DateTime>(type: "datetime", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime", nullable: true)
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
                    DateDelivrance = table.Column<DateTime>(type: "date", nullable: false),
                    DateExpiration = table.Column<DateTime>(type: "date", nullable: false),
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
                    DateCreation = table.Column<DateTime>(type: "datetime", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime", nullable: true)
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
                table: "AdminUtilisateurs",
                columns: new[] { "Id", "ChangementMotPasse", "Compte", "DateCreation", "DateModification", "DerniereConnexion", "Desactive", "IdProfil", "MotPasse", "Nom", "Prenoms", "ProfilId", "Remarques", "UtilisateurCreation", "UtilisateurModification" },
                values: new object[] { new Guid("0fd0551d-54c7-4b8b-9fd6-649ef57704bb"), true, "admin", null, null, null, false, null, "$2a$11$DNhK2QJHVj9WupwQkawuFuBCEaECCXSblICeGEILTMojcrooVx0pW", "Administrateur", "Système", null, null, null, null });

            migrationBuilder.InsertData(
                table: "modesReglements",
                columns: new[] { "Id", "Code", "DateCreation", "DateModification", "Libelle", "MobileBanking", "Remarques", "UtilisateurCreation", "UtilisateurModification" },
                values: new object[,]
                {
                    { new Guid("107a1592-f98d-4e65-b92b-7a31053bd6bb"), "Especes", null, null, "Espèces", (byte)0, null, null, null },
                    { new Guid("3c039c40-fe5c-4ca8-a4e9-165a2fc252b2"), "MobileBanking", null, null, "Paiement mobile", (byte)1, null, null, null },
                    { new Guid("6b95539b-d298-4a64-85da-3a8de033a7b0"), "Virement", null, null, "Virement bancaire", (byte)0, null, null, null },
                    { new Guid("8aeec79d-3bc0-4720-a3bb-34496a59110d"), "Cheque", null, null, "Chèque", (byte)0, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "statuts",
                columns: new[] { "Id", "Code", "Libelle" },
                values: new object[,]
                {
                    { new Guid("024046ef-dc6c-46db-b1ea-d9120523934d"), "DevisCreer", "Devis créé" },
                    { new Guid("0e5a7133-8bec-4308-bc95-29eb6707b722"), "DossierPayer", "Paiement effectué" },
                    { new Guid("117f534d-662d-4b8a-9cd8-72eb08561905"), "DevisValideTr", "Devis validé par Trésorerie" },
                    { new Guid("16c246cc-2298-4eff-9e71-052542e082ed"), "DossierSigner", "Attestation signée" },
                    { new Guid("1dae9195-9355-438d-8d8b-19dee7e62d2a"), "DossierSignature", "Attestation en signature" },
                    { new Guid("365eb9c9-46ab-4de3-bba8-a6b6f54ba077"), "Instruction", "En cours d'instruction" },
                    { new Guid("3e3c1cf4-dd8a-4651-8a16-fade71efb9a4"), "NouveauDossier", "Nouvelle demande" },
                    { new Guid("804634fb-46de-4da5-89b3-d782d2aca862"), "DevisEmit", "Devis émis" },
                    { new Guid("94a0ab8e-1f0f-43f6-ac12-92b1c022a18b"), "RefusDossier", "Refus de la demande" },
                    { new Guid("acdb910b-2145-45dd-935c-eccbe7d6b6ee"), "PaiementExpirer", "Paiement expiré" },
                    { new Guid("b1ddfcfb-b773-4d8a-8289-ea88555c4561"), "ApprobationInstruction", "Envoyé pour approbation" },
                    { new Guid("be1bfedf-4270-4d14-a762-81a3e943abca"), "DevisRefuser", "Devis refusé par client" },
                    { new Guid("c87929d2-85c2-4179-a4f2-573a4d2b971f"), "InstructionApprouve", "Instruction Approuvée" },
                    { new Guid("cee48ba6-02b4-4072-b109-a98ac42570cd"), "DevisValideSC", "Devis validé par Chef Service" },
                    { new Guid("e913f6b8-3066-429a-9a99-81c2f55218b0"), "PaiementRejete", "Paiement non accepté" },
                    { new Guid("e9322554-6bd5-4d15-b8f9-19155e3cdfb7"), "DevisValide", "Devis validé par client" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_adminJournal_IdEvenementType",
                table: "adminJournal",
                column: "IdEvenementType");

            migrationBuilder.CreateIndex(
                name: "IX_adminProfilsAcces_IdAccess",
                table: "adminProfilsAcces",
                column: "IdAccess");

            migrationBuilder.CreateIndex(
                name: "IX_adminProfilsUtilisateursLDAP_IdProfil",
                table: "adminProfilsUtilisateursLDAP",
                column: "IdProfil");

            migrationBuilder.CreateIndex(
                name: "IX_AdminUtilisateurs_Compte",
                table: "AdminUtilisateurs",
                column: "Compte",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminUtilisateurs_ProfilId",
                table: "AdminUtilisateurs",
                column: "ProfilId");

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
                name: "adminUtilisateurTypes");

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
        }
    }
}
