-- database/seeds/003_seed_day2_data.sql
-- Description: Seed data for Timeline Events, Recovery Executions, Approvals, and Trust Scores
USE authpractice;

SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE `accountability_trust_scores`;
TRUNCATE TABLE `manager_approval_logs`;
TRUNCATE TABLE `recovery_executions`;
TRUNCATE TABLE `incident_timeline_events`;
SET FOREIGN_KEY_CHECKS = 1;

-- =========================================================================
-- 1. INCIDENT TIMELINE EVENTS (Module 7: Timeline & History Modal)
-- =========================================================================
INSERT INTO `incident_timeline_events`
(`incident_id`, `event_time`, `stage`, `title`, `description`, `severity_badge`, `source`)
VALUES
-- INC-1004 (Cold Chain / Reefer failure - Hero scenario)
('INC-1004', '2026-11-25 09:30:00', 'Event Ingestion', 'Cold-Chain IoT Sensor Alert Triggered', 'Reefer Unit TRK-882 telemetry flagged compressor cycle drop and thermal rise to +4.5°C on NH-544.', 'CRITICAL P1', 'Carrier IoT Telematics'),
('INC-1004', '2026-11-25 09:32:00', 'Exception Detection', 'Automated Anomaly Verification', 'OpsRescue Engine verified temperature violation against critical bio-pharma threshold (+2°C to +8°C).', 'CRITICAL P1', 'Rule Engine'),
('INC-1004', '2026-11-25 09:34:15', 'Impact Propagation', 'Causal Impact Graph Computed', 'Determined blast radius of 11 entities (4 consignees including KMCH Hospital, ₹ 1.15 Cr cargo at risk).', 'HIGH P2', 'Impact Engine'),
('INC-1004', '2026-11-25 09:35:40', 'SLA Risk Assessment', 'Breach Probability Reached 96.5%', 'Calculated remaining buffer at 22 minutes. Liquidated damage exposure assessed at ₹ 95,000.', 'CRITICAL P1', 'SLA Risk Engine'),
('INC-1004', '2026-11-25 09:37:00', 'Recovery Simulation', '3 Recovery Paths Simulated', 'Engine recommended Strategy 1 (Salem Cold-Storage Transfer & Dry Ice) yielding ₹ 67,000 net benefit.', 'OPTIMAL', 'Simulator Engine'),

-- INC-1001 (Mechanical breakdown)
('INC-1001', '2026-11-25 08:15:00', 'Event Ingestion', 'Mechanical Caliper Lock Warning', 'Brake thermal sensor on axle 2 triggered high resistance alert on NH-48 KM 142.4.', 'CRITICAL P1', 'Vehicle Telematics'),
('INC-1001', '2026-11-25 08:18:30', 'Exception Detection', 'Immobilization Confirmed', 'Vehicle speed dropped to 0 km/h for >10 mins on active expressway corridor.', 'CRITICAL P1', 'Anomaly Detector'),
('INC-1001', '2026-11-25 08:22:00', 'Impact Propagation', 'Blast Radius 14 Entities Affected', 'Identified 6 high-priority enterprise consignees and 4 linked purchase orders.', 'CRITICAL P1', 'Impact Engine'),
('INC-1001', '2026-11-25 08:25:00', 'SLA Risk Assessment', 'SLA Buffer 38 Mins Remaining', 'Dynamic breach probability calculated at 92.4%. Exposure estimated at ₹ 1,45,000.', 'CRITICAL P1', 'SLA Risk Engine'),

-- INC-1002 (Landslide / Route blockage)
('INC-1002', '2026-11-25 07:45:00', 'Event Ingestion', 'Expressway Boulder Fall Alert', 'Highway Authority API broadcast landslide warning at Khandala Ghat KM 42.', 'CRITICAL P1', 'Govt Transit Feed'),
('INC-1002', '2026-11-25 07:50:00', 'Exception Detection', 'Corridor Blockage Flagged', 'All westbound freight movements halted; estimated clearance delay 180+ minutes.', 'CRITICAL P1', 'Event Correlator'),
('INC-1002', '2026-11-25 07:55:00', 'Impact Propagation', 'Cross-Dock Shipment Congestion', '3 consignees and 2 shipments rerouted towards Talegaon bypass hub.', 'HIGH P2', 'Impact Engine'),

