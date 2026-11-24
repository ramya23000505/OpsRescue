-- File: database/migrations/004_create_sla_and_recovery_tables.sql
-- Description: Creates operational tables for Module 3 (SLA Risk Engine)
-- and Module 4 (Recovery Simulator).

USE `authpractice`;

-- 1. SLA Assessments (Historical Risk Snapshots)
CREATE TABLE IF NOT EXISTS `sla_assessments` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `incident_id` VARCHAR(50) NOT NULL,
    `shipment_id` VARCHAR(50) NOT NULL,
    `breach_probability` DECIMAL(5,2) NOT NULL,
    `projected_delay_minutes` INT NOT NULL,
    `time_to_breach_minutes` INT NOT NULL,
    `financial_exposure` DECIMAL(12,2) NOT NULL,
    `assessed_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 2. Recovery Strategies (Simulator Candidate Strategies)
CREATE TABLE IF NOT EXISTS `recovery_strategies` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `strategy_code` VARCHAR(50) NOT NULL UNIQUE,
    `incident_id` VARCHAR(50) NOT NULL,
    `title` VARCHAR(150) NOT NULL,
    `category` VARCHAR(50) NOT NULL,
    `predicted_delivery` VARCHAR(50) NOT NULL,
    `estimated_outlay` DECIMAL(10,2) NOT NULL,
    `sla_success_probability` DECIMAL(5,2) NOT NULL,
    `is_recommended` BOOLEAN NOT NULL DEFAULT FALSE,
    `required_approval_tier` VARCHAR(50) NOT NULL
);