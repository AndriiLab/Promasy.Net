-- ADD MIGRATION COLUMNS
ALTER TABLE "PromasyCore"."Addresses"
    ADD COLUMN OldId INT;

ALTER TABLE "PromasyCore"."Units"
    ADD COLUMN OldId INT;

ALTER TABLE "PromasyCore"."Orders"
    ADD COLUMN OldId INT;

ALTER TABLE "PromasyCore"."OrderStatuses"
    ADD COLUMN OldId INT;

ALTER TABLE "PromasyCore"."Departments"
    ADD COLUMN OldId INT;

ALTER TABLE "PromasyCore"."Employees"
    ADD COLUMN OldId INT;

ALTER TABLE "PromasyCore"."FinanceSubDepartments"
    ADD COLUMN OldId INT;

ALTER TABLE "PromasyCore"."FinanceSources"
    ADD COLUMN OldId INT;

ALTER TABLE "PromasyCore"."Organizations"
    ADD COLUMN OldId INT;

ALTER TABLE "PromasyCore"."Manufacturers"
    ADD COLUMN OldId INT;

ALTER TABLE "PromasyCore"."ReasonForSupplierChoice"
    ADD COLUMN OldId INT;

ALTER TABLE "PromasyCore"."SubDepartments"
    ADD COLUMN OldId INT;

ALTER TABLE "PromasyCore"."Suppliers"
    ADD COLUMN OldId INT;

-- MIGRATION

INSERT INTO "PromasyCore"."Cpvs" ("Code", "DescriptionEnglish", "DescriptionUkrainian", "Level", "IsTerminal")
SELECT cpv_code, cpv_eng, cpv_ukr, cpv_level, terminal
FROM promasy.cpv;

UPDATE "PromasyCore"."Cpvs"
SET "ParentId" = SQ.ParentId
FROM (SELECT C."Id"                                                              AS Id,
             (SELECT CP."Id"
              FROM "PromasyCore"."Cpvs" CP
              WHERE CP."Level" = C."Level" - 1
                AND CP."Code" LIKE concat(substr(C."Code", 1, C."Level"), '0%')) AS ParentId
      FROM "PromasyCore"."Cpvs" C
      WHERE C."Level" > 1) AS SQ
WHERE "Level" > 1
  AND "Id" = SQ.Id
  AND SQ.ParentId IS NOT NULL;


CREATE OR REPLACE FUNCTION FN_CityTypeConverter(city_eum varchar(255)) RETURNS integer
AS
$$
DECLARE
    result integer;
BEGIN
    CASE city_eum
        WHEN 'CITY'
            THEN result = 1;
        WHEN 'URBAN_VILLAGE'
            THEN result = 2;
        WHEN 'SETTLEMENT'
            THEN result = 3;
        ELSE result = 4;
        END CASE;
    RETURN result;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION FN_StreetTypeConverter(street_eum varchar(255)) RETURNS integer
AS
$$
DECLARE
    result integer;
BEGIN
    CASE street_eum
        WHEN 'STREET'
            THEN result = 1;
        WHEN 'AVENUE'
            THEN result = 2;
        WHEN 'QUAY'
            THEN result = 3;
        WHEN 'BOULEVARD'
            THEN result = 4;
        WHEN 'ALLEY'
            THEN result = 5;
        WHEN 'BLIND_ALLEY'
            THEN result = 6;
        WHEN 'DESCENT'
            THEN result = 7;
        WHEN 'HIGHWAY'
            THEN result = 8;
        WHEN 'SQUARE'
            THEN result = 9;
        WHEN 'LANE'
            THEN result = 10;
        WHEN 'LINE'
            THEN result = 11;
        WHEN 'BACK_ALLEY'
            THEN result = 12;
        WHEN 'ENTRY'
            THEN result = 13;
        WHEN 'ENTRY2'
            THEN result = 14;
        WHEN 'PASSAGE'
            THEN result = 15;
        WHEN 'CROSSING'
            THEN result = 16;
        WHEN 'GLADE'
            THEN result = 17;
        WHEN 'SQUARE2'
            THEN result = 18;
        ELSE result = 19;
        END CASE;
    RETURN result;
