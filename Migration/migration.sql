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

ALTER TABLE "PromasyCore"."FinanceDepartments"
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
FROM (SELECT C."Id" AS Id,
       (SELECT CP."Id" FROM "PromasyCore"."Cpvs" CP WHERE CP."Level" = C."Level" - 1 AND CP."Code" LIKE concat(substr(C."Code", 1, C."Level"), '0%')) AS ParentId
FROM "PromasyCore"."Cpvs" C
WHERE C."Level" > 1) AS SQ
WHERE "Level" > 1 AND "Id" = SQ.Id AND SQ.ParentId IS NOT NULL;

INSERT INTO "PromasyCore"."Addresses" ("CreatedDate", "ModifiedDate", "Deleted", oldid, "BuildingNumber", "City",
                                       "CityType", "CorpusNumber", "Country", "PostalCode", "Region", "Street",
                                       "StreetType", "CreatorId")
SELECT created_date,
       modified_date,
       active = FALSE,
       id,
       building_number,
       city,
       citytype,
       corpus_number,
       country,
       postal_code,
       region,
       street,
       streettype,
       0
FROM promasy.addresses;

INSERT INTO "PromasyCore"."Organizations" ("CreatedDate", "ModifiedDate", "Deleted", oldid, "Name", "Email", "Edrpou",
                                        "FaxNumber", "PhoneNumber", "AddressId", "CreatorId")
SELECT I.created_date,
       I.modified_date,
       I.active = FALSE,
       I.id,
       I.inst_name,
       I.email,
       I.edrpou,
       I.fax_number,
       I.phone_number,
       AD."Id",
       0
FROM promasy.institutes I
         JOIN "PromasyCore"."Addresses" AD ON AD.OldId = address_id;

INSERT INTO "PromasyCore"."Departments" ("CreatedDate", "ModifiedDate", "Deleted", oldid, "Name", "OrganizationId", "CreatorId")
SELECT D.created_date, D.modified_date, D.active = FALSE, D.id, D.dep_name, I."Id", 0
FROM promasy.departments D
         JOIN "PromasyCore"."Organizations" I ON I.OldId = D.inst_id;

INSERT INTO "PromasyCore"."SubDepartments" ("CreatedDate", "ModifiedDate", "Deleted", oldid, "Name", "DepartmentId", "CreatorId")
SELECT S.created_date, S.modified_date, S.active = FALSE, S.id, S.subdep_name, D."Id", 0
FROM promasy.subdepartments S
         JOIN "PromasyCore"."Departments" D ON D.OldId = S.dep_id;

INSERT INTO "PromasyCore"."Employees" ("CreatedDate", "ModifiedDate", "Deleted", oldid, "FirstName", "MiddleName",
                                       "LastName", "PrimaryPhone", "ReservePhone", "SubDepartmentId", "UserName",
                                       "Email", "CreatorId", "Password", "Salt")
SELECT E.created_date,
       E.modified_date,
       E.active = FALSE,
       E.id,
       TRIM(E.emp_fname),
       TRIM(E.emp_mname),
       TRIM(E.emp_lname),
       TRANSLATE(TRIM(E.phone_main), '- ()', ''),
       TRANSLATE(TRIM(E.phone_reserve), '- ()', ''),
       SUB."Id",
       E.login,
       TRIM(E.email),
       0,
       E.password,
       E.salt
FROM promasy.employees E
         JOIN "PromasyCore"."SubDepartments" SUB ON SUB.OldId = E.subdep_id;

