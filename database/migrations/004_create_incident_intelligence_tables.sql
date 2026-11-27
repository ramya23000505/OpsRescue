-- database/migrations/004_create_incident_intelligence_tables.sql
-- Description: Creates normalized tables for Impact, Simulation, and SLA Risk modules
USE authpractice;

-- 1. Base Incidents Table
CREATE TABLE IF NOT EXISTS `incidents` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `incident_id` VARCHAR(50) NOT NULL UNIQUE,
    `incident_type` VARCHAR(150) NOT NULL,
    `root_cause` TEXT NOT NULL,
    `severity` VARCHAR(50) NOT NULL,
    `propagation_model` VARCHAR(100) NOT NULL DEFAULT 'Active Causal Graph',
    `status` VARCHAR(50) NOT NULL DEFAULT 'Synced',
    `cargo_value` DECIMAL(14,2) NOT NULL DEFAULT 0,
    `liquidated_penalty` DECIMAL(14,2) NOT NULL DEFAULT 0,
    `remaining_sla_buffer_minutes` INT NOT NULL DEFAULT 0,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 2. Impact Blast Radius Breakdown (1-to-1 with incidents)
CREATE TABLE IF NOT EXISTS `impact_blast_radiuses` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `incident_id` VARCHAR(50) NOT NULL UNIQUE,
    `root_incidents` INT NOT NULL DEFAULT 1,
    `shipments` INT NOT NULL DEFAULT 0,
    `purchase_orders` INT NOT NULL DEFAULT 0,
    `consignees` INT NOT NULL DEFAULT 0,
    CONSTRAINT `fk_blast_incident` FOREIGN KEY (`incident_id`) 
        REFERENCES `incidents` (`incident_id`) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- 3. Impact Exposure Dossier (Consignees at risk)
CREATE TABLE IF NOT EXISTS `impact_exposure_dossiers` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `incident_id` VARCHAR(50) NOT NULL,
    `entity_id` VARCHAR(50) NOT NULL,
    `entity_name` VARCHAR(150) NOT NULL,
    `tier` VARCHAR(100) NOT NULL,
    `risk_level` VARCHAR(50) NOT NULL,
    `location` VARCHAR(150) NOT NULL,
    `po_reference` VARCHAR(50) NOT NULL,
    `value_at_risk` DECIMAL(14,2) NOT NULL DEFAULT 0,
    `buffer_remaining_minutes` INT NOT NULL DEFAULT 0,
    `sla_penalty_rate` VARCHAR(100) NOT NULL,
    `clinical_or_business_impact` TEXT NOT NULL,
    `assigned_carrier` VARCHAR(100) NOT NULL,
    `cargo_details` TEXT NOT NULL,
    CONSTRAINT `fk_dossier_incident` FOREIGN KEY (`incident_id`) 
        REFERENCES `incidents` (`incident_id`) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- 4. Impact Graph Nodes
CREATE TABLE IF NOT EXISTS `impact_graph_nodes` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `incident_id` VARCHAR(50) NOT NULL,
    `node_id` VARCHAR(50) NOT NULL,
    `label` VARCHAR(100) NOT NULL,
    `tier_level` INT NOT NULL,
    `node_type` VARCHAR(50) NOT NULL,
    `status` VARCHAR(100) NOT NULL,
    `subtitle` VARCHAR(150) NOT NULL DEFAULT '',
    `attributes_json` TEXT NULL,
    CONSTRAINT `fk_nodes_incident` FOREIGN KEY (`incident_id`) 
        REFERENCES `incidents` (`incident_id`) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- 5. Impact Graph Edges
CREATE TABLE IF NOT EXISTS `impact_graph_edges` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `incident_id` VARCHAR(50) NOT NULL,
    `from_node` VARCHAR(50) NOT NULL,
    `to_node` VARCHAR(50) NOT NULL,
    `relationship` VARCHAR(50) NOT NULL,
    `label` VARCHAR(100) NULL,
    `severity` VARCHAR(50) NOT NULL DEFAULT 'High',
    CONSTRAINT `fk_edges_incident` FOREIGN KEY (`incident_id`) 
        REFERENCES `incidents` (`incident_id`) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- 6. Incident Simulations Metadata (1-to-1)
CREATE TABLE IF NOT EXISTS `incident_simulations` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `incident_id` VARCHAR(50) NOT NULL UNIQUE,
    `shipment_id` VARCHAR(50) NOT NULL,
    `recommended_strategy_id` VARCHAR(50) NOT NULL,
    `recommendation_summary` TEXT NOT NULL,
    `key_tradeoff_finding` TEXT NOT NULL,
    `why_recommended` TEXT NOT NULL,
    `why_rejected_alternatives` TEXT NOT NULL,
    CONSTRAINT `fk_sim_incident` FOREIGN KEY (`incident_id`) 
        REFERENCES `incidents` (`incident_id`) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- 7. Simulation Strategies
