using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FrontOffice.Infrastructure.Migrations
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
                    Utilisateur = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    DateConnexion = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adminConnexions", x => new { x.Utilisateur, x.DateConnexion });
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
                    LDAPAuthentificationNomDomaine = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LDAPCreationAutoActivation = table.Column<byte>(type: "tinyint", nullable: true),
                    LDAPCreationAutoNomServeur = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LDAPCreationAutoCompte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LDAPCreationAutoPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplicationActivation = table.Column<byte>(type: "tinyint", nullable: true),
                    ReplicationNomServeur = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplicationCompte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplicationPassword = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Code = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    Libelle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Remarques = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<DateTime>(type: "smalldatetime", nullable: true)
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
                    TarifEtude = table.Column<decimal>(type: "money", nullable: true),
                    TarifHomologation = table.Column<decimal>(type: "money", nullable: true),
                    TarifHomologationParLot = table.Column<byte>(type: "tinyint", nullable: true),
                    TarifHomologationQuantiteParLot = table.Column<int>(type: "int", nullable: true),
                    TarifControle = table.Column<decimal>(type: "money", nullable: true),
                    Remarques = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<DateTime>(type: "smalldatetime", nullable: true)
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
                    DateCreation = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<DateTime>(type: "smalldatetime", nullable: true)
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
                    DateCreation = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<DateTime>(type: "smalldatetime", nullable: true)
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
                    Libelle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
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
                    Utilisateur = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_adminProfilsAcces_adminProfils_IdProfil",
                        column: x => x.IdProfil,
                        principalTable: "adminProfils",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "adminUtilisateurs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUtilisateurType = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdProfil = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Compte = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Prenoms = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    MotPasse = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ChangementMotPasse = table.Column<byte>(type: "tinyint", nullable: true),
                    Desactive = table.Column<byte>(type: "tinyint", nullable: true),
                    Remarques = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DerniereConnexion = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adminUtilisateurs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_adminUtilisateurs_adminProfils_IdProfil",
                        column: x => x.IdProfil,
                        principalTable: "adminProfils",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_adminUtilisateurs_adminUtilisateurTypes_IdUtilisateurType",
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
                    DateOuverture = table.Column<DateTime>(type: "date", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Libelle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dossiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dossiers_clients_IdClient",
                        column: x => x.IdClient,
                        principalTable: "clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    DateCommentaire = table.Column<DateTime>(type: "date", nullable: false),
                    commentaire = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    NomInstructeur = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Proposition = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Remise = table.Column<decimal>(type: "decimal(5,2)", nullable: true)
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
                name: "documentsDossiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDossier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Type = table.Column<byte>(type: "tinyint", nullable: true),
                    Donnees = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
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
                name: "devis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDemande = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    MontantEtude = table.Column<decimal>(type: "money", nullable: false),
                    MontantHomologation = table.Column<decimal>(type: "money", nullable: true),
                    MontantControle = table.Column<decimal>(type: "money", nullable: true),
                    PaiementOk = table.Column<byte>(type: "tinyint", nullable: true),
                    PaiementMobileId = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DossierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_devis_demandes_IdDemande",
                        column: x => x.IdDemande,
                        principalTable: "demandes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_devis_dossiers_DossierId",
                        column: x => x.DossierId,
                        principalTable: "dossiers",
                        principalColumn: "Id");
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
                    FilePath = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
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
                table: "modesReglements",
                columns: new[] { "Id", "Code", "DateCreation", "DateModification", "Libelle", "MobileBanking", "Remarques", "UtilisateurCreation", "UtilisateurModification" },
                values: new object[,]
                {
                    { new Guid("0f69c893-86a1-4e90-b526-af584d73fe0f"), "Cheque", null, null, "Chèque", (byte)0, null, null, null },
                    { new Guid("218286de-89da-4722-9a3e-266876c8a495"), "Especes", null, null, "Espèces", (byte)0, null, null, null },
                    { new Guid("775b8196-8ce0-4832-ac32-03729db8d673"), "MobileBanking", null, null, "Paiement mobile", (byte)1, null, null, null },
                    { new Guid("ea473f9e-91c2-4be1-8c20-22b1c746e5a1"), "Virement", null, null, "Virement bancaire", (byte)0, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "statuts",
                columns: new[] { "Id", "Code", "Libelle" },
                values: new object[,]
                {
                    { new Guid("18b334d3-9a56-4d96-95f7-52466f77d28d"), "RefusDossier", "Refus de la demande" },
                    { new Guid("1d161ddb-b8ac-4bbd-a2f3-d0ff6a45a9e7"), "InstructionApprouve", "Instruction Approuvée" },
                    { new Guid("3291240d-4acb-4bb2-977c-111713469031"), "DossierPayer", "Paiement effectué" },
                    { new Guid("3ae52f82-6b8e-438b-993d-298b6c49ce35"), "PaiementExpirer", "Paiement expiré" },
                    { new Guid("634177c1-24b2-4d2a-b5b1-af24d2f14a50"), "DevisCreer", "Devis créé" },
                    { new Guid("65b75c38-b341-41a2-842f-62f97e23ef1a"), "ApprobationInstruction", "Envoyé pour approbation" },
                    { new Guid("6efbc594-c55f-41ac-8174-9ac2e4b796a9"), "DevisRefuser", "Devis refusé par client" },
                    { new Guid("7337838f-de14-415f-95bc-333962e5411b"), "NouveauDossier", "Nouvelle demande" },
                    { new Guid("7ce14616-a644-4fd7-b512-c39cdfa3048f"), "PaiementRejete", "Paiement non accepté" },
                    { new Guid("7f8460d3-1d79-4301-8897-dd2fa4e484b6"), "DevisEmit", "Devis émis" },
                    { new Guid("8800ac78-df56-43e8-9c8f-bf3da051ff9f"), "Instruction", "En cours d'instruction" },
                    { new Guid("8a583396-ca38-4f79-9205-82e6cdcd60bd"), "DevisValideTr", "Devis validé par Trésorerie" },
                    { new Guid("a5e60514-81b5-4444-9884-8c8fd6c667f1"), "DevisValideSC", "Devis validé par Chef Service" },
                    { new Guid("ab42cd4c-7fba-42f1-83f0-8f28bef8b984"), "DevisValide", "Devis validé par client" },
                    { new Guid("ded7727a-024b-4fb3-b72c-0480f484a249"), "DossierSignature", "Attestation en signature" },
                    { new Guid("fc6b42ff-1c0b-4916-bd87-a4e8e779b0d8"), "DossierSigner", "Attestation signée" }
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
                name: "IX_adminUtilisateurs_IdProfil",
                table: "adminUtilisateurs",
                column: "IdProfil");

            migrationBuilder.CreateIndex(
                name: "IX_adminUtilisateurs_IdUtilisateurType",
                table: "adminUtilisateurs",
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
                name: "IX_devis_DossierId",
                table: "devis",
                column: "DossierId");

            migrationBuilder.CreateIndex(
                name: "IX_devis_IdDemande",
                table: "devis",
                column: "IdDemande");

            migrationBuilder.CreateIndex(
                name: "IX_documentsDemandes_IdDemande",
                table: "documentsDemandes",
                column: "IdDemande");

            migrationBuilder.CreateIndex(
                name: "IX_documentsDossiers_IdDossier",
                table: "documentsDossiers",
                column: "IdDossier");

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
                name: "adminUtilisateurs");

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
                name: "adminProfils");

            migrationBuilder.DropTable(
                name: "adminUtilisateurTypes");

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
                name: "clients");

            migrationBuilder.DropTable(
                name: "modesReglements");

            migrationBuilder.DropTable(
                name: "statuts");
        }
    }
}
