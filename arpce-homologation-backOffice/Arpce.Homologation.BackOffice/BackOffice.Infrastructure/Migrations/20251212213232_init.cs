using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackOffice.Infrastructure.Migrations
{
    /// <inheritdoc />
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
    public partial class InitialCreate : Migration
========
    public partial class init : Migration
>>>>>>>> 8caf2139681d5e1ffa745bdafb14f25b4b9e2689:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212214022_init.cs
========
    public partial class init : Migration
>>>>>>>> d6352f000ab0dfbf17929b9d71f5985e16e23c29:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212213232_init.cs
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
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
========
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
>>>>>>>> 8caf2139681d5e1ffa745bdafb14f25b4b9e2689:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212214022_init.cs
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
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
                    TypeEquipement = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TypeClient = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FormuleHomologation = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    QuantiteReference = table.Column<int>(type: "int", nullable: true),
                    TarifEtude = table.Column<decimal>(type: "money", nullable: true),
                    TarifHomologation = table.Column<byte>(type: "tinyint", nullable: true),
                    FraisHomologationParLot = table.Column<byte>(type: "tinyint", nullable: true),
                    FraisHomologationQuantiteParLot = table.Column<int>(type: "int", nullable: true),
                    TarifControle = table.Column<decimal>(type: "money", nullable: true),
========
                    TypeEquipement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeClient = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormuleHomologation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuantiteReference = table.Column<int>(type: "int", nullable: true),
                    FraisEtude = table.Column<decimal>(type: "money", nullable: true),
                    FraisHomologation = table.Column<decimal>(type: "money", nullable: true),
                    FraisHomologationParLot = table.Column<byte>(type: "tinyint", nullable: true),
                    FraisHomologationQuantiteParLot = table.Column<int>(type: "int", nullable: true),
                    FraisControle = table.Column<decimal>(type: "money", nullable: true),
>>>>>>>> 8caf2139681d5e1ffa745bdafb14f25b4b9e2689:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212214022_init.cs
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
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
========
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(max)", nullable: true),
>>>>>>>> 8caf2139681d5e1ffa745bdafb14f25b4b9e2689:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212214022_init.cs
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
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
                    Compte = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prenoms = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    MotPasse = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ChangementMotPasse = table.Column<bool>(type: "bit", nullable: false),
                    Desactive = table.Column<bool>(type: "bit", nullable: false),
========
                    Compte = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Prenoms = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    MotPasse = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ChangementMotPasse = table.Column<byte>(type: "tinyint", nullable: false),
                    Desactive = table.Column<byte>(type: "tinyint", nullable: false),
>>>>>>>> 8caf2139681d5e1ffa745bdafb14f25b4b9e2689:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212214022_init.cs
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
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
                        name: "FK_AdminUtilisateurs_adminProfils_AdminProfilsId",
========
                        name: "FK_adminUtilisateurs_adminProfils_AdminProfilsId",
>>>>>>>> 8caf2139681d5e1ffa745bdafb14f25b4b9e2689:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212214022_init.cs
                        column: x => x.AdminProfilsId,
                        principalTable: "adminProfils",
                        principalColumn: "Id");
                    table.ForeignKey(
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
                        name: "FK_AdminUtilisateurs_adminProfils_IdProfil",
========
                        name: "FK_adminUtilisateurs_adminProfils_IdProfil",
>>>>>>>> 8caf2139681d5e1ffa745bdafb14f25b4b9e2689:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212214022_init.cs
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
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
========
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(max)", nullable: true),
>>>>>>>> 8caf2139681d5e1ffa745bdafb14f25b4b9e2689:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212214022_init.cs
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
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
                    Utilisateur = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
========
                    Utilisateur = table.Column<string>(type: "nvarchar(max)", nullable: false),
>>>>>>>> 8caf2139681d5e1ffa745bdafb14f25b4b9e2689:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212214022_init.cs
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
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
                    Proposition = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
========
                    Proposition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(max)", nullable: true),
>>>>>>>> 8caf2139681d5e1ffa745bdafb14f25b4b9e2689:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212214022_init.cs
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
                    Remise = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    EstHomologable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
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
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
========
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(max)", nullable: true),
>>>>>>>> 8caf2139681d5e1ffa745bdafb14f25b4b9e2689:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212214022_init.cs
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
                name: "devis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
                    IdDossier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
========
                    IdDemande = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
>>>>>>>> 8caf2139681d5e1ffa745bdafb14f25b4b9e2689:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212214022_init.cs
========
                    IdDossier = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
>>>>>>>> d6352f000ab0dfbf17929b9d71f5985e16e23c29:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212213232_init.cs
                    Date = table.Column<long>(type: "bigint", nullable: false),
                    MontantEtude = table.Column<decimal>(type: "money", nullable: false),
                    MontantHomologation = table.Column<decimal>(type: "money", nullable: true),
                    MontantControle = table.Column<decimal>(type: "money", nullable: true),
                    PaiementOk = table.Column<byte>(type: "tinyint", nullable: true),
                    PaiementMobileId = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DemandeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateModification = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_devis_demandes_DemandeId",
                        column: x => x.DemandeId,
                        principalTable: "demandes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_devis_dossiers_IdDossier",
                        column: x => x.IdDossier,
                        principalTable: "dossiers",
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
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
========
                    UtilisateurCreation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreation = table.Column<long>(type: "bigint", nullable: true),
                    UtilisateurModification = table.Column<string>(type: "nvarchar(max)", nullable: true),
>>>>>>>> 8caf2139681d5e1ffa745bdafb14f25b4b9e2689:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212214022_init.cs
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
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
                    { new Guid("00108f77-b2d4-4230-a7b9-9e3c8b95f2ed"), "ATTRIBUTION", "Attribution de droits/profils" },
                    { new Guid("3c9e0acc-6712-4456-86a7-4c894ff94a25"), "SUPPRESSION", "Suppression de données" },
                    { new Guid("4601a315-aa4c-4961-97c2-df72660c4656"), "QUALIFICATION", "Qualification de données" },
                    { new Guid("63c158ee-df56-429a-b024-dad316c3ac8b"), "COMMUNICATION", "Envoi de communication" },
                    { new Guid("6dd2c94b-da0b-45c3-9771-b269ca54fb11"), "MODIFICATION", "Modification de données" },
                    { new Guid("815b2cfb-942c-4d29-9be2-a490aea7d2a5"), "CONNEXION", "Modification" },
                    { new Guid("8b610d0d-fccb-4552-8434-e83e983d107f"), "CREATION", "Création de données" },
                    { new Guid("a52c1339-08e5-4223-a9d1-01fd9d6bea07"), "VALIDATION", "Validation de processus" },
                    { new Guid("eda93f2f-c76d-4999-b32e-deabcd980172"), "SECURITE", "Action de sécurité" },
                    { new Guid("fcf83bfd-a2ae-4d2b-8fa6-3611d18439fd"), "MODIFICATION", "Connexion utilisateur" }
========
                    { new Guid("390ece3c-08f2-4ae2-9396-547b5a60559e"), "QUALIFICATION", "Qualification de données" },
                    { new Guid("41b6be1b-c167-4041-8b75-d17273b9bbf3"), "ATTRIBUTION", "Attribution de droits/profils" },
                    { new Guid("7a9fc0e5-a731-418a-ae19-4e075f9b164c"), "CREATION", "Création de données" },
                    { new Guid("89211b41-4d24-4235-b629-93452d510821"), "CONNEXION", "Modification" },
                    { new Guid("8d68dd60-c098-4b2a-82e7-3bb3ab8b7f41"), "MODIFICATION", "Modification de données" },
                    { new Guid("8d6fa96d-ea2e-4af7-a11d-3c4b426edbe8"), "MODIFICATION", "Connexion utilisateur" },
                    { new Guid("8e6cf061-3fd5-4147-bab6-8130b01b7911"), "SUPPRESSION", "Suppression de données" },
                    { new Guid("92c689bc-a675-4f75-9067-a086763dc649"), "SECURITE", "Action de sécurité" },
                    { new Guid("d2a9dc50-2a35-44a6-b918-9451c86149d7"), "COMMUNICATION", "Envoi de communication" },
                    { new Guid("ecf66f28-2618-49ae-9525-522a0f721f77"), "VALIDATION", "Validation de processus" }
>>>>>>>> d6352f000ab0dfbf17929b9d71f5985e16e23c29:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212213232_init.cs
                });

            migrationBuilder.InsertData(
                table: "adminUtilisateurTypes",
                columns: new[] { "Id", "Libelle" },
                values: new object[,]
                {
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
                    { new Guid("0e0ac5cf-ee48-4462-8d25-8fb3412e1608"), "Auditeur" },
                    { new Guid("7e5b7d94-4f5d-4eff-9983-c8f846d3cee6"), "Administrateur" },
                    { new Guid("d0916bee-2154-4a2c-99bb-90b74854d3ec"), "Utilisateur Standard" }
========
                    { new Guid("233299e4-d4e2-424a-a3c2-b69d782eca35"), "Auditeur" },
                    { new Guid("7e5b7d94-4f5d-4eff-9983-c8f846d3cee6"), "Administrateur" },
                    { new Guid("e4a63583-4450-42f8-8a2f-081496578e00"), "Utilisateur Standard" }
>>>>>>>> d6352f000ab0dfbf17929b9d71f5985e16e23c29:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212213232_init.cs
                });

            migrationBuilder.InsertData(
                table: "modesReglements",
                columns: new[] { "Id", "Code", "DateCreation", "DateModification", "Libelle", "MobileBanking", "Remarques", "UtilisateurCreation", "UtilisateurModification" },
                values: new object[,]
                {
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
                    { new Guid("4da4a406-9e1e-431b-af72-3b35c8c5c277"), "Especes", null, null, "Espèces", (byte)0, null, null, null },
                    { new Guid("c0cb7af8-b23b-46b3-990a-6ca42f21cf5f"), "MobileBanking", null, null, "Paiement mobile", (byte)1, null, null, null },
                    { new Guid("e186de6e-53f2-4ea7-81b0-b25b2b688355"), "Virement", null, null, "Virement bancaire", (byte)0, null, null, null },
                    { new Guid("e447a939-79c3-4f6b-b875-ce9812b1e9f3"), "Cheque", null, null, "Chèque", (byte)0, null, null, null }
========
                    { new Guid("3e38fc1b-01c2-438e-a872-500275b8ad99"), "Especes", null, null, "Espèces", (byte)0, null, null, null },
                    { new Guid("708dec01-3ee1-4a01-8366-3f7161ceb42f"), "MobileBanking", null, null, "Paiement mobile", (byte)1, null, null, null },
                    { new Guid("bc947f38-22fe-4379-8930-1224559b6316"), "Cheque", null, null, "Chèque", (byte)0, null, null, null },
                    { new Guid("f2001635-ec3e-46fb-b876-414bc4ce1a18"), "Virement", null, null, "Virement bancaire", (byte)0, null, null, null }
>>>>>>>> 8caf2139681d5e1ffa745bdafb14f25b4b9e2689:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212214022_init.cs
========
                    { new Guid("921924ca-83a9-4635-a738-f0dcd0bb2493"), "MobileBanking", null, null, "Paiement mobile", (byte)1, null, null, null },
                    { new Guid("bb6dd42e-c5de-4fea-9aa5-7da142d82cf5"), "Especes", null, null, "Espèces", (byte)0, null, null, null },
                    { new Guid("bd35b328-213d-42a0-8b00-8d9e7a8bfc0a"), "Cheque", null, null, "Chèque", (byte)0, null, null, null },
                    { new Guid("c7573900-5a9c-4d1d-bb0b-cf5739a9e6ff"), "Virement", null, null, "Virement bancaire", (byte)0, null, null, null }
>>>>>>>> d6352f000ab0dfbf17929b9d71f5985e16e23c29:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212213232_init.cs
                });

            migrationBuilder.InsertData(
                table: "statuts",
                columns: new[] { "Id", "Code", "Libelle" },
                values: new object[,]
                {
                    { new Guid("0c47f66c-3ae3-4dbf-af5b-1b7eb3f2a213"), "PaiementRejete", "Paiement non accepté" },
                    { new Guid("0d4f6e52-ad8e-4b67-9a11-985b66dca204"), "ApprobationInstruction", "Envoyé pour approbation" },
                    { new Guid("0e9a8bb4-7989-4eb8-9f21-4f9b7ffca216"), "DossierSignature", "Attestation en signature" },
                    { new Guid("1a173ab7-6db7-4b9f-906e-a85352a4a212"), "DevisPaiement", "En attente de paiement" },
                    { new Guid("33b7bd1d-5901-4afe-be70-d4c10e3fa215"), "DossierPayer", "Paiement effectué" },
                    { new Guid("3b9ed3a1-1e24-4d0c-8f13-7c55c9baa203"), "Instruction", "En cours d'instruction" },
                    { new Guid("84c6d32b-1f44-4fbd-8c91-12e3b5e0a205"), "InstructionApprouve", "Instruction Approuvée" },
                    { new Guid("8c2bc784-06a3-4d73-a9f4-5f6d94a7a210"), "DevisValide", "Devis validé par client" },
                    { new Guid("9f1c2f69-5d8e-4ec8-a6a1-0aa1e1c5a201"), "NouveauDossier", "Nouvelle demande" },
                    { new Guid("a7c55954-7b1c-4f43-9cc4-1f2af3cca202"), "RefusDossier", "Refus de la demande" },
                    { new Guid("ae906d70-a1c2-4b2a-8db7-b22c6d4ca207"), "DevisValideSC", "Devis validé par Chef Service" },
                    { new Guid("ccf4f5b7-8be7-4f01-9c09-fa5522d6a209"), "DevisEmit", "Devis émis" },
                    { new Guid("cd3c7e21-6909-4a29-9f0e-90e9ac2da211"), "DevisRefuser", "Devis refusé par client" },
                    { new Guid("d62e63cb-4c2f-4e24-b5a2-8fae11e0a206"), "DevisCreer", "Devis créé" },
                    { new Guid("df4bb5aa-17d5-4c04-8545-48f59c59a214"), "PaiementExpirer", "Paiement expiré" },
                    { new Guid("ed13c54b-5e63-4a0f-a0a7-332a7c27a217"), "DossierSigner", "Attestation signée" },
                    { new Guid("f1a2b3c4-d5e6-4f78-9012-34567890a218"), "Echantillon", "En attente échantillon" },
                    { new Guid("fc01b3e8-82d8-4e55-953f-0fc9edb2a208"), "DevisValideTr", "Devis validé par Trésorerie" }
                });

            migrationBuilder.InsertData(
                table: "AdminUtilisateurs",
                columns: new[] { "Id", "AdminProfilsId", "ChangementMotPasse", "Compte", "DateCreation", "DateModification", "DerniereConnexion", "Desactive", "IdProfil", "IdUtilisateurType", "MotPasse", "Nom", "Prenoms", "Remarques", "UtilisateurCreation", "UtilisateurModification" },
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
                values: new object[] { new Guid("88888888-8888-8888-8888-888888888888"), null, true, "admin", 1765574571027L, null, null, false, null, new Guid("7e5b7d94-4f5d-4eff-9983-c8f846d3cee6"), "$2a$11$liznrld3jmsQCzlEREJnLuAQYfKMbfnmD6xzk8QGmRm0tHbuByue2", "root", "ARPCE", null, "SYSTEM_SEED", null });

            migrationBuilder.CreateIndex(
                name: "IX_adminJournal_DossierId",
                table: "adminJournal",
                column: "DossierId");
