-- database/migrations/001_update_users_table.sql
-- Description: Non-destructive addition of role-based registration columns to `users`
CREATE DATABASE IF NOT EXISTS authpractice;
USE authpractice;

-- 1. Ensure required columns exist
CREATE TABLE IF NOT EXISTS `users` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `firstname` VARCHAR(100) NOT NULL,
    `lastname` VARCHAR(100) NOT NULL,
    `username` VARCHAR(100) NOT NULL UNIQUE,
    `email` VARCHAR(150) NOT NULL UNIQUE,
    `phone_number` VARCHAR(20) NOT NULL DEFAULT '',
    `password` VARCHAR(255) NOT NULL,
    `role` VARCHAR(50) NOT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 2. Add Unique Constraints for username and email (safely checked)
SET @dbname = DATABASE();
SET @tablename = "users";

-- Username unique index
SET @exist := (SELECT COUNT(*) FROM information_schema.statistics 
               WHERE table_schema = @dbname AND table_name = @tablename AND index_name = 'uq_users_username');
SET @sqlstmt := IF(@exist = 0, 'ALTER TABLE users ADD CONSTRAINT uq_users_username UNIQUE (username);', 'SELECT "uq_users_username exists";');
PREPARE stmt FROM @sqlstmt;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Email unique index
SET @exist := (SELECT COUNT(*) FROM information_schema.statistics 
               WHERE table_schema = @dbname AND table_name = @tablename AND index_name = 'uq_users_email');
SET @sqlstmt := IF(@exist = 0, 'ALTER TABLE users ADD CONSTRAINT uq_users_email UNIQUE (email);', 'SELECT "uq_users_email exists";');
PREPARE stmt FROM @sqlstmt;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 3. Check Constraint for approved roles only
SET @exist := (SELECT COUNT(*) FROM information_schema.table_constraints 
               WHERE table_schema = @dbname AND table_name = @tablename AND constraint_name = 'chk_users_approved_role');
SET @sqlstmt := IF(@exist = 0, 
    "ALTER TABLE users ADD CONSTRAINT chk_users_approved_role CHECK (role IN ('Admin', 'Operations Manager', 'Operations User', 'Delivery Partner'));", 
    'SELECT "chk_users_approved_role exists";');
PREPARE stmt FROM @sqlstmt;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;