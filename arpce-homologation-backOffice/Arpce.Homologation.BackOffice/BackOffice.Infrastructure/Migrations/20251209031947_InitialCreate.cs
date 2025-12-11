using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackOffice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    { new Guid("0189db59-a66c-4b02-b0f8-20293cceed23"), "CONNEXION", "Modification" },
                    { new Guid("11cf420d-fdba-4e5e-ad74-49cc3bf5509a"), "SECURITE", "Action de sécurité" },
                    { new Guid("3f1812fa-0dee-40a2-9f6e-ea55284ab301"), "COMMUNICATION", "Envoi de communication" },
                    { new Guid("531dd82e-4946-4914-be42-490090a51978"), "MODIFICATION", "Connexion utilisateur" },
                    { new Guid("6f62515f-74b5-4719-81c3-b55c9427b1dc"), "MODIFICATION", "Modification de données" },
                    { new Guid("813df7ae-be17-4a53-bfe4-ce1f1389db55"), "VALIDATION", "Validation de processus" },
                    { new Guid("8d3b90ec-23a4-4468-8dc2-0000676f23ca"), "SUPPRESSION", "Suppression de données" },
                    { new Guid("93e1c6b5-ef10-4d9b-b48f-830ec06bebcb"), "ATTRIBUTION", "Attribution de droits/profils" },
                    { new Guid("f62e36c7-203f-41ac-8c7c-af96b404ce0f"), "CREATION", "Création de données" },
                    { new Guid("ff8d7205-35f4-418d-bca6-8253af932cf9"), "QUALIFICATION", "Qualification de données" }
                });

            migrationBuilder.InsertData(
                table: "adminUtilisateurTypes",
                columns: new[] { "Id", "Libelle" },
                values: new object[,]
                {
                    { new Guid("2025a86f-b446-495d-be85-f764c8127544"), "Utilisateur Standard" },
                    { new Guid("471b4cc6-e7e6-406e-b6e7-98dee3c35465"), "Auditeur" },
                    { new Guid("7e5b7d94-4f5d-4eff-9983-c8f846d3cee6"), "Administrateur" }
                });

            migrationBuilder.InsertData(
                table: "modesReglements",
                columns: new[] { "Id", "Code", "DateCreation", "DateModification", "Libelle", "MobileBanking", "Remarques", "UtilisateurCreation", "UtilisateurModification" },
                values: new object[,]
                {
                    { new Guid("2881df95-7d3d-49d0-8045-9b9a73acfd56"), "Virement", null, null, "Virement bancaire", (byte)0, null, null, null },
                    { new Guid("5c6aad65-433b-4841-9a42-cf120bd35fb9"), "Cheque", null, null, "Chèque", (byte)0, null, null, null },
                    { new Guid("d11cde5c-4f61-4be1-8959-0cc856e8cd20"), "MobileBanking", null, null, "Paiement mobile", (byte)1, null, null, null },
                    { new Guid("fdbf6e78-247e-4f9c-9b48-1ec89bb34f45"), "Especes", null, null, "Espèces", (byte)0, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "statuts",
                columns: new[] { "Id", "Code", "Libelle" },
                values: new object[,]
                {
                    { new Guid("13e9b5c7-5c0a-4bee-9634-c088c3a6cba6"), "DevisValideTr", "Devis validé par Trésorerie" },
                    { new Guid("31674d2c-6d97-45ac-9be6-3c60503d75f9"), "DevisValideSC", "Devis validé par Chef Service" },
                    { new Guid("37787f75-3dda-442e-8e5c-3e7f5299726e"), "DevisCreer", "Devis créé" },
                    { new Guid("38e4eff9-d1c2-45c8-83a1-deaaf5c77add"), "PaiementExpirer", "Paiement expiré" },
                    { new Guid("489f5877-a582-4bb8-a501-a5aab5a5de22"), "DossierPayer", "Paiement effectué" },
                    { new Guid("4b7f34a0-9304-45ec-872e-8dc411c3bac5"), "ApprobationInstruction", "Envoyé pour approbation" },
                    { new Guid("57f3c519-1f6f-47f7-91ac-a3fe936b3589"), "DossierSigner", "Attestation signée" },
                    { new Guid("5a927d93-c248-4e1a-8229-e6df492d618e"), "DevisEmit", "Devis émis" },
                    { new Guid("61ad4a09-e5bb-42db-b4ac-219e661269bc"), "Instruction", "En cours d'instruction" },
                    { new Guid("62557f3d-91c3-4b53-979f-5fd8fef587f5"), "RefusDossier", "Refus de la demande" },
                    { new Guid("760c9b49-3614-4121-9da3-ceaa3222dff8"), "DevisPaiement", "En attente de paiement" },
                    { new Guid("7919fa42-5ecb-4311-a438-358ae952d0bb"), "InstructionApprouve", "Instruction Approuvée" },
                    { new Guid("7bbab791-186a-4667-a298-49d689a3e2ce"), "PaiementRejete", "Paiement non accepté" },
                    { new Guid("b529bcc4-e7a5-441a-b41e-799ebe44c7fa"), "DossierSignature", "Attestation en signature" },
                    { new Guid("c4a9ae4f-8421-4362-85ca-daa4ee806748"), "NouveauDossier", "Nouvelle demande" },
                    { new Guid("d7e50ca5-a902-4739-bd3f-84de26e96858"), "DevisRefuser", "Devis refusé par client" },
                    { new Guid("f6242124-552e-456b-9abd-1bb085fd4f85"), "DevisValide", "Devis validé par client" }
                });

            migrationBuilder.InsertData(
                table: "AdminUtilisateurs",
                columns: new[] { "Id", "AdminProfilsId", "ChangementMotPasse", "Compte", "DateCreation", "DateModification", "DerniereConnexion", "Desactive", "IdProfil", "IdUtilisateurType", "MotPasse", "Nom", "Prenoms", "Remarques", "UtilisateurCreation", "UtilisateurModification" },
                values: new object[] { new Guid("2da7c8a6-c9f5-4c11-9daf-f5f0a2bc2ff6"), null, true, "admin", 1765250386368L, null, null, false, null, new Guid("7e5b7d94-4f5d-4eff-9983-c8f846d3cee6"), "$2a$11$/oAAQsnYQhV/hBCeLB62Uuv0jGkXE5sl97Fl37f4g.mmdbslbtO/u", "Administrateur", "ARPCE", null, "SYSTEM_SEED", null });

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
