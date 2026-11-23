-- database/seeds/test_seed_data.sql
USE authpractice;

-- 1. Insert Operations Manager
START TRANSACTION;
INSERT INTO users (firstname, lastname, username, email, phone_number, password, role)
VALUES ('Alex', 'Mercer', 'alex_mercer', 'alex.mercer@opsrescue.com', '+919812345678', 
        'M@nsoon84!Puddle$Walk', 'Operations Manager');
SET @mgr_id = LAST_INSERT_ID();

INSERT INTO operations_manager_profiles (user_id, employee_id, department, branch_location, designation)
VALUES (@mgr_id, 'EMP-OM-901', 'Cold-Chain Logistics', 'Hosur Logistics Hub', 'Tier 2 Manager');
COMMIT;

-- 2. Insert Operations User
START TRANSACTION;
INSERT INTO users (firstname, lastname, username, email, phone_number, password, role)
VALUES ('Kiran', 'Sharma', 'kiran_ops', 'kiran.sharma@opsrescue.com', '+919812345679', 
        '79#FilterCoffee*Breeze', 'Operations User');
SET @usr_id = LAST_INSERT_ID();

INSERT INTO operations_user_profiles (user_id, employee_id, department, branch_location, designation)
VALUES (@usr_id, 'EMP-OU-402', 'Incident Triage', 'BLR South Cross-Dock', 'Triage Lead');
COMMIT;

-- 3. Insert Delivery Partner
START TRANSACTION;
INSERT INTO users (firstname, lastname, username, email, phone_number, password, role)
VALUES ('Murugan', 'D', 'driver_murugan', 'murugan@transexpress.com', '+919812345680', 
        'T@raco82^Lantern%Trail', 'Delivery Partner');
SET @drv_id = LAST_INSERT_ID();

INSERT INTO delivery_partner_profiles (user_id, partner_id, company_name, vehicle_number, operating_location)
VALUES (@drv_id, 'PARTNER-TRK-882', 'TransExpress Logistics', 'TRK-882 (Reefer)', 'NH-48 Corridor');
COMMIT;

-- 4. Verify Unified Output
SELECT 
    u.id AS user_id,
    CONCAT(u.firstname, ' ', u.lastname) AS full_name,
    u.username,
    u.email,
    u.role,
    COALESCE(omp.employee_id, oup.employee_id, 'N/A') AS emp_id,
    COALESCE(dpp.partner_id, 'N/A') AS partner_id,
    COALESCE(dpp.vehicle_number, 'N/A') AS vehicle
FROM users u
LEFT JOIN operations_manager_profiles omp ON u.id = omp.user_id
LEFT JOIN operations_user_profiles oup ON u.id = oup.user_id
LEFT JOIN delivery_partner_profiles dpp ON u.id = dpp.user_id;