END;
$$ LANGUAGE plpgsql;

INSERT INTO "PromasyCore"."Addresses" ("CreatedDate", "ModifiedDate", "Deleted", oldid, "BuildingNumber", "City",
                                       "CityType", "InternalNumber", "Country", "PostalCode", "Region", "Street",
                                       "StreetType", "CreatorId")
SELECT created_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       modified_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       active = FALSE,
       id,
       building_number,
       city,
       FN_CityTypeConverter(citytype),
       NULLIF(corpus_number, ''),
       country,
       postal_code,
       region,
       street,
       FN_StreetTypeConverter(streettype),
       0
FROM promasy.addresses;

INSERT INTO "PromasyCore"."Organizations" ("CreatedDate", "ModifiedDate", "Deleted", oldid, "Name", "Email", "Edrpou",
                                           "FaxNumber", "PhoneNumber", "AddressId", "CreatorId")
SELECT I.created_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       I.modified_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       I.active = FALSE,
       I.id,
       I.inst_name,
       LOWER(I.email),
       I.edrpou,
       TRANSLATE(TRIM(NULLIF(I.fax_number, '')), '+- ()', ''),
       TRANSLATE(TRIM(I.phone_number), '+- ()', ''),
       AD."Id",
       0
FROM promasy.institutes I
         JOIN "PromasyCore"."Addresses" AD ON AD.OldId = address_id;

INSERT INTO "PromasyCore"."Departments" ("CreatedDate", "ModifiedDate", "Deleted", oldid, "Name", "OrganizationId",
                                         "CreatorId")
SELECT D.created_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       D.modified_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       D.active = FALSE,
       D.id,
       D.dep_name,
       I."Id",
       0
FROM promasy.departments D
         JOIN "PromasyCore"."Organizations" I ON I.OldId = D.inst_id;

INSERT INTO "PromasyCore"."SubDepartments" ("CreatedDate", "ModifiedDate", "Deleted", oldid, "Name", "DepartmentId",
                                            "CreatorId", "OrganizationId")
SELECT S.created_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       S.modified_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       S.active = FALSE,
       S.id,
       S.subdep_name,
       D."Id",
       0,
       D."OrganizationId"
FROM promasy.subdepartments S
         JOIN "PromasyCore"."Departments" D ON D.OldId = S.dep_id;

INSERT INTO "PromasyCore"."Employees" ("CreatedDate", "ModifiedDate", "Deleted", oldid, "FirstName", "MiddleName",
                                       "LastName", "PrimaryPhone", "ReservePhone", "SubDepartmentId", "UserName",
                                       "Email", "CreatorId", "Password", "Salt", "OrganizationId")
SELECT E.created_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       E.modified_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       E.active = FALSE,
       E.id,
       TRIM(E.emp_fname),
       TRIM(NULLIF(E.emp_mname, '')),
       TRIM(E.emp_lname),
       TRANSLATE(TRIM(E.phone_main), '+- ()', ''),
       TRANSLATE(TRIM(NULLIF(E.phone_reserve, '')), '+- ()', ''),
       SUB."Id",
       LOWER(TRIM(E.login)),
       LOWER(TRIM(E.email)),
       0,
       E.password,
       E.salt,
       D."OrganizationId"
FROM promasy.employees E
         JOIN "PromasyCore"."SubDepartments" SUB ON SUB.OldId = E.subdep_id
         JOIN "PromasyCore"."Departments" D ON D."Id" = SUB."DepartmentId";