UPDATE "PromasyCore"."Addresses"
SET "CreatorId" = (SELECT "Id" FROM "PromasyCore"."Employees" WHERE OLdId = 1)
WHERE "CreatedDate" IS NOT NULL;
UPDATE "PromasyCore"."Addresses"
SET "ModifierId" = (SELECT "Id" FROM "PromasyCore"."Employees" WHERE OLdId = 1)
WHERE "ModifiedDate" IS NOT NULL;
UPDATE "PromasyCore"."Organizations"
SET "CreatorId" = (SELECT "Id" FROM "PromasyCore"."Employees" WHERE OLdId = 1)
WHERE "CreatedDate" IS NOT NULL;
UPDATE "PromasyCore"."Organizations"
SET "ModifierId" = (SELECT "Id" FROM "PromasyCore"."Employees" WHERE OLdId = 1)
WHERE "ModifiedDate" IS NOT NULL;
UPDATE "PromasyCore"."Departments"
SET "CreatorId" = (SELECT "Id" FROM "PromasyCore"."Employees" WHERE OLdId = 1)
WHERE "CreatedDate" IS NOT NULL;
UPDATE "PromasyCore"."Departments"
SET "ModifierId" = (SELECT "Id" FROM "PromasyCore"."Employees" WHERE OLdId = 1)
WHERE "ModifiedDate" IS NOT NULL;
UPDATE "PromasyCore"."SubDepartments"
SET "CreatorId" = (SELECT "Id" FROM "PromasyCore"."Employees" WHERE OLdId = 1)
WHERE "CreatedDate" IS NOT NULL;
UPDATE "PromasyCore"."SubDepartments"
SET "ModifierId" = (SELECT "Id" FROM "PromasyCore"."Employees" WHERE OLdId = 1)
WHERE "ModifiedDate" IS NOT NULL;

CREATE OR REPLACE FUNCTION FN_RoleConverter(role_eum text) RETURNS integer
AS
$$
DECLARE
    result    integer;
    role_name text;
BEGIN
    CASE role_eum
        WHEN 'ADMIN'
            THEN role_name = 'Адміністратор';
        WHEN 'DIRECTOR'
            THEN role_name = 'Директор';
        WHEN 'DEPUTY_DIRECTOR'
            THEN role_name = 'Заступник директора';
        WHEN 'HEAD_OF_TENDER_COMMITTEE'
            THEN role_name = 'Голова тендерного комітету';
        WHEN 'SECRETARY_OF_TENDER_COMMITTEE'
            THEN role_name = 'Секретар тендерного комітету';
        WHEN 'ECONOMIST'
            THEN role_name = 'Головний економіст';
        WHEN 'ACCOUNTANT'
            THEN role_name = 'Головний бухгалтер';
        WHEN 'HEAD_OF_DEPARTMENT'
            THEN role_name = 'Керівник підрозділу';
        WHEN 'PERSONALLY_LIABLE_EMPLOYEE'
            THEN role_name = 'Матеріально-відповідальна особа';
        ELSE role_name = 'Користувач';
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
                                        "ModifierId", oldid)
SELECT TRIM(A.amount_unit_desc), A.created_date, A.modified_date, A.active = FALSE, CR."Id", ED."Id", A.id
FROM promasy.amount_units A
         LEFT JOIN "PromasyCore"."Employees" CR ON CR.OldId = A.created_by
         LEFT JOIN "PromasyCore"."Employees" ED ON ED.OldId = A.modified_by AND A.modified_by IS NOT NULL;

INSERT INTO "PromasyCore"."Manufacturers"("Name", "CreatedDate", "ModifiedDate", "Deleted", "CreatorId", "ModifierId",
                                      OldId)
SELECT TRIM(P.brand_name), P.created_date, P.modified_date, P.active = FALSE, CR."Id", ED."Id", P.id
FROM promasy.producers P
         LEFT JOIN "PromasyCore"."Employees" CR ON CR.OldId = P.created_by
         LEFT JOIN "PromasyCore"."Employees" ED ON ED.OldId = P.modified_by AND P.modified_by IS NOT NULL;

INSERT INTO "PromasyCore"."Suppliers"("Name", "Comment", "Phone", "CreatedDate", "ModifiedDate", "Deleted", "CreatorId",
                                      "ModifierId", oldid)
SELECT TRIM(S.supplier_name),
       TRIM(S.supplier_comments),
       TRANSLATE(TRIM(S.supplier_tel), '- ()', ''),
       S.created_date,
       S.modified_date,
       S.active = FALSE,
       CR."Id",
       ED."Id",
       S.id
FROM promasy.suppliers S
         LEFT JOIN "PromasyCore"."Employees" CR ON CR.OldId = S.created_by
         LEFT JOIN "PromasyCore"."Employees" ED ON ED.OldId = S.modified_by AND S.modified_by IS NOT NULL;

