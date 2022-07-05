using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Promasy.Persistence.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PromasyCore");

            migrationBuilder.CreateTable(
                name: "Addresses",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CityType = table.Column<int>(type: "integer", nullable: false),
                    Street = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StreetType = table.Column<int>(type: "integer", nullable: false),
                    BuildingNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    InternalNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    ModifierId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cpvs",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    DescriptionEnglish = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DescriptionUkrainian = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    IsTerminal = table.Column<bool>(type: "boolean", nullable: false),
                    ParentId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cpvs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cpvs_Cpvs_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "PromasyCore",
                        principalTable: "Cpvs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FinanceSources",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Number = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    FundType = table.Column<int>(type: "integer", nullable: false),
                    Kpkvk = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    StartsOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DueTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalEquipment = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalMaterials = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalServices = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    ModifierId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturers",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    ModifierId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReasonForSupplierChoice",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    ModifierId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReasonForSupplierChoice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Token = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByIp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Revoked = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RevokedByIp = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReasonRevoked = table.Column<int>(type: "integer", nullable: true),
                    ReplacedByTokenId = table.Column<int>(type: "integer", nullable: true),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    ModifierId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<int>(type: "integer", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Phone = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    ModifierId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    ModifierId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Email = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Edrpou = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FaxNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    AddressId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    ModifierId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organizations_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "PromasyCore",
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    ModifierId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "PromasyCore",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubDepartments",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    ModifierId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubDepartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubDepartments_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "PromasyCore",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    FirstName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    LastName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    UserName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Email = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    PrimaryPhone = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ReservePhone = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Password = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Salt = table.Column<long>(type: "bigint", nullable: true),
                    SubDepartmentId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    ModifierId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_SubDepartments_SubDepartmentId",
                        column: x => x.SubDepartmentId,
                        principalSchema: "PromasyCore",
                        principalTable: "SubDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinanceDepartments",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    TotalEquipment = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalMaterials = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalServices = table.Column<decimal>(type: "numeric", nullable: false),
                    FinanceSourceId = table.Column<int>(type: "integer", nullable: false),
                    SubDepartmentId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    ModifierId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceDepartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinanceDepartments_FinanceSources_FinanceSourceId",
                        column: x => x.FinanceSourceId,
                        principalSchema: "PromasyCore",
                        principalTable: "FinanceSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FinanceDepartments_SubDepartments_SubDepartmentId",
                        column: x => x.SubDepartmentId,
                        principalSchema: "PromasyCore",
                        principalTable: "SubDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeRoles",
                schema: "PromasyCore",
                columns: table => new
                {
                    EmployeesId = table.Column<int>(type: "integer", nullable: false),
                    RolesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeRoles", x => new { x.EmployeesId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_EmployeeRoles_Employees_EmployeesId",
                        column: x => x.EmployeesId,
                        principalSchema: "PromasyCore",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeRoles_Roles_RolesId",
                        column: x => x.RolesId,
                        principalSchema: "PromasyCore",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CatNum = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    OnePrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Kekv = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProcurementStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UnitId = table.Column<int>(type: "integer", nullable: false),
                    CpvId = table.Column<int>(type: "integer", nullable: false),
                    FinanceDepartmentId = table.Column<int>(type: "integer", nullable: false),
                    ManufacturerId = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<int>(type: "integer", nullable: false),
                    ReasonId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    ModifierId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Cpvs_CpvId",
                        column: x => x.CpvId,
                        principalSchema: "PromasyCore",
                        principalTable: "Cpvs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_FinanceDepartments_FinanceDepartmentId",
                        column: x => x.FinanceDepartmentId,
                        principalSchema: "PromasyCore",
                        principalTable: "FinanceDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalSchema: "PromasyCore",
                        principalTable: "Manufacturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_ReasonForSupplierChoice_ReasonId",
                        column: x => x.ReasonId,
                        principalSchema: "PromasyCore",
                        principalTable: "ReasonForSupplierChoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "PromasyCore",
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Units_UnitId",
                        column: x => x.UnitId,
                        principalSchema: "PromasyCore",
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatuses",
                schema: "PromasyCore",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    ModifierId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderStatuses_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "PromasyCore",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cpvs_ParentId",
                schema: "PromasyCore",
                table: "Cpvs",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_OrganizationId",
                schema: "PromasyCore",
                table: "Departments",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRoles_RolesId",
                schema: "PromasyCore",
                table: "EmployeeRoles",
                column: "RolesId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_SubDepartmentId",
                schema: "PromasyCore",
                table: "Employees",
                column: "SubDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceDepartments_FinanceSourceId",
                schema: "PromasyCore",
                table: "FinanceDepartments",
                column: "FinanceSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceDepartments_SubDepartmentId",
                schema: "PromasyCore",
                table: "FinanceDepartments",
                column: "SubDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CpvId",
                schema: "PromasyCore",
                table: "Orders",
                column: "CpvId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_FinanceDepartmentId",
                schema: "PromasyCore",
                table: "Orders",
                column: "FinanceDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ManufacturerId",
                schema: "PromasyCore",
                table: "Orders",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ReasonId",
                schema: "PromasyCore",
                table: "Orders",
                column: "ReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SupplierId",
                schema: "PromasyCore",
                table: "Orders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UnitId",
                schema: "PromasyCore",
                table: "Orders",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatuses_OrderId",
                schema: "PromasyCore",
                table: "OrderStatuses",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_AddressId",
                schema: "PromasyCore",
                table: "Organizations",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_SubDepartments_DepartmentId",
                schema: "PromasyCore",
                table: "SubDepartments",
                column: "DepartmentId");

            migrationBuilder.Sql(@"
CREATE FUNCTION ""PromasyCore"".fn_getemployeeshortname(id integer)
RETURNS TEXT AS $$
DECLARE result TEXT;
BEGIN
    SELECT concat(""LastName"", ' ', left(""FirstName"", 1), '.', left(""MiddleName"", 1), '.') INTO result
        FROM    ""PromasyCore"".""Employees""
        WHERE   ""Id"" = id;

        RETURN result;
END
    $$ LANGUAGE plpgsql;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeRoles",
                schema: "PromasyCore");

            migrationBuilder.DropTable(
                name: "OrderStatuses",
                schema: "PromasyCore");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "PromasyCore");

            migrationBuilder.DropTable(
                name: "Employees",
                schema: "PromasyCore");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "PromasyCore");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "PromasyCore");

            migrationBuilder.DropTable(
                name: "Cpvs",
                schema: "PromasyCore");

            migrationBuilder.DropTable(
                name: "FinanceDepartments",
                schema: "PromasyCore");

            migrationBuilder.DropTable(
                name: "Manufacturers",
                schema: "PromasyCore");

            migrationBuilder.DropTable(
                name: "ReasonForSupplierChoice",
                schema: "PromasyCore");

            migrationBuilder.DropTable(
                name: "Suppliers",
                schema: "PromasyCore");

            migrationBuilder.DropTable(
                name: "Units",
                schema: "PromasyCore");

            migrationBuilder.DropTable(
                name: "FinanceSources",
                schema: "PromasyCore");

            migrationBuilder.DropTable(
                name: "SubDepartments",
                schema: "PromasyCore");

            migrationBuilder.DropTable(
                name: "Departments",
                schema: "PromasyCore");

            migrationBuilder.DropTable(
                name: "Organizations",
                schema: "PromasyCore");

            migrationBuilder.DropTable(
                name: "Addresses",
                schema: "PromasyCore");
        }
    }
}
