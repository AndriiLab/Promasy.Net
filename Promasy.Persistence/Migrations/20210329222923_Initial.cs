using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Promasy.Persistence.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                "PromasyCore");

            migrationBuilder.CreateTable(
                "Cpvs",
                schema: "PromasyCore",
                columns: table => new
                {
                    CpvCode = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    CpvEng = table.Column<string>("character varying(1000)", maxLength: 1000,
                        nullable: false),
                    CpvUkr = table.Column<string>("character varying(1000)", maxLength: 1000,
                        nullable: false),
                    CpvLevel = table.Column<int>("integer", nullable: false),
                    Terminal = table.Column<bool>("boolean", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Cpvs", x => x.CpvCode); });

            migrationBuilder.CreateTable(
                "DeviceFlowCodes",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    DeviceCode = table.Column<string>("text", nullable: true),
                    UserCode = table.Column<string>("text", nullable: true),
                    SubjectId = table.Column<string>("text", nullable: true),
                    SessionId = table.Column<string>("text", nullable: true),
                    ClientId = table.Column<string>("text", nullable: true),
                    Description = table.Column<string>("text", nullable: true),
                    CreationTime =
                        table.Column<DateTime>("timestamp without time zone", nullable: false),
                    Expiration =
                        table.Column<DateTime>("timestamp without time zone", nullable: true),
                    Data = table.Column<string>("text", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_DeviceFlowCodes", x => x.Id); });

            migrationBuilder.CreateTable(
                "PersistedGrants",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Key = table.Column<string>("text", nullable: true),
                    Type = table.Column<string>("text", nullable: true),
                    SubjectId = table.Column<string>("text", nullable: true),
                    SessionId = table.Column<string>("text", nullable: true),
                    ClientId = table.Column<string>("text", nullable: true),
                    Description = table.Column<string>("text", nullable: true),
                    CreationTime =
                        table.Column<DateTime>("timestamp without time zone", nullable: false),
                    Expiration =
                        table.Column<DateTime>("timestamp without time zone", nullable: true),
                    ConsumedTime =
                        table.Column<DateTime>("timestamp without time zone", nullable: true),
                    Data = table.Column<string>("text", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_PersistedGrants", x => x.Id); });

            migrationBuilder.CreateTable(
                "Roles",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    NormalizedName = table.Column<string>("character varying(256)", maxLength: 256,
                        nullable: true),
                    ConcurrencyStamp = table.Column<string>("text", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Roles", x => x.Id); });

            migrationBuilder.CreateTable(
                "AspNetRoleClaims",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    RoleId = table.Column<int>("integer", nullable: false),
                    ClaimType = table.Column<string>("text", nullable: true),
                    ClaimValue = table.Column<string>("text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        "FK_AspNetRoleClaims_Roles_RoleId",
                        x => x.RoleId,
                        principalSchema: "PromasyCore",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Institutes",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    Email = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: true),
                    Edrpou = table.Column<string>("character varying(20)", maxLength: 20,
                        nullable: true),
                    FaxNumber = table.Column<string>("character varying(30)", maxLength: 30,
                        nullable: true),
                    PhoneNumber = table.Column<string>("character varying(30)", maxLength: 30,
                        nullable: true),
                    AddressId = table.Column<int>("integer", nullable: false),
                    CreatedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: false),
                    ModifiedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: true),
                    Deleted = table.Column<bool>("boolean", nullable: false),
                    CreatorId = table.Column<int>("integer", nullable: true),
                    ModifierId = table.Column<int>("integer", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Institutes", x => x.Id); });

            migrationBuilder.CreateTable(
                "Bids",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Amount = table.Column<int>("integer", nullable: false),
                    Description = table.Column<string>("character varying(1000)", maxLength: 1000,
                        nullable: false),
                    CatNum = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: true),
                    OnePrice = table.Column<decimal>("numeric", nullable: false),
                    Type = table.Column<int>("integer", nullable: false),
                    Kekv = table.Column<int>("integer", nullable: true),
                    ProcurementStartDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: true),
                    AmountUnitId = table.Column<int>("integer", nullable: false),
                    CpvCode = table.Column<string>("character varying(300)", nullable: true),
                    FinanceDepartmentId = table.Column<int>("integer", nullable: false),
                    ProducerId = table.Column<int>("integer", nullable: true),
                    ReasonId = table.Column<int>("integer", nullable: false),
                    SupplierId = table.Column<int>("integer", nullable: false),
                    CreatedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: false),
                    ModifiedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: true),
                    Deleted = table.Column<bool>("boolean", nullable: false),
                    CreatorId = table.Column<int>("integer", nullable: true),
                    ModifierId = table.Column<int>("integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bids", x => x.Id);
                    table.ForeignKey(
                        "FK_Bids_Cpvs_CpvCode",
                        x => x.CpvCode,
                        principalSchema: "PromasyCore",
                        principalTable: "Cpvs",
                        principalColumn: "CpvCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "BidStatuses",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Status = table.Column<int>("integer", nullable: false),
                    BidId = table.Column<int>("integer", nullable: false),
                    CreatedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: false),
                    ModifiedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: true),
                    Deleted = table.Column<bool>("boolean", nullable: false),
                    CreatorId = table.Column<int>("integer", nullable: true),
                    ModifierId = table.Column<int>("integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidStatuses", x => x.Id);
                    table.ForeignKey(
                        "FK_BidStatuses_Bids_BidId",
                        x => x.BidId,
                        principalSchema: "PromasyCore",
                        principalTable: "Bids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "SubDepartments",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    DepartmentId = table.Column<int>("integer", nullable: false),
                    CreatedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: false),
                    ModifiedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: true),
                    Deleted = table.Column<bool>("boolean", nullable: false),
                    CreatorId = table.Column<int>("integer", nullable: true),
                    ModifierId = table.Column<int>("integer", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_SubDepartments", x => x.Id); });

            migrationBuilder.CreateTable(
                "Employees",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    FirstName = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    MiddleName = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: true),
                    LastName = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    PhoneReserve = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: true),
                    SubDepartmentId = table.Column<int>("integer", nullable: false),
                    CreatedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: false),
                    ModifiedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: true),
                    Deleted = table.Column<bool>("boolean", nullable: false),
                    CreatorId = table.Column<int>("integer", nullable: true),
                    ModifierId = table.Column<int>("integer", nullable: true),
                    UserName = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    NormalizedUserName = table.Column<string>("character varying(256)",
                        maxLength: 256, nullable: true),
                    Email = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    NormalizedEmail = table.Column<string>("character varying(256)", maxLength: 256,
                        nullable: true),
                    EmailConfirmed = table.Column<bool>("boolean", nullable: false),
                    PasswordHash = table.Column<string>("text", nullable: true),
                    SecurityStamp = table.Column<string>("text", nullable: true),
                    ConcurrencyStamp = table.Column<string>("text", nullable: true),
                    PhoneNumber = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>("boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>("boolean", nullable: false),
                    LockoutEnd =
                        table.Column<DateTimeOffset>("timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>("boolean", nullable: false),
                    AccessFailedCount = table.Column<int>("integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        "FK_Employees_Employees_CreatorId",
                        x => x.CreatorId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        "FK_Employees_Employees_ModifierId",
                        x => x.ModifierId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        "FK_Employees_SubDepartments_SubDepartmentId",
                        x => x.SubDepartmentId,
                        principalSchema: "PromasyCore",
                        principalTable: "SubDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Addresses",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    BuildingNumber = table.Column<string>("character varying(10)", maxLength: 10,
                        nullable: false),
                    City = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    CityType = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    CorpusNumber = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    Country = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    PostalCode = table.Column<string>("character varying(10)", maxLength: 10,
                        nullable: false),
                    Region = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    Street = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    StreetType = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    CreatedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: false),
                    ModifiedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: true),
                    Deleted = table.Column<bool>("boolean", nullable: false),
                    CreatorId = table.Column<int>("integer", nullable: true),
                    ModifierId = table.Column<int>("integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        "FK_Addresses_Employees_CreatorId",
                        x => x.CreatorId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Addresses_Employees_ModifierId",
                        x => x.ModifierId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "AmountUnits",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Description = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    CreatedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: false),
                    ModifiedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: true),
                    Deleted = table.Column<bool>("boolean", nullable: false),
                    CreatorId = table.Column<int>("integer", nullable: true),
                    ModifierId = table.Column<int>("integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmountUnits", x => x.Id);
                    table.ForeignKey(
                        "FK_AmountUnits_Employees_CreatorId",
                        x => x.CreatorId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_AmountUnits_Employees_ModifierId",
                        x => x.ModifierId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "AspNetUserClaims",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    UserId = table.Column<int>("integer", nullable: false),
                    ClaimType = table.Column<string>("text", nullable: true),
                    ClaimValue = table.Column<string>("text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        "FK_AspNetUserClaims_Employees_UserId",
                        x => x.UserId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserLogins",
                schema: "PromasyCore",
                columns: table => new
                {
                    LoginProvider = table.Column<string>("text", nullable: false),
                    ProviderKey = table.Column<string>("text", nullable: false),
                    ProviderDisplayName = table.Column<string>("text", nullable: true),
                    UserId = table.Column<int>("integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins",
                        x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        "FK_AspNetUserLogins_Employees_UserId",
                        x => x.UserId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserRoles",
                schema: "PromasyCore",
                columns: table => new
                {
                    UserId = table.Column<int>("integer", nullable: false),
                    RoleId = table.Column<int>("integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        "FK_AspNetUserRoles_Employees_UserId",
                        x => x.UserId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_AspNetUserRoles_Roles_RoleId",
                        x => x.RoleId,
                        principalSchema: "PromasyCore",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserTokens",
                schema: "PromasyCore",
                columns: table => new
                {
                    UserId = table.Column<int>("integer", nullable: false),
                    LoginProvider = table.Column<string>("text", nullable: false),
                    Name = table.Column<string>("text", nullable: false),
                    Value = table.Column<string>("text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens",
                        x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        "FK_AspNetUserTokens_Employees_UserId",
                        x => x.UserId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Departments",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    InstituteId = table.Column<int>("integer", nullable: false),
                    CreatedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: false),
                    ModifiedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: true),
                    Deleted = table.Column<bool>("boolean", nullable: false),
                    CreatorId = table.Column<int>("integer", nullable: true),
                    ModifierId = table.Column<int>("integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        "FK_Departments_Employees_CreatorId",
                        x => x.CreatorId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Departments_Employees_ModifierId",
                        x => x.ModifierId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Departments_Institutes_InstituteId",
                        x => x.InstituteId,
                        principalSchema: "PromasyCore",
                        principalTable: "Institutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "FinanceSources",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    Number = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    FundType = table.Column<int>("integer", nullable: false),
                    Kpkvk = table.Column<string>("character varying(10)", maxLength: 10,
                        nullable: false),
                    StartsOn =
                        table.Column<DateTime>("timestamp without time zone", nullable: false),
                    DueTo = table.Column<DateTime>("timestamp without time zone", nullable: false),
                    TotalEquipment = table.Column<decimal>("numeric", nullable: false),
                    TotalMaterials = table.Column<decimal>("numeric", nullable: false),
                    TotalServices = table.Column<decimal>("numeric", nullable: false),
                    CreatedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: false),
                    ModifiedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: true),
                    Deleted = table.Column<bool>("boolean", nullable: false),
                    CreatorId = table.Column<int>("integer", nullable: true),
                    ModifierId = table.Column<int>("integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceSources", x => x.Id);
                    table.ForeignKey(
                        "FK_FinanceSources_Employees_CreatorId",
                        x => x.CreatorId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_FinanceSources_Employees_ModifierId",
                        x => x.ModifierId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Producers",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    CreatedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: false),
                    ModifiedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: true),
                    Deleted = table.Column<bool>("boolean", nullable: false),
                    CreatorId = table.Column<int>("integer", nullable: true),
                    ModifierId = table.Column<int>("integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producers", x => x.Id);
                    table.ForeignKey(
                        "FK_Producers_Employees_CreatorId",
                        x => x.CreatorId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Producers_Employees_ModifierId",
                        x => x.ModifierId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "ReasonForSuppliers",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Description = table.Column<string>("character varying(1000)", maxLength: 1000,
                        nullable: false),
                    CreatedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: false),
                    ModifiedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: true),
                    Deleted = table.Column<bool>("boolean", nullable: false),
                    CreatorId = table.Column<int>("integer", nullable: true),
                    ModifierId = table.Column<int>("integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReasonForSuppliers", x => x.Id);
                    table.ForeignKey(
                        "FK_ReasonForSuppliers_Employees_CreatorId",
                        x => x.CreatorId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_ReasonForSuppliers_Employees_ModifierId",
                        x => x.ModifierId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Suppliers",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: false),
                    Comment = table.Column<string>("character varying(1000)", maxLength: 1000,
                        nullable: true),
                    Phone = table.Column<string>("character varying(300)", maxLength: 300,
                        nullable: true),
                    CreatedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: false),
                    ModifiedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: true),
                    Deleted = table.Column<bool>("boolean", nullable: false),
                    CreatorId = table.Column<int>("integer", nullable: true),
                    ModifierId = table.Column<int>("integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                    table.ForeignKey(
                        "FK_Suppliers_Employees_CreatorId",
                        x => x.CreatorId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Suppliers_Employees_ModifierId",
                        x => x.ModifierId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "FinanceDepartments",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>("integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    TotalEquipment = table.Column<decimal>("numeric", nullable: false),
                    TotalMaterials = table.Column<decimal>("numeric", nullable: false),
                    TotalServices = table.Column<decimal>("numeric", nullable: false),
                    FinanceSourceId = table.Column<int>("integer", nullable: false),
                    SubDepartmentId = table.Column<int>("integer", nullable: false),
                    CreatedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: false),
                    ModifiedDate =
                        table.Column<DateTime>("timestamp without time zone", nullable: true),
                    Deleted = table.Column<bool>("boolean", nullable: false),
                    CreatorId = table.Column<int>("integer", nullable: true),
                    ModifierId = table.Column<int>("integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceDepartments", x => x.Id);
                    table.ForeignKey(
                        "FK_FinanceDepartments_Employees_CreatorId",
                        x => x.CreatorId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_FinanceDepartments_Employees_ModifierId",
                        x => x.ModifierId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_FinanceDepartments_FinanceSources_FinanceSourceId",
                        x => x.FinanceSourceId,
                        principalSchema: "PromasyCore",
                        principalTable: "FinanceSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_FinanceDepartments_SubDepartments_SubDepartmentId",
                        x => x.SubDepartmentId,
                        principalSchema: "PromasyCore",
                        principalTable: "SubDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_Addresses_CreatorId",
                schema: "PromasyCore",
                table: "Addresses",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                "IX_Addresses_ModifierId",
                schema: "PromasyCore",
                table: "Addresses",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                "IX_AmountUnits_CreatorId",
                schema: "PromasyCore",
                table: "AmountUnits",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                "IX_AmountUnits_ModifierId",
                schema: "PromasyCore",
                table: "AmountUnits",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                "IX_AspNetRoleClaims_RoleId",
                schema: "PromasyCore",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserClaims_UserId",
                schema: "PromasyCore",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserLogins_UserId",
                schema: "PromasyCore",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserRoles_RoleId",
                schema: "PromasyCore",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                "IX_Bids_AmountUnitId",
                schema: "PromasyCore",
                table: "Bids",
                column: "AmountUnitId");

            migrationBuilder.CreateIndex(
                "IX_Bids_CpvCode",
                schema: "PromasyCore",
                table: "Bids",
                column: "CpvCode");

            migrationBuilder.CreateIndex(
                "IX_Bids_CreatorId",
                schema: "PromasyCore",
                table: "Bids",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                "IX_Bids_FinanceDepartmentId",
                schema: "PromasyCore",
                table: "Bids",
                column: "FinanceDepartmentId");

            migrationBuilder.CreateIndex(
                "IX_Bids_ModifierId",
                schema: "PromasyCore",
                table: "Bids",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                "IX_Bids_ProducerId",
                schema: "PromasyCore",
                table: "Bids",
                column: "ProducerId");

            migrationBuilder.CreateIndex(
                "IX_Bids_ReasonId",
                schema: "PromasyCore",
                table: "Bids",
                column: "ReasonId");

            migrationBuilder.CreateIndex(
                "IX_Bids_SupplierId",
                schema: "PromasyCore",
                table: "Bids",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                "IX_BidStatuses_BidId",
                schema: "PromasyCore",
                table: "BidStatuses",
                column: "BidId");

            migrationBuilder.CreateIndex(
                "IX_BidStatuses_CreatorId",
                schema: "PromasyCore",
                table: "BidStatuses",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                "IX_BidStatuses_ModifierId",
                schema: "PromasyCore",
                table: "BidStatuses",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                "IX_Departments_CreatorId",
                schema: "PromasyCore",
                table: "Departments",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                "IX_Departments_InstituteId",
                schema: "PromasyCore",
                table: "Departments",
                column: "InstituteId");

            migrationBuilder.CreateIndex(
                "IX_Departments_ModifierId",
                schema: "PromasyCore",
                table: "Departments",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                "EmailIndex",
                schema: "PromasyCore",
                table: "Employees",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                "IX_Employees_CreatorId",
                schema: "PromasyCore",
                table: "Employees",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                "IX_Employees_ModifierId",
                schema: "PromasyCore",
                table: "Employees",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                "IX_Employees_SubDepartmentId",
                schema: "PromasyCore",
                table: "Employees",
                column: "SubDepartmentId");

            migrationBuilder.CreateIndex(
                "UserNameIndex",
                schema: "PromasyCore",
                table: "Employees",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_FinanceDepartments_CreatorId",
                schema: "PromasyCore",
                table: "FinanceDepartments",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                "IX_FinanceDepartments_FinanceSourceId",
                schema: "PromasyCore",
                table: "FinanceDepartments",
                column: "FinanceSourceId");

            migrationBuilder.CreateIndex(
                "IX_FinanceDepartments_ModifierId",
                schema: "PromasyCore",
                table: "FinanceDepartments",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                "IX_FinanceDepartments_SubDepartmentId",
                schema: "PromasyCore",
                table: "FinanceDepartments",
                column: "SubDepartmentId");

            migrationBuilder.CreateIndex(
                "IX_FinanceSources_CreatorId",
                schema: "PromasyCore",
                table: "FinanceSources",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                "IX_FinanceSources_ModifierId",
                schema: "PromasyCore",
                table: "FinanceSources",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                "IX_Institutes_AddressId",
                schema: "PromasyCore",
                table: "Institutes",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                "IX_Institutes_CreatorId",
                schema: "PromasyCore",
                table: "Institutes",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                "IX_Institutes_ModifierId",
                schema: "PromasyCore",
                table: "Institutes",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                "IX_Producers_CreatorId",
                schema: "PromasyCore",
                table: "Producers",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                "IX_Producers_ModifierId",
                schema: "PromasyCore",
                table: "Producers",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                "IX_ReasonForSuppliers_CreatorId",
                schema: "PromasyCore",
                table: "ReasonForSuppliers",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                "IX_ReasonForSuppliers_ModifierId",
                schema: "PromasyCore",
                table: "ReasonForSuppliers",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                "RoleNameIndex",
                schema: "PromasyCore",
                table: "Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_SubDepartments_CreatorId",
                schema: "PromasyCore",
                table: "SubDepartments",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                "IX_SubDepartments_DepartmentId",
                schema: "PromasyCore",
                table: "SubDepartments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                "IX_SubDepartments_ModifierId",
                schema: "PromasyCore",
                table: "SubDepartments",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                "IX_Suppliers_CreatorId",
                schema: "PromasyCore",
                table: "Suppliers",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                "IX_Suppliers_ModifierId",
                schema: "PromasyCore",
                table: "Suppliers",
                column: "ModifierId");

            migrationBuilder.AddForeignKey(
                "FK_Institutes_Addresses_AddressId",
                schema: "PromasyCore",
                table: "Institutes",
                column: "AddressId",
                principalSchema: "PromasyCore",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_Institutes_Employees_CreatorId",
                schema: "PromasyCore",
                table: "Institutes",
                column: "CreatorId",
                principalSchema: "PromasyCore",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                "FK_Institutes_Employees_ModifierId",
                schema: "PromasyCore",
                table: "Institutes",
                column: "ModifierId",
                principalSchema: "PromasyCore",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                "FK_Bids_AmountUnits_AmountUnitId",
                schema: "PromasyCore",
                table: "Bids",
                column: "AmountUnitId",
                principalSchema: "PromasyCore",
                principalTable: "AmountUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_Bids_Employees_CreatorId",
                schema: "PromasyCore",
                table: "Bids",
                column: "CreatorId",
                principalSchema: "PromasyCore",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                "FK_Bids_Employees_ModifierId",
                schema: "PromasyCore",
                table: "Bids",
                column: "ModifierId",
                principalSchema: "PromasyCore",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                "FK_Bids_FinanceDepartments_FinanceDepartmentId",
                schema: "PromasyCore",
                table: "Bids",
                column: "FinanceDepartmentId",
                principalSchema: "PromasyCore",
                principalTable: "FinanceDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_Bids_Producers_ProducerId",
                schema: "PromasyCore",
                table: "Bids",
                column: "ProducerId",
                principalSchema: "PromasyCore",
                principalTable: "Producers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                "FK_Bids_ReasonForSuppliers_ReasonId",
                schema: "PromasyCore",
                table: "Bids",
                column: "ReasonId",
                principalSchema: "PromasyCore",
                principalTable: "ReasonForSuppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_Bids_Suppliers_SupplierId",
                schema: "PromasyCore",
                table: "Bids",
                column: "SupplierId",
                principalSchema: "PromasyCore",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_BidStatuses_Employees_CreatorId",
                schema: "PromasyCore",
                table: "BidStatuses",
                column: "CreatorId",
                principalSchema: "PromasyCore",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                "FK_BidStatuses_Employees_ModifierId",
                schema: "PromasyCore",
                table: "BidStatuses",
                column: "ModifierId",
                principalSchema: "PromasyCore",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                "FK_SubDepartments_Departments_DepartmentId",
                schema: "PromasyCore",
                table: "SubDepartments",
                column: "DepartmentId",
                principalSchema: "PromasyCore",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                "FK_SubDepartments_Employees_CreatorId",
                schema: "PromasyCore",
                table: "SubDepartments",
                column: "CreatorId",
                principalSchema: "PromasyCore",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                "FK_SubDepartments_Employees_ModifierId",
                schema: "PromasyCore",
                table: "SubDepartments",
                column: "ModifierId",
                principalSchema: "PromasyCore",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Addresses_Employees_CreatorId",
                schema: "PromasyCore",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                "FK_Addresses_Employees_ModifierId",
                schema: "PromasyCore",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                "FK_Departments_Employees_CreatorId",
                schema: "PromasyCore",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                "FK_Departments_Employees_ModifierId",
                schema: "PromasyCore",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                "FK_Institutes_Employees_CreatorId",
                schema: "PromasyCore",
                table: "Institutes");

            migrationBuilder.DropForeignKey(
                "FK_Institutes_Employees_ModifierId",
                schema: "PromasyCore",
                table: "Institutes");

            migrationBuilder.DropForeignKey(
                "FK_SubDepartments_Employees_CreatorId",
                schema: "PromasyCore",
                table: "SubDepartments");

            migrationBuilder.DropForeignKey(
                "FK_SubDepartments_Employees_ModifierId",
                schema: "PromasyCore",
                table: "SubDepartments");

            migrationBuilder.DropTable(
                "AspNetRoleClaims",
                "PromasyCore");

            migrationBuilder.DropTable(
                "AspNetUserClaims",
                "PromasyCore");

            migrationBuilder.DropTable(
                "AspNetUserLogins",
                "PromasyCore");

            migrationBuilder.DropTable(
                "AspNetUserRoles",
                "PromasyCore");

            migrationBuilder.DropTable(
                "AspNetUserTokens",
                "PromasyCore");

            migrationBuilder.DropTable(
                "BidStatuses",
                "PromasyCore");

            migrationBuilder.DropTable(
                "DeviceFlowCodes",
                "PromasyCore");

            migrationBuilder.DropTable(
                "PersistedGrants",
                "PromasyCore");

            migrationBuilder.DropTable(
                "Roles",
                "PromasyCore");

            migrationBuilder.DropTable(
                "Bids",
                "PromasyCore");

            migrationBuilder.DropTable(
                "AmountUnits",
                "PromasyCore");

            migrationBuilder.DropTable(
                "Cpvs",
                "PromasyCore");

            migrationBuilder.DropTable(
                "FinanceDepartments",
                "PromasyCore");

            migrationBuilder.DropTable(
                "Producers",
                "PromasyCore");

            migrationBuilder.DropTable(
                "ReasonForSuppliers",
                "PromasyCore");

            migrationBuilder.DropTable(
                "Suppliers",
                "PromasyCore");

            migrationBuilder.DropTable(
                "FinanceSources",
                "PromasyCore");

            migrationBuilder.DropTable(
                "Employees",
                "PromasyCore");

            migrationBuilder.DropTable(
                "SubDepartments",
                "PromasyCore");

            migrationBuilder.DropTable(
                "Departments",
                "PromasyCore");

            migrationBuilder.DropTable(
                "Institutes",
                "PromasyCore");

            migrationBuilder.DropTable(
                "Addresses",
                "PromasyCore");
        }
    }
}