========
                values: new object[] { new Guid("88888888-8888-8888-8888-888888888888"), null, true, "admin", 1765575145519L, null, null, false, null, new Guid("7e5b7d94-4f5d-4eff-9983-c8f846d3cee6"), "$2a$11$sIYhhCUqEB5O1RAp36wHdunmgMoLn/BNIn6cJBUsv7axVdREih6G6", "root", "ARPCE", null, "SYSTEM_SEED", null });
>>>>>>>> d6352f000ab0dfbf17929b9d71f5985e16e23c29:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212213232_init.cs

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
<<<<<<<< HEAD:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212212252_InitialCreate.cs
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
========
                name: "IX_adminUtilisateurs_AdminProfilsId",
                table: "adminUtilisateurs",
                column: "AdminProfilsId");

            migrationBuilder.CreateIndex(
                name: "IX_adminUtilisateurs_IdProfil",
                table: "adminUtilisateurs",
>>>>>>>> 8caf2139681d5e1ffa745bdafb14f25b4b9e2689:arpce-homologation-backOffice/Arpce.Homologation.BackOffice/BackOffice.Infrastructure/Migrations/20251212214022_init.cs
                column: "IdProfil");

            migrationBuilder.CreateIndex(
                name: "IX_AdminUtilisateurs_IdUtilisateurType",
                table: "AdminUtilisateurs",
                column: "IdUtilisateurType");

            migrationBuilder.CreateIndex(
                name: "IX_AdminUtilisateurs_Nom",
                table: "AdminUtilisateurs",
                column: "Nom",
                unique: true);

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
                name: "IX_devis_DemandeId",
                table: "devis",
                column: "DemandeId");

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
