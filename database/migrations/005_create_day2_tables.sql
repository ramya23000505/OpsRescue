-- database/migrations/005_create_day2_tables.sql
-- Description: Creates tables for Incident Timelines, Module 6 Recovery Execution, Module 5 Manager Approval Logs, and Module 9 Trust Score
USE authpractice;

-- 1. Incident Timeline Events (Module 7: Timeline & History Modal)
CREATE TABLE IF NOT EXISTS `incident_timeline_events` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `incident_id` VARCHAR(50) NOT NULL,
    `event_time` DATETIME NOT NULL,
    `stage` VARCHAR(100) NOT NULL,
    `title` VARCHAR(200) NOT NULL,
    `description` TEXT NOT NULL,
    `severity_badge` VARCHAR(50) NOT NULL,
    `source` VARCHAR(100) NOT NULL DEFAULT 'IoT Telematics',
    CONSTRAINT `fk_timeline_incident` FOREIGN KEY (`incident_id`)
        REFERENCES `incidents` (`incident_id`)
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- 2. Module 6: Governed Recovery Executions
CREATE TABLE IF NOT EXISTS `recovery_executions` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `execution_id` VARCHAR(50) NOT NULL UNIQUE,
    `incident_id` VARCHAR(50) NOT NULL,
    `strategy_id` VARCHAR(50) NOT NULL,
    `strategy_name` VARCHAR(200) NOT NULL,
    `dispatched_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `estimated_rendezvous_at` DATETIME NULL,
    `actual_rendezvous_at` DATETIME NULL,
    `resolved_at` DATETIME NULL,
    `status` VARCHAR(50) NOT NULL DEFAULT 'Dispatched', -- Dispatched, Rendezvous, Resolved
    `assigned_partner` VARCHAR(150) NOT NULL,
    `vehicle_id` VARCHAR(100) NOT NULL,
    `current_location` VARCHAR(150) NOT NULL,
    `action_notes` TEXT NULL,
    CONSTRAINT `fk_execution_incident` FOREIGN KEY (`incident_id`)
        REFERENCES `incidents` (`incident_id`)
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- 3. Module 5: Manager Approval Logs (Audit Log)
CREATE TABLE IF NOT EXISTS `manager_approval_logs` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `incident_id` VARCHAR(50) NOT NULL,
    `strategy_id` VARCHAR(50) NOT NULL,
    `strategy_name` VARCHAR(200) NOT NULL,
    `approved_by` VARCHAR(100) NOT NULL,
    `approved_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `approval_tier` VARCHAR(50) NOT NULL DEFAULT 'Tier 1',
    `notes` TEXT NULL,
    CONSTRAINT `fk_approval_incident` FOREIGN KEY (`incident_id`)
        REFERENCES `incidents` (`incident_id`)
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- 4. Module 9: Accountability & Trust Score
CREATE TABLE IF NOT EXISTS `accountability_trust_scores` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `incident_id` VARCHAR(50) NOT NULL,
    `recovery_strategy_id` VARCHAR(50) NOT NULL,
    `predicted_recovery_time_mins` INT NOT NULL,
    `actual_recovery_time_mins` INT NOT NULL,
    `trust_score_pct` DECIMAL(5,2) NOT NULL,
    `prediction_accuracy_grade` VARCHAR(50) NOT NULL,
    `variance_reason` TEXT NOT NULL,
    `evaluated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT `fk_trust_incident` FOREIGN KEY (`incident_id`)
        REFERENCES `incidents` (`incident_id`)
        ON DELETE CASCADE ON UPDATE CASCADE
);
