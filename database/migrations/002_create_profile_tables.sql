-- database/migrations/002_create_profile_tables.sql
-- Description: Creates 1-to-1 profile tables with foreign key cascades and unique constraints
USE authpractice;

-- 1. Operations Manager Profile Table
CREATE TABLE IF NOT EXISTS `operations_manager_profiles` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `user_id` INT NOT NULL UNIQUE,
    `employee_id` VARCHAR(50) NOT NULL UNIQUE,
    `department` VARCHAR(100) NOT NULL,
    `branch_location` VARCHAR(100) NOT NULL,
    `designation` VARCHAR(100) NOT NULL,
    CONSTRAINT `fk_ops_mgr_user` FOREIGN KEY (`user_id`) 
        REFERENCES `users` (`id`) 
        ON DELETE CASCADE 
        ON UPDATE CASCADE
);

-- 2. Operations User Profile Table
CREATE TABLE IF NOT EXISTS `operations_user_profiles` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `user_id` INT NOT NULL UNIQUE,
    `employee_id` VARCHAR(50) NOT NULL UNIQUE,
    `department` VARCHAR(100) NOT NULL,
    `branch_location` VARCHAR(100) NOT NULL,
    `designation` VARCHAR(100) NOT NULL,
    CONSTRAINT `fk_ops_usr_user` FOREIGN KEY (`user_id`) 
        REFERENCES `users` (`id`) 
        ON DELETE CASCADE 
        ON UPDATE CASCADE
);

-- 3. Delivery Partner Profile Table
CREATE TABLE IF NOT EXISTS `delivery_partner_profiles` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `user_id` INT NOT NULL UNIQUE,
    `partner_id` VARCHAR(50) NOT NULL UNIQUE,
    `company_name` VARCHAR(150) NOT NULL,
    `vehicle_number` VARCHAR(50) NOT NULL,
    `operating_location` VARCHAR(100) NOT NULL,
    CONSTRAINT `fk_dp_user` FOREIGN KEY (`user_id`) 
        REFERENCES `users` (`id`) 
        ON DELETE CASCADE 
        ON UPDATE CASCADE
);