-- INC-1003 (Transmission fault)
('INC-1003', '2026-11-25 06:10:00', 'Event Ingestion', 'Transmission Overheating Code P0700', 'Engine ECU sent continuous transmission slip fault code on NH-48 Plaza 03.', 'HIGH P2', 'OBD-II Stream'),
('INC-1003', '2026-11-25 06:15:00', 'Exception Detection', 'Reduced Speed Derating Mode', 'Vehicle limited to 20 km/h crawl mode; ETA delayed by 75 minutes.', 'HIGH P2', 'Anomaly Detector'),

-- INC-1005 (Customs Bottleneck)
('INC-1005', '2026-11-25 05:30:00', 'Event Ingestion', 'Customs Manifest Discrepancy', 'Inbound air-freight airway bill weight variance exceeded 2.5% threshold.', 'HIGH P2', 'EDI Handoff'),
('INC-1005', '2026-11-25 05:40:00', 'Exception Detection', 'Hold Status Placed on Cargo', 'Port authority initiated manual inspection hold on Bay C-12.', 'HIGH P2', 'Port Authority Feed');

-- =========================================================================
-- 2. RECOVERY EXECUTIONS (Module 6: Governed Execution)
-- =========================================================================
INSERT INTO `recovery_executions`
(`execution_id`, `incident_id`, `strategy_id`, `strategy_name`, `dispatched_at`, `estimated_rendezvous_at`, `status`, `assigned_partner`, `vehicle_id`, `current_location`, `action_notes`)
VALUES
('EXEC-901', 'INC-1004', 'STRAT-01', 'Salem Cold-Storage Transfer & Dry Ice Intervention', '2026-11-25 09:42:00', '2026-11-25 10:18:00', 'Dispatched', 'TransExpress Logistics (Ravi Kumar)', 'TRK-882 (Reefer Unit)', 'Salem Highway Toll Plaza KM 18', 'Emergency cold-chain transfer dispatched with certified dry-ice recharge unit.'),
('EXEC-902', 'INC-1001', 'STRAT-02', 'Heavy-Duty Prime Mover Tow & Swap', '2026-11-25 08:35:00', '2026-11-25 09:15:00', 'Rendezvous', 'Apex Recovery Fleet', 'TOW-401', 'NH-48 KM 141.8', 'Tow truck on site; hitching trailer for swap to secondary depot.');

-- =========================================================================
-- 3. MANAGER APPROVAL LOGS (Audit Log)
-- =========================================================================
INSERT INTO `manager_approval_logs`
(`incident_id`, `strategy_id`, `strategy_name`, `approved_by`, `approved_at`, `approval_tier`, `notes`)
VALUES
('INC-1004', 'STRAT-01', 'Salem Cold-Storage Transfer & Dry Ice Intervention', 'Sethu Admin', '2026-11-25 09:40:12', 'Tier 1 Auto-Approved', 'Pre-authorized for pharma reefer excursion under ₹ 1,00,000 threshold.'),
('INC-1001', 'STRAT-02', 'Heavy-Duty Prime Mover Tow & Swap', 'Sethu Admin', '2026-11-25 08:32:45', 'Tier 2 Manager Approved', 'Approved secondary prime mover allocation to prevent factory line stoppage.');

-- =========================================================================
-- 4. ACCOUNTABILITY & TRUST SCORE (Module 9 - 42m predicted vs 38m actual, 96.4%)
-- =========================================================================
INSERT INTO `accountability_trust_scores`
(`incident_id`, `recovery_strategy_id`, `predicted_recovery_time_mins`, `actual_recovery_time_mins`, `trust_score_pct`, `prediction_accuracy_grade`, `variance_reason`, `evaluated_at`)
VALUES
('INC-1004', 'STRAT-01', 42, 38, 96.40, 'High Confidence (Grade A)', 'Intervention completed 4 minutes faster than predicted due to rapid relief unit dispatch.', '2026-11-25 10:45:00'),
('INC-1001', 'STRAT-02', 55, 59, 93.20, 'High Confidence (Grade A)', 'Slight 4-minute delay due to congestion at NH-48 toll plaza, SLA preserved with 12m margin.', '2026-11-25 09:40:00');