UPDATE "PromasyCore"."Addresses"
SET "CreatorId" = (SELECT "Id" FROM "PromasyCore"."Employees" WHERE OLdId = 1)
WHERE "CreatorId" = 0;
UPDATE "PromasyCore"."Addresses"
SET "ModifierId" = (SELECT "Id" FROM "PromasyCore"."Employees" WHERE OLdId = 1)
WHERE "ModifiedDate" IS NOT NULL;
UPDATE "PromasyCore"."Organizations"
SET "CreatorId" = (SELECT "Id" FROM "PromasyCore"."Employees" WHERE OLdId = 1)
WHERE "CreatorId" = 0;
UPDATE "PromasyCore"."Organizations"
SET "ModifierId" = (SELECT "Id" FROM "PromasyCore"."Employees" WHERE OLdId = 1)
WHERE "ModifiedDate" IS NOT NULL;
UPDATE "PromasyCore"."Departments"
SET "CreatorId" = (SELECT "Id" FROM "PromasyCore"."Employees" WHERE OLdId = 1)
WHERE "CreatorId" = 0;
UPDATE "PromasyCore"."Departments"
SET "ModifierId" = (SELECT "Id" FROM "PromasyCore"."Employees" WHERE OLdId = 1)
WHERE "ModifiedDate" IS NOT NULL;
UPDATE "PromasyCore"."SubDepartments"
SET "CreatorId" = (SELECT "Id" FROM "PromasyCore"."Employees" WHERE OLdId = 1)
WHERE "CreatorId" = 0;
UPDATE "PromasyCore"."SubDepartments"
SET "ModifierId" = (SELECT "Id" FROM "PromasyCore"."Employees" WHERE OLdId = 1)
WHERE "ModifiedDate" IS NOT NULL;

CREATE OR REPLACE FUNCTION FN_RoleConverter(role_eum text) RETURNS integer
AS
$$
DECLARE
    result    integer;
    role_name integer;
BEGIN
    CASE role_eum
        WHEN 'ADMIN'
            THEN role_name = 1;
        WHEN 'DIRECTOR'
            THEN role_name = 2;
        WHEN 'DEPUTY_DIRECTOR'
            THEN role_name = 3;
        WHEN 'HEAD_OF_TENDER_COMMITTEE'
            THEN role_name = 4;
        WHEN 'SECRETARY_OF_TENDER_COMMITTEE'
            THEN role_name = 5;
        WHEN 'ACCOUNTANT'
            THEN role_name = 6;
        WHEN 'ECONOMIST'
            THEN role_name = 7;
        WHEN 'HEAD_OF_DEPARTMENT'
            THEN role_name = 8;
        WHEN 'PERSONALLY_LIABLE_EMPLOYEE'
            THEN role_name = 9;
        ELSE role_name = 10;
        END CASE;

    SELECT "Id"
    INTO result
    FROM "PromasyCore"."Roles"
    WHERE "Name" = role_name;

    RETURN result;
END;
$$ LANGUAGE plpgsql;

INSERT INTO "PromasyCore"."EmployeeRoles"("EmployeesId", "RolesId")
SELECT EN."Id", FN_RoleConverter(EO.role)
FROM "PromasyCore"."Employees" EN
         JOIN promasy.employees EO ON EO.id = EN.OldId;

UPDATE "PromasyCore"."Employees" EN
SET "CreatorId" = E."Id"
FROM promasy.employees EO
         JOIN "PromasyCore"."Employees" E ON E.OldId = EO.created_by
WHERE EO.id = EN.OldId;

UPDATE "PromasyCore"."Employees" EN
SET "ModifierId" = E."Id"
FROM promasy.employees EO
         JOIN "PromasyCore"."Employees" E ON E.OldId = EO.modified_by
WHERE EO.id = EN.OldId
  AND EO.modified_by IS NOT NULL;

INSERT INTO "PromasyCore"."Units"("Name", "CreatedDate", "ModifiedDate", "Deleted", "CreatorId",
                                  "ModifierId", oldid, "OrganizationId")
SELECT TRIM(A.amount_unit_desc),
       A.created_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       A.modified_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       A.active = FALSE,
       CR."Id",
       ED."Id",
       A.id,
       D."OrganizationId"