INSERT INTO "PromasyCore"."ReasonForSupplierChoice"("Name", "CreatedDate", "ModifiedDate", "Deleted", "CreatorId",
                                               "ModifierId", oldid)
SELECT TRIM(R.reason_name), R.created_date, R.modified_date, R.active = FALSE, CR."Id", ED."Id", R.id
FROM promasy.reasons_for_suppl R
         LEFT JOIN "PromasyCore"."Employees" CR ON CR.OldId = R.created_by
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

INSERT INTO "PromasyCore"."FinanceSources"("Name", "Number", "FundType", "Kpkvk", "StartsOn", "DueTo", "TotalEquipment",
                                           "TotalMaterials", "TotalServices", "CreatedDate", "ModifiedDate", "Deleted",
                                           "CreatorId", "ModifierId", oldid)
SELECT TRIM(F.name),
       TRIM(F.number),
       FN_FundTypeConverter(F.fundtype),
       F.kpkvk,
       F.starts_on,
       F.due_to,
       F.total_equpment,
       F.total_materials,
       F.total_services,
       F.created_date,
       F.modified_date,
       F.active = false,
       CR."Id",
       ED."Id",
       F.id
FROM promasy.finances F
         LEFT JOIN "PromasyCore"."Employees" CR ON CR.OldId = F.created_by
         LEFT JOIN "PromasyCore"."Employees" ED ON ED.OldId = F.modified_by AND F.modified_by IS NOT NULL;

INSERT INTO "PromasyCore"."FinanceDepartments"("TotalEquipment", "TotalMaterials", "TotalServices", "FinanceSourceId",
                                               "SubDepartmentId", "CreatedDate", "ModifiedDate", "Deleted", "CreatorId",
                                               "ModifierId", oldid)
SELECT COALESCE(FD.total_eqipment, 0),
       COALESCE(FD.total_materials, 0),
       COALESCE(FD.total_services, 0),
       COALESCE(F."Id", 1),
       COALESCE(S."Id", 1),
       COALESCE(FD.created_date, NOW()),
       FD.modified_date,
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

INSERT INTO "PromasyCore"."Orders"("Amount", "Description", "CatNum", "OnePrice", "Type", "Kekv", "ProcurementStartDate",
                                 "UnitId", "CpvId", "FinanceDepartmentId", "ManufacturerId", "ReasonId",
                                 "SupplierId", "CreatedDate", "ModifiedDate", "Deleted", "CreatorId", "ModifierId",
                                 oldid)
SELECT B.amount,
       TRIM(B.bid_desc),
       TRIM(B.cat_num),
       B.one_price,
       FN_BidTypeConverter(B.type),
       B.kekv,
       B.proc_start_date,
       AU."Id",
       (SELECT Id FROM "PromasyCore"."Cpvs" WHERE "Code" = B.cpv_code),
       FD."Id",
       COALESCE(PR."Id", 1),
       COALESCE(RS."Id", 1),
       COALESCE(SUP."Id", 1),
       B.created_date,
       B.modified_date,
       B.active = FALSE,
       CR."Id",
       ED."Id",
       B.id
FROM promasy.bids B
         JOIN "PromasyCore"."Units" AU on AU.OldId = B.am_unit_id
         JOIN "PromasyCore"."FinanceDepartments" FD on FD.OldId = B.finance_dep_id
         LEFT JOIN "PromasyCore"."Manufacturers" PR on PR.OldId = B.producer_id
         LEFT JOIN "PromasyCore"."ReasonForSupplierChoice" RS on RS.OldId = B.reason_id
         LEFT JOIN "PromasyCore"."Suppliers" SUP on SUP.OldId = B.supplier_id
         LEFT JOIN "PromasyCore"."Employees" CR ON CR.OldId = B.created_by
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
       BS.created_date,
       BS.modified_date,
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

ALTER TABLE "PromasyCore"."FinanceDepartments"
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

DROP FUNCTION FN_FundTypeConverter;

DROP FUNCTION FN_BidTypeConverter;

DROP FUNCTION FN_BidStatusConverter;