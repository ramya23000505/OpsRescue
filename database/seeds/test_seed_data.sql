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

-- 5. SLA Assessments (Module 3 Demo Data)
INSERT INTO `sla_assessments` 
(`incident_id`, `shipment_id`, `breach_probability`, `projected_delay_minutes`, `time_to_breach_minutes`, `financial_exposure`)
VALUES 
('INC-1001', 'SHP-1042', 94.20, 108, 38, 135000.00),
('INC-1002', 'SHP-1058', 88.50, 75, 45, 95000.00);
-- 6. Recovery Strategies (Module 4 Demo Data for INC-1001)
INSERT INTO `recovery_strategies` 
(`strategy_code`, `incident_id`, `title`, `category`, `predicted_delivery`, `estimated_outlay`, `sla_success_probability`, `is_recommended`, `required_approval_tier`)
VALUES 
('STRAT-01', 'INC-1001', 'Alternate Route Bypass', 'Continuation', '11:42 AM IST', 18500.00, 42.00, FALSE, 'Tier 1'),
('STRAT-02', 'INC-1001', 'Relief Cross-Dock Transfer', 'Optimized', '11:24 AM IST', 34500.00, 97.80, TRUE, 'Tier 2'),
('STRAT-03', 'INC-1001', 'Secondary Depot Swap', 'Stock Swap', '11:28 AM IST', 58000.00, 98.20, FALSE, 'Tier 4');

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

SELECT 
    sla.incident_id,
    sla.shipment_id,
    CONCAT(sla.breach_probability, '%') AS risk_probability,
    CONCAT(sla.projected_delay_minutes, ' mins (+1h 48m)') AS projected_delay,
    CONCAT(sla.time_to_breach_minutes, ' mins') AS remaining_buffer,
    CONCAT('₹', FORMAT(sla.financial_exposure, 2)) AS revenue_at_risk,
    rec.strategy_code,
    rec.title AS strategy_title,
    rec.category AS strategy_type,
    rec.predicted_delivery,
    CONCAT('₹', FORMAT(rec.estimated_outlay, 2)) AS outlay_cost,
    CONCAT(rec.sla_success_probability, '%') AS success_chance,
    CASE WHEN rec.is_recommended = 1 THEN '★ YES (Recommended)' ELSE 'NO' END AS is_recommended,
    rec.required_approval_tier
FROM `sla_assessments` sla
INNER JOIN `recovery_strategies` rec 
    ON sla.incident_id = rec.incident_id
WHERE sla.incident_id = 'INC-1001'
ORDER BY rec.is_recommended DESC, rec.sla_success_probability DESC;

SELECT 'users' AS table_name, COUNT(*) AS record_count FROM `users`
UNION ALL
SELECT 'operations_manager_profiles', COUNT(*) FROM `operations_manager_profiles`
UNION ALL
SELECT 'operations_user_profiles', COUNT(*) FROM `operations_user_profiles`
UNION ALL
SELECT 'delivery_partner_profiles', COUNT(*) FROM `delivery_partner_profiles`
UNION ALL
SELECT 'sla_assessments', COUNT(*) FROM `sla_assessments`
UNION ALL
SELECT 'recovery_strategies', COUNT(*) FROM `recovery_strategies`;