FROM promasy.amount_units A
         LEFT JOIN "PromasyCore"."Employees" CR ON CR.OldId = A.created_by
         LEFT JOIN "PromasyCore"."SubDepartments" SD on SD."Id" = CR."SubDepartmentId"
         LEFT JOIN "PromasyCore"."Departments" D on D."Id" = SD."DepartmentId"
         LEFT JOIN "PromasyCore"."Employees" ED ON ED.OldId = A.modified_by AND A.modified_by IS NOT NULL;

INSERT INTO "PromasyCore"."Manufacturers"("Name", "CreatedDate", "ModifiedDate", "Deleted", "CreatorId", "ModifierId",
                                          OldId, "OrganizationId")
SELECT TRIM(P.brand_name),
       P.created_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       P.modified_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       P.active = FALSE,
       CR."Id",
       ED."Id",
       P.id,
       D."OrganizationId"
FROM promasy.producers P
         LEFT JOIN "PromasyCore"."Employees" CR ON CR.OldId = P.created_by
         LEFT JOIN "PromasyCore"."SubDepartments" SD on SD."Id" = CR."SubDepartmentId"
         LEFT JOIN "PromasyCore"."Departments" D on D."Id" = SD."DepartmentId"
         LEFT JOIN "PromasyCore"."Employees" ED ON ED.OldId = P.modified_by AND P.modified_by IS NOT NULL;

INSERT INTO "PromasyCore"."Suppliers"("Name", "Comment", "Phone", "CreatedDate", "ModifiedDate", "Deleted", "CreatorId",
                                      "ModifierId", oldid, "OrganizationId")
SELECT TRIM(S.supplier_name),
       TRIM(NULLIF(S.supplier_comments, '')),
       TRANSLATE(TRIM(NULLIF(S.supplier_tel, '')), '- ()', ''),
       S.created_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       S.modified_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       S.active = FALSE,
       CR."Id",
       ED."Id",
       S.id,
       D."OrganizationId"
FROM promasy.suppliers S
         LEFT JOIN "PromasyCore"."Employees" CR ON CR.OldId = S.created_by
         LEFT JOIN "PromasyCore"."SubDepartments" SD on SD."Id" = CR."SubDepartmentId"
         LEFT JOIN "PromasyCore"."Departments" D on D."Id" = SD."DepartmentId"
         LEFT JOIN "PromasyCore"."Employees" ED ON ED.OldId = S.modified_by AND S.modified_by IS NOT NULL;

INSERT INTO "PromasyCore"."ReasonForSupplierChoice"("Name", "CreatedDate", "ModifiedDate", "Deleted", "CreatorId",
                                                    "ModifierId", oldid, "OrganizationId")
SELECT TRIM(R.reason_name),
       R.created_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       R.modified_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       R.active = FALSE,
       CR."Id",
       ED."Id",
       R.id,
       D."OrganizationId"
FROM promasy.reasons_for_suppl R
         LEFT JOIN "PromasyCore"."Employees" CR ON CR.OldId = R.created_by
         LEFT JOIN "PromasyCore"."SubDepartments" SD on SD."Id" = CR."SubDepartmentId"
         LEFT JOIN "PromasyCore"."Departments" D on D."Id" = SD."DepartmentId"
         LEFT JOIN "PromasyCore"."Employees" ED ON ED.OldId = R.modified_by AND R.modified_by IS NOT NULL;

CREATE OR REPLACE FUNCTION FN_FundTypeConverter(fund_eum varchar(255)) RETURNS integer
AS
$$
DECLARE
    result integer;
BEGIN
    CASE fund_eum
        WHEN 'COMMON_FUND'
            THEN result = 1;
        ELSE result = 2;
        END CASE;
    RETURN result;
END;
$$ LANGUAGE plpgsql;

INSERT INTO "PromasyCore"."FinanceSources"("Name", "Number", "FundType", "Kpkvk", "Start", "End", "TotalEquipment",
                                           "TotalMaterials", "TotalServices", "CreatedDate", "ModifiedDate", "Deleted",
                                           "CreatorId", "ModifierId", oldid, "OrganizationId")
SELECT TRIM(F.name),
       TRIM(F.number),
       FN_FundTypeConverter(F.fundtype),
       F.kpkvk,
       F.starts_on,
       F.due_to,
       F.total_equpment,
       F.total_materials,
       F.total_services,
       F.created_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       F.modified_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       F.active = false,
       CR."Id",
       ED."Id",
       F.id,
       D."OrganizationId"
FROM promasy.finances F
         LEFT JOIN "PromasyCore"."Employees" CR ON CR.OldId = F.created_by
         LEFT JOIN "PromasyCore"."SubDepartments" SD on SD."Id" = CR."SubDepartmentId"
         LEFT JOIN "PromasyCore"."Departments" D on D."Id" = SD."DepartmentId"
         LEFT JOIN "PromasyCore"."Employees" ED ON ED.OldId = F.modified_by AND F.modified_by IS NOT NULL;

INSERT INTO "PromasyCore"."FinanceSubDepartments"("TotalEquipment", "TotalMaterials", "TotalServices", "FinanceSourceId",
                                               "SubDepartmentId", "CreatedDate", "ModifiedDate", "Deleted", "CreatorId",
                                               "ModifierId", oldid)
SELECT COALESCE(FD.total_eqipment, 0),
       COALESCE(FD.total_materials, 0),
       COALESCE(FD.total_services, 0),
       COALESCE(F."Id", 1),
       COALESCE(S."Id", 1),
       COALESCE(FD.created_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC', NOW() AT TIME ZONE 'UTC'),
       FD.modified_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       CASE FD.created_date IS NOT NULL WHEN TRUE THEN FD.active = FALSE ELSE FALSE END,
       COALESCE(CR."Id", 1),
       ED."Id",
       FD.id
FROM promasy.finance_dep FD
         LEFT JOIN "PromasyCore"."FinanceSources" F ON F.OldId = FD.finance_id
         LEFT JOIN "PromasyCore"."SubDepartments" S ON S.OldId = FD.subdep_id
         LEFT JOIN "PromasyCore"."Employees" CR ON CR.OldId = FD.created_by
         LEFT JOIN "PromasyCore"."Employees" ED ON ED.OldId = FD.modified_by AND FD.modified_by IS NOT NULL;

CREATE OR REPLACE FUNCTION FN_BidTypeConverter(bid_eum varchar(255)) RETURNS integer
AS
$$
DECLARE
    result integer;
BEGIN
    CASE bid_eum
        WHEN 'MATERIALS'
            THEN result = 1;
        WHEN 'EQUIPMENT'
            THEN result = 2;
        ELSE result = 3;
        END CASE;
    RETURN result;
END;
$$ LANGUAGE plpgsql;

INSERT INTO "PromasyCore"."Orders"("Amount", "Description", "CatNum", "OnePrice", "Type", "Kekv",
                                   "ProcurementStartDate",
                                   "UnitId", "CpvId", "FinanceSubDepartmentId", "ManufacturerId", "ReasonId",
                                   "SupplierId", "CreatedDate", "ModifiedDate", "Deleted", "CreatorId", "ModifierId",
                                   oldid, "OrganizationId")
SELECT B.amount,
       TRIM(B.bid_desc),
       TRIM(B.cat_num),
       B.one_price,
       FN_BidTypeConverter(B.type),
       B.kekv,
       B.proc_start_date,
       AU."Id",
       (SELECT CPV."Id" FROM "PromasyCore"."Cpvs" AS CPV WHERE CPV."Code" = B.cpv_code),
       FD."Id",
       COALESCE(PR."Id", 1),
       COALESCE(RS."Id", 1),
       COALESCE(SUP."Id", 1),
       B.created_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       B.modified_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       B.active = FALSE,
       CR."Id",
       ED."Id",
       B.id,
       D."OrganizationId"
FROM promasy.bids B
         JOIN "PromasyCore"."Units" AU on AU.OldId = B.am_unit_id
         JOIN "PromasyCore"."FinanceSubDepartments" FD on FD.OldId = B.finance_dep_id
         LEFT JOIN "PromasyCore"."Manufacturers" PR on PR.OldId = B.producer_id
         LEFT JOIN "PromasyCore"."ReasonForSupplierChoice" RS on RS.OldId = B.reason_id
         LEFT JOIN "PromasyCore"."Suppliers" SUP on SUP.OldId = B.supplier_id
         LEFT JOIN "PromasyCore"."Employees" CR ON CR.OldId = B.created_by
         LEFT JOIN "PromasyCore"."SubDepartments" SD on SD."Id" = CR."SubDepartmentId"
         LEFT JOIN "PromasyCore"."Departments" D on D."Id" = SD."DepartmentId"
         LEFT JOIN "PromasyCore"."Employees" ED ON ED.OldId = B.modified_by AND B.modified_by IS NOT NULL;

CREATE OR REPLACE FUNCTION FN_BidStatusConverter(bid_eum varchar(255)) RETURNS integer
AS
$$
DECLARE
    result integer;
BEGIN
    CASE bid_eum
        WHEN 'CREATED'
            THEN result = 1;
        WHEN 'SUBMITTED'
            THEN result = 2;
        WHEN 'POSTED_IN_PROZORRO'
            THEN result = 3;
        WHEN 'RECEIVED'
            THEN result = 4;
        WHEN 'NOT_RECEIVED'
            THEN result = 10;
        ELSE result = 20;
        END CASE;
    RETURN result;
END;
$$ LANGUAGE plpgsql;

INSERT INTO "PromasyCore"."OrderStatuses" ("Status", "OrderId", "CreatedDate", "ModifiedDate", "Deleted", "CreatorId",
                                           "ModifierId", oldid)
SELECT FN_BidStatusConverter(BS.status),
       B."Id",
       BS.created_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       BS.modified_date AT TIME ZONE 'Europe/Kiev' AT TIME ZONE 'UTC',
       BS.active = FALSE,
       CR."Id",
       ED."Id",
       BS.id
FROM promasy.bid_statuses BS
         JOIN "PromasyCore"."Orders" B ON B.OldId = BS.bid_id
         LEFT JOIN "PromasyCore"."Employees" CR ON CR.OldId = BS.created_by
         LEFT JOIN "PromasyCore"."Employees" ED ON ED.OldId = BS.modified_by AND BS.modified_by IS NOT NULL;

-- MIGRATION END

-- CLEANUP
ALTER TABLE "PromasyCore"."Addresses"
    DROP COLUMN OldId;

ALTER TABLE "PromasyCore"."Units"
    DROP COLUMN OldId;

ALTER TABLE "PromasyCore"."Orders"
    DROP COLUMN OldId;

ALTER TABLE "PromasyCore"."OrderStatuses"
    DROP COLUMN OldId;

ALTER TABLE "PromasyCore"."Departments"
    DROP COLUMN OldId;

ALTER TABLE "PromasyCore"."Employees"
    DROP COLUMN OldId;

ALTER TABLE "PromasyCore"."FinanceSubDepartments"
    DROP COLUMN OldId;

ALTER TABLE "PromasyCore"."FinanceSources"
    DROP COLUMN OldId;

ALTER TABLE "PromasyCore"."Organizations"
    DROP COLUMN OldId;

ALTER TABLE "PromasyCore"."Manufacturers"
    DROP COLUMN OldId;

ALTER TABLE "PromasyCore"."SubDepartments"
    DROP COLUMN OldId;

ALTER TABLE "PromasyCore"."Suppliers"
    DROP COLUMN OldId;

DROP FUNCTION FN_RoleConverter;

DROP FUNCTION FN_CityTypeConverter;

DROP FUNCTION FN_StreetTypeConverter;

DROP FUNCTION FN_FundTypeConverter;

DROP FUNCTION FN_BidTypeConverter;

DROP FUNCTION FN_BidStatusConverter;