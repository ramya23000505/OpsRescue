-- database/migrations/003_role_integrity_triggers.sql
-- Description: Ensures profile records match the exact user.role assigned
USE authpractice;

DELIMITER $$

DROP TRIGGER IF EXISTS trg_check_omp_role_before_insert$$
CREATE TRIGGER trg_check_omp_role_before_insert
BEFORE INSERT ON operations_manager_profiles
FOR EACH ROW
BEGIN
    DECLARE v_role VARCHAR(50);
    SELECT role INTO v_role FROM users WHERE id = NEW.user_id;
    IF v_role IS NULL OR v_role != 'Operations Manager' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Integrity Violation: user_id is not assigned the role Operations Manager';
    END IF;
END$$

DROP TRIGGER IF EXISTS trg_check_oup_role_before_insert$$
CREATE TRIGGER trg_check_oup_role_before_insert
BEFORE INSERT ON operations_user_profiles
FOR EACH ROW
BEGIN
    DECLARE v_role VARCHAR(50);
    SELECT role INTO v_role FROM users WHERE id = NEW.user_id;
    IF v_role IS NULL OR v_role != 'Operations User' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Integrity Violation: user_id is not assigned the role Operations User';
    END IF;
END$$

DROP TRIGGER IF EXISTS trg_check_dpp_role_before_insert$$
CREATE TRIGGER trg_check_dpp_role_before_insert
BEFORE INSERT ON delivery_partner_profiles
FOR EACH ROW
BEGIN
    DECLARE v_role VARCHAR(50);
    SELECT role INTO v_role FROM users WHERE id = NEW.user_id;
    IF v_role IS NULL OR v_role != 'Delivery Partner' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Integrity Violation: user_id is not assigned the role Delivery Partner';
    END IF;
END$$

DELIMITER ;