CREATE TABLE IF NOT EXISTS `simulation_strategies` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `incident_id` VARCHAR(50) NOT NULL,
    `strategy_id` VARCHAR(50) NOT NULL,
    `strategy_number` INT NOT NULL,
    `title` VARCHAR(150) NOT NULL,
    `category_tag` VARCHAR(50) NOT NULL,
    `description` TEXT NOT NULL,
    `is_recommended` BOOLEAN NOT NULL DEFAULT FALSE,
    `recommendation_badge` VARCHAR(100) NULL,
    `predicted_delivery_time` VARCHAR(50) NOT NULL,
    `delivery_margin` VARCHAR(50) NOT NULL,
    `estimated_outlay` DECIMAL(12,2) NOT NULL DEFAULT 0,
    `outlay_breakdown` VARCHAR(100) NOT NULL,
    `expected_sla_outcome` VARCHAR(100) NOT NULL,
    `sla_success_probability` DECIMAL(5,2) NOT NULL DEFAULT 0,
    `liquidated_damages_impact` DECIMAL(12,2) NOT NULL DEFAULT 0,
    `is_damages_avoided` BOOLEAN NOT NULL DEFAULT FALSE,
    `operational_protocol` TEXT NOT NULL,
    `identified_constraints` TEXT NULL,
    `operational_governance` TEXT NULL,
    `required_approval_tier` VARCHAR(50) NOT NULL DEFAULT 'Tier 1',
    `governance_alert` TEXT NULL,
    CONSTRAINT `fk_strat_incident` FOREIGN KEY (`incident_id`) 
        REFERENCES `incidents` (`incident_id`) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- 8. SLA Risk Assessments (1-to-1)
CREATE TABLE IF NOT EXISTS `sla_risk_assessments` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `incident_id` VARCHAR(50) NOT NULL UNIQUE,
    `shipment_id` VARCHAR(50) NOT NULL,
    `route` VARCHAR(150) NOT NULL,
    `severity` VARCHAR(50) NOT NULL,
    `breach_probability` DECIMAL(5,2) NOT NULL DEFAULT 0,
    `risk_tier` VARCHAR(50) NOT NULL,
    `risk_baseline_delta` VARCHAR(50) NOT NULL,
    `time_to_sla_breach_minutes` INT NOT NULL DEFAULT 0,
    `hard_breach_threshold_time` VARCHAR(50) NOT NULL,
    `projected_delay` VARCHAR(50) NOT NULL,
    `committed_sla_window` VARCHAR(50) NOT NULL,
    `financial_exposure` DECIMAL(12,2) NOT NULL DEFAULT 0,
    `financial_exposure_type` VARCHAR(100) NOT NULL DEFAULT 'Contractual liquidated damages',
    `remaining_buffer_badge` VARCHAR(100) NOT NULL,
    CONSTRAINT `fk_sla_incident` FOREIGN KEY (`incident_id`) 
        REFERENCES `incidents` (`incident_id`) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- 9. SLA Trend Points
CREATE TABLE IF NOT EXISTS `sla_trend_points` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `incident_id` VARCHAR(50) NOT NULL,
    `sort_order` INT NOT NULL DEFAULT 0,
    `time_label` VARCHAR(50) NOT NULL,
    `normal_transit_probability` DECIMAL(5,2) NOT NULL DEFAULT 0,
    `breach_curve_probability` DECIMAL(5,2) NOT NULL DEFAULT 0,
    `is_current_point` BOOLEAN NOT NULL DEFAULT FALSE,
    `is_hard_breach_threshold` BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT `fk_trend_incident` FOREIGN KEY (`incident_id`) 
        REFERENCES `incidents` (`incident_id`) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- 10. SLA Contributing Factors
CREATE TABLE IF NOT EXISTS `sla_contributing_factors` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `incident_id` VARCHAR(50) NOT NULL,
    `sort_order` INT NOT NULL DEFAULT 0,
    `factor_name` VARCHAR(100) NOT NULL,
    `severity_badge` VARCHAR(100) NOT NULL,
    `contribution_percentage` INT NOT NULL DEFAULT 0,
    `description` TEXT NOT NULL,
    `key_metrics_json` TEXT NULL,
    CONSTRAINT `fk_factors_incident` FOREIGN KEY (`incident_id`) 
        REFERENCES `incidents` (`incident_id`) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- 11. SLA Shipment Context (1-to-1)
CREATE TABLE IF NOT EXISTS `sla_shipment_contexts` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `incident_id` VARCHAR(50) NOT NULL UNIQUE,
    `consignee` VARCHAR(150) NOT NULL,
    `master_sla` VARCHAR(150) NOT NULL,
    `criticality` VARCHAR(150) NOT NULL,
    `temperature_status` VARCHAR(100) NOT NULL,
    `assigned_vehicle` VARCHAR(100) NOT NULL,
    CONSTRAINT `fk_context_incident` FOREIGN KEY (`incident_id`) 
        REFERENCES `incidents` (`incident_id`) 
        ON DELETE CASCADE ON UPDATE CASCADE
);
