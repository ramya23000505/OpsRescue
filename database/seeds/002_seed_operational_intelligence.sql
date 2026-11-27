-- database/seeds/002_seed_operational_intelligence.sql
-- Description: Comprehensive, idempotent seed data for INC-1001 through INC-1005 across Impact, Simulation, and SLA Risk
USE authpractice;

-- Disable foreign key checks for clean re-seeding
SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE TABLE `sla_shipment_contexts`;
TRUNCATE TABLE `sla_contributing_factors`;
TRUNCATE TABLE `sla_trend_points`;
TRUNCATE TABLE `sla_risk_assessments`;
TRUNCATE TABLE `simulation_strategies`;
TRUNCATE TABLE `incident_simulations`;
TRUNCATE TABLE `impact_graph_edges`;
TRUNCATE TABLE `impact_graph_nodes`;
TRUNCATE TABLE `impact_exposure_dossiers`;
TRUNCATE TABLE `impact_blast_radiuses`;
TRUNCATE TABLE `incidents`;
SET FOREIGN_KEY_CHECKS = 1;

-- =========================================================================
-- 1. INCIDENTS BASE TABLE
-- =========================================================================
INSERT INTO `incidents` 
(`incident_id`, `incident_type`, `root_cause`, `severity`, `propagation_model`, `status`, `cargo_value`, `liquidated_penalty`, `remaining_sla_buffer_minutes`)
VALUES
('INC-1001', 'Cold-Chain Delay / Mechanical Immobilization', 'Mechanical brake caliper lock on NH-48 KM 142.4 (EVT-9401 Sensor Triggered)', 'CRITICAL P1', 'Active Causal Graph', 'Active Triage', 4850000.00, 13550000.00, 38),
('INC-1002', 'Route Blockage / Landslide', 'Major boulder fall and expressway blockage on Mumbai-Pune Expressway KM 42 Ghat section', 'CRITICAL P1', 'Active Causal Graph', 'Action Required', 3200000.00, 3050000.00, 50),
('INC-1003', 'Stalled Vehicle / Transmission Fault', 'Transmission overheating and sensor failure on Delhi-Jaipur highway NH-48 Plaza 03', 'HIGH P2', 'Active Causal Graph', 'Investigating', 2000000.00, 1400000.00, 100),
('INC-1004', 'Temperature Excursion / Reefer Sensor Alert', 'Reefer compressor cycling failure resulting in temperature excursion towards +4.5°C threshold', 'CRITICAL P1', 'Active Causal Graph', 'Mitigating', 5500000.00, 6000000.00, 22),
('INC-1005', 'Supplier Handoff Delay', 'Documentation manifest discrepancy and inbound dock customs clearance bottleneck', 'HIGH P2', 'Active Causal Graph', 'Pending Approval', 2500000.00, 1620000.00, 135);

-- =========================================================================
-- 2. IMPACT BLAST RADIUS BREAKDOWN (Total = root + shipments + POs + consignees)
-- =========================================================================
INSERT INTO `impact_blast_radiuses` (`incident_id`, `root_incidents`, `shipments`, `purchase_orders`, `consignees`)
VALUES
('INC-1001', 1, 3, 4, 6),  -- 1 + 3 + 4 + 6 = 14
('INC-1002', 1, 2, 2, 3),  -- 1 + 2 + 2 + 3 = 8
('INC-1003', 1, 1, 1, 2),  -- 1 + 1 + 1 + 2 = 5
('INC-1004', 1, 2, 4, 4),  -- 1 + 2 + 4 + 4 = 11 (Matches summary!)
('INC-1005', 1, 1, 2, 2);  -- 1 + 1 + 2 + 2 = 6

-- =========================================================================
-- 3. IMPACT EXPOSURE DOSSIER (Consignees at Risk)
-- =========================================================================
INSERT INTO `impact_exposure_dossiers` 
(`incident_id`, `entity_id`, `entity_name`, `tier`, `risk_level`, `location`, `po_reference`, `value_at_risk`, `buffer_remaining_minutes`, `sla_penalty_rate`, `clinical_or_business_impact`, `assigned_carrier`, `cargo_details`)
VALUES
-- INC-1001 (6 accounts matching blast radius)
('INC-1001', 'CONS-APOLLO', 'Apollo Hospitals & Research Centre', 'Tier-1 Strategic Consignee', 'CRITICAL', 'BLR North Oncology Campus, Bangalore', 'PO-99104', 7200000.00, 38, '₹ 1.2 Lakh / hour', 'Outpatient surgery oncology ward stockout projected at 11:30 AM IST if cold-chain delivery is not intercepted.', 'TransExpress (TRK-882)', '2,400 Vaccine Vials (2°C - 8°C cold-chain verified)'),
('INC-1001', 'CONS-FORTIS', 'Fortis Health Network Hub', 'Tier-3 Consignee', 'HIGH RISK', 'Bannerghatta Center, Bangalore', 'PO-99112', 4650000.00, 75, '₹ 45,000 / hour', 'Surgical consumables delivery delay; cross-dock inbound synchronization risk at Bangalore South Hub.', 'TransExpress (TRK-882)', 'Specialty Reagents & Surgical Prep Lot'),
('INC-1001', 'CONS-STATE-DEP', 'State Medical Depository', 'Public Health Consignee', 'MEDIUM', 'Central Health Logistics Depot, Bangalore', 'PO-99120', 2200000.00, 270, '₹ 15,000 / hour', 'Regulatory reserve buffer healthy (4h 30m); low immediate stockout hazard.', 'TransExpress (TRK-882)', 'Sterile Buffer Stock'),
('INC-1001', 'CONS-MANIPAL', 'Manipal Hospital Life Science Unit', 'Tier-2 Consignee', 'HIGH RISK', 'Old Airport Road Campus, Bangalore', 'PO-99128', 1850000.00, 52, '₹ 30,000 / hour', 'ICU critical maintenance buffer low; transshipment priority requested.', 'TransExpress (TRK-882)', 'Clinical Trial Bio-samples'),
('INC-1001', 'CONS-COLUMBIA', 'Columbia Asia Hospital Hebbal', 'Tier-2 Consignee', 'MEDIUM', 'Hebbal Medical Corridor, Bangalore', 'PO-99134', 1500000.00, 90, '₹ 20,000 / hour', 'Pediatric vaccination drive scheduled for 01:00 PM IST.', 'TransExpress (TRK-882)', 'Pediatric Vaccines Lot A'),
('INC-1001', 'CONS-NARAYANA', 'Narayana Health City', 'Tier-2 Consignee', 'MEDIUM', 'Bommasandra Industrial Area, Bangalore', 'PO-99140', 1000000.00, 110, '₹ 15,000 / hour', 'Cardiac care post-op sterile kits; cross-dock connection needed.', 'TransExpress (TRK-882)', 'Surgical Sterile Supplies'),

-- INC-1002 (3 accounts)
('INC-1002', 'CONS-TATA-PIMPRI', 'Tata Motors Commercial Vehicles', 'Tier-1 Strategic Plant', 'CRITICAL', 'Pimpri Manufacturing Plant, Pune', 'PO-88101', 3500000.00, 50, '₹ 50,000 / hour', 'Final assembly line stop hazard projected in 50 minutes if transmission parts not delivered.', 'FreightLink (MH-12-TR-402)', 'Heavy Commercial Transmission Sub-assemblies'),
('INC-1002', 'CONS-BAJAJ-CHAKAN', 'Bajaj Auto Plant 2', 'Tier-2 Strategic Consignee', 'HIGH RISK', 'Chakan MIDC Industrial Area, Pune', 'PO-88109', 1750000.00, 70, '₹ 25,000 / hour', 'Engine casing casting delay impacting morning production shift.', 'FreightLink (MH-12-TR-402)', 'Alloy Cast Engine Brackets'),
('INC-1002', 'CONS-BHARAT-FORGE', 'Bharat Forge Mundhwa Works', 'Tier-2 Consignee', 'MEDIUM', 'Mundhwa Industrial Corridor, Pune', 'PO-88118', 1000000.00, 120, '₹ 10,000 / hour', 'Forging billets handoff delay; warehouse buffer sufficient for 2 hours.', 'FreightLink (MH-12-TR-402)', 'Machined Forged Components'),

-- INC-1003 (2 accounts)
('INC-1003', 'CONS-CROMA-JAIPUR', 'Croma Regional Distribution Hub', 'Tier-2 Commercial Account', 'HIGH RISK', 'Mansarovar Logistics Park, Jaipur', 'PO-77401', 2000000.00, 100, '₹ 20,000 / hour', 'Weekend retail promotions stock delivery delay across 6 flagship stores.', 'NorthStar Express (HR-55-E-890)', 'High-End Smart Displays & Audio Gear'),
('INC-1003', 'CONS-RELIANCE-DIG', 'Reliance Digital Central Depot', 'Tier-2 Commercial Account', 'MEDIUM', 'Sitapura Industrial Area, Jaipur', 'PO-77412', 1400000.00, 140, '₹ 14,000 / hour', 'E-commerce fulfillment same-day dispatch window at risk.', 'NorthStar Express (HR-55-E-890)', 'Mobile Handsets & Accessories'),

-- INC-1004 (4 accounts matching blast radius 11 = 1+2+4+4!)
('INC-1004', 'CONS-KMCH-CBE', 'KMCH Hospital & Research Center', 'Tier-1 Strategic Consignee', 'CRITICAL', 'Avinashi Road Medical Hub, Coimbatore', 'PO-66201', 4500000.00, 22, '₹ 45,000 / hour', 'Critical oncology chemotherapy infusion biologics at risk of irreversible thermal degradation if temperature exceeds +5°C.', 'ColdTrans Fleet (KA-01-MJ-1103)', 'Temperature-Sensitive Biologics (2°C-8°C)'),
('INC-1004', 'CONS-PSG-HOSP', 'PSG Hospitals Specialty Center', 'Tier-1 Strategic Consignee', 'CRITICAL', 'Peelamedu Healthcare Complex, Coimbatore', 'PO-66208', 3200000.00, 25, '₹ 30,000 / hour', 'Organ transplant immunosuppressant stockout hazard in transplant recovery ICU.', 'ColdTrans Fleet (KA-01-MJ-1103)', 'Immunosuppressant Solutions'),
('INC-1004', 'CONS-GANGA-MED', 'Ganga Medical Center & Hospital', 'Tier-2 Consignee', 'HIGH RISK', 'Ramnagar Medical Zone, Coimbatore', 'PO-66215', 2300000.00, 35, '₹ 12,000 / hour', 'Orthopedic biological bone graft matrix materials awaiting temperature confirmation.', 'ColdTrans Fleet (KA-01-MJ-1103)', 'Biological Graft Matrix Kits'),
('INC-1004', 'CONS-ROYAL-CARE', 'Royal Care Super Speciality Hospital', 'Tier-2 Consignee', 'HIGH RISK', 'Neelambur Bypass Campus, Coimbatore', 'PO-66224', 1500000.00, 45, '₹ 8,000 / hour', 'Specialty diagnostic reagents buffer low; auxiliary cooling required.', 'ColdTrans Fleet (KA-01-MJ-1103)', 'In-Vitro Clinical Diagnostic Reagents'),

-- INC-1005 (2 accounts)
('INC-1005', 'CONS-DR-REDDYS', 'Dr. Reddy’s Laboratories FTO Unit', 'Tier-1 Strategic Consignee', 'HIGH RISK', 'Bachupally Pharmaceutical Zone, Hyderabad', 'PO-55102', 2600000.00, 135, '₹ 25,000 / hour', 'Formulation processing batch hold pending delivery of active pharmaceutical reagents.', 'Deccan Logistics (AP-31-TX-901)', 'Active Pharmaceutical Reagents (Grade A)'),
('INC-1005', 'CONS-AUROBINDO', 'Aurobindo Pharma Research Centre', 'Tier-2 Consignee', 'MEDIUM', 'Pashamylaram Industrial Area, Hyderabad', 'PO-55115', 1520000.00, 160, '₹ 16,200 / hour', 'Chemical synthesis catalyst lot; process scheduling buffer intact for 2.5 hours.', 'Deccan Logistics (AP-31-TX-901)', 'High-Purity Catalyst Compounds');

-- =========================================================================
-- 4. IMPACT GRAPH NODES (INC-1001 & INC-1004 sample nodes)
-- =========================================================================
INSERT INTO `impact_graph_nodes` (`incident_id`, `node_id`, `label`, `tier_level`, `node_type`, `status`, `subtitle`, `attributes_json`)
VALUES
-- INC-1001
('INC-1001', 'INC-1001', 'INC-1001', 0, 'IncidentOrigin', 'CRITICAL DELAY', 'NH-48 Caliper Lock (EVT-9401)', '{"Location":"NH-48 KM 142.4","Severity":"Critical P1"}'),
('INC-1001', 'SHP-1042', 'SHP-1042 (Vaccines)', 1, 'DirectDisruption', 'Critical Delay (+1h 48m)', 'Temp: +3.8°C (Cold-Chain OK)', '{"Batch":"#V-902","Quantity":"2,400 vials","Carrier":"TRK-882"}'),
('INC-1001', 'SHP-1043', 'SHP-1043 (Syringe Kits)', 1, 'DirectDisruption', 'Interdependent Lot B', 'Cross-dock Bay At Risk', '{"Coupling":"Coupled Transit with SHP-1042"}'),
('INC-1001', 'FAC-BLR-SOUTH', 'Cross-dock BLR-South', 1, 'Facility', 'Bay Inbound Buffer 55m', '3 Outbound Couplings Affected', '{"FacilityType":"Regional Transshipment Hub"}'),
('INC-1001', 'PO-99104', 'PO-99104 (Apollo)', 2, 'DownstreamPO', 'Breach at 11:30 AM IST', 'Value: ₹ 72.0 Lakhs', '{"RemainingBuffer":"38 mins"}'),
('INC-1001', 'PO-99112', 'PO-99112 (Fortis)', 2, 'DownstreamPO', 'Buffer: 1h 15m left', 'Value: ₹ 46.5 Lakhs', '{"Penalty":"₹ 45,000 / hr"}'),
('INC-1001', 'INT-PKG-LINE4', 'Packaging Line 4', 2, 'Facility', 'Idle Risk: 2h 10m', 'Sterile Assembly BLR', '{"Department":"Internal Ops Assembly"}'),
('INC-1001', 'CONS-APOLLO', 'Apollo Hospitals Hub', 3, 'EndConsignee', 'Immediate Stockout Risk', 'Tier 1 · VIP Consignee', '{"Campus":"BLR North Oncology Campus","PenaltyRate":"₹ 1.2L / hr"}'),
('INC-1001', 'CONS-FORTIS', 'Fortis Health Network', 3, 'EndConsignee', 'Tier 3 · Strategic Consignee', 'Penalty: ₹ 45,000 / hr', '{"Campus":"Bannerghatta Center"}'),
('INC-1001', 'CONS-STATE-DEP', 'State Medical Depository', 3, 'EndConsignee', 'Low Stockout Hazard', 'Regulatory Buffer: 4h 30m', '{"Depot":"Bangalore Central Depot"}'),

-- INC-1004
('INC-1004', 'INC-1004', 'INC-1004', 0, 'IncidentOrigin', 'CRITICAL EXCURSION', 'NH-544 Reefer Alarm (+4.5°C)', '{"Location":"NH-544 Bypass","Severity":"Critical P1"}'),
('INC-1004', 'SHP-1103', 'SHP-1103 (Biologics)', 1, 'DirectDisruption', 'Critical Excursion (+1h 35m)', 'Current Temp: +4.5°C', '{"Batch":"#BIO-441","Carrier":"KA-01-MJ-1103"}'),
('INC-1004', 'PO-66201', 'PO-66201 (KMCH)', 2, 'DownstreamPO', 'Breach in 22 mins', 'Value: ₹ 45.0 Lakhs', '{"RemainingBuffer":"22 mins"}'),
('INC-1004', 'PO-66208', 'PO-66208 (PSG)', 2, 'DownstreamPO', 'Breach in 25 mins', 'Value: ₹ 32.0 Lakhs', '{"Penalty":"₹ 30,000 / hr"}'),
('INC-1004', 'CONS-KMCH-CBE', 'KMCH Hospital Coimbatore', 3, 'EndConsignee', 'Emergency Biologics Risk', 'Tier 1 · VIP Hospital', '{"Campus":"Avinashi Road","PenaltyRate":"₹ 45,000 / hr"}'),
('INC-1004', 'CONS-PSG-HOSP', 'PSG Hospitals Center', 3, 'EndConsignee', 'Transplant ICU Stockout Hazard', 'Tier 1 · Strategic Consignee', '{"Campus":"Peelamedu","PenaltyRate":"₹ 30,000 / hr"}');

-- =========================================================================
-- 5. IMPACT GRAPH EDGES
-- =========================================================================
INSERT INTO `impact_graph_edges` (`incident_id`, `from_node`, `to_node`, `relationship`, `label`, `severity`)
VALUES
-- INC-1001
('INC-1001', 'INC-1001', 'SHP-1042', 'Triggers', 'STALL: NH-48', 'Critical'),
('INC-1001', 'INC-1001', 'SHP-1043', 'CoupledWith', 'COUPLED TRANSIT', 'High'),
('INC-1001', 'INC-1001', 'FAC-BLR-SOUTH', 'Feeds', 'BAY IMPACT', 'Warning'),
('INC-1001', 'SHP-1042', 'PO-99104', 'Carries', 'BUFFER 38m', 'Critical'),
('INC-1001', 'SHP-1042', 'PO-99112', 'Carries', 'CROSS-DOCK', 'High'),
('INC-1001', 'SHP-1043', 'INT-PKG-LINE4', 'Feeds', 'ASSEMBLY FEED', 'Warning'),
('INC-1001', 'PO-99104', 'CONS-APOLLO', 'DeliversTo', 'VIP TIER 1', 'Critical'),
('INC-1001', 'PO-99112', 'CONS-FORTIS', 'DeliversTo', 'TIER 3', 'High'),
('INC-1001', 'FAC-BLR-SOUTH', 'CONS-STATE-DEP', 'DeliversTo', 'REGULATORY BUFFER', 'Warning'),

-- INC-1004
('INC-1004', 'INC-1004', 'SHP-1103', 'Triggers', 'TEMP EXCURSION', 'Critical'),
('INC-1004', 'SHP-1103', 'PO-66201', 'Carries', 'BUFFER 22m', 'Critical'),
('INC-1004', 'SHP-1103', 'PO-66208', 'Carries', 'BUFFER 25m', 'Critical'),
('INC-1004', 'PO-66201', 'CONS-KMCH-CBE', 'DeliversTo', 'URGENT ONCOLOGY', 'Critical'),
('INC-1004', 'PO-66208', 'CONS-PSG-HOSP', 'DeliversTo', 'TRANSPLANT ICU', 'Critical');

-- =========================================================================
-- 6. INCIDENT SIMULATIONS METADATA
-- =========================================================================
INSERT INTO `incident_simulations` 
(`incident_id`, `shipment_id`, `recommended_strategy_id`, `recommendation_summary`, `key_tradeoff_finding`, `why_recommended`, `why_rejected_alternatives`)
VALUES
('INC-1001', 'SHP-1042', 'STRAT-02', 
 'Highest SLA recovery certainty with manageable operational outlay. Preserves 100% cold-chain compliance window (2°C-8°C) and completes hospital dock handover 6 minutes prior to contractual SLA breach limit.',
 'Strategy 2 preserves hospital dock delivery window while saving ₹ 1,00,500 in net penalties versus Strategy 1, and avoids the ₹ 58,000 cost + regional stockout risk of Strategy 3.',
 'High SLA certainty (97.8%), fits within Tier 2 budget ceiling (< ₹ 50k), and preserves cold-chain buffer without depleting central warehouse reserves.',
 'Strategy 1 breaches delivery cut-off (+12m) triggering liquidated damages; Strategy 3 exceeds Tier 2 spend threshold and risks buffer depletion at Nelamangala central depot.'),

('INC-1002', 'SHP-1058', 'STRAT-01',
 'Expedited bypass via Old Pune-Mumbai Highway avoids 2-hour landslide stoppage, preserving automotive assembly shift timelines.',
 'Strategy 1 costs ₹ 14,000 and recovers on-time SLA at 95.5%, avoiding ₹ 85,000 in contractual line stoppage penalties.',
 'Ensures on-time dock delivery with minimal outlay and zero component handling damage risk.',
 'Strategy 2 (waiting) triggers severe factory line shut penalties; Strategy 3 (air-cargo/drone) is economically non-viable for heavy commercial components.'),

('INC-1003', 'SHP-1081', 'STRAT-01',
 'Mobile transmission repair unit dispatch provides fastest on-site turnaround without freight offloading.',
 'Strategy 1 saves ₹ 25,500 net compared to towing delays and guarantees afternoon retail delivery window.',
 'Maintains driver custody, fast mechanical resolution, and preserves full retail promotional commitments.',
 'Strategy 2 causes high towing outlay and cross-dock delays; Strategy 3 waiting causes guaranteed retail SLA breach.'),

('INC-1004', 'SHP-1103', 'STRAT-01',
 'Salem Cold-Storage Transfer & Auxiliary Dry Ice Top-up prevents thermal runaway and preserves hospital biologics delivery cut-off.',
 'Strategy 1 costs ₹ 28,000 while preventing ₹ 95,000 in liquidated damages and total loss of ₹ 55 Lakhs biological cargo.',
 'Stops temperature climb at +4.5°C before critical +5.0°C excursion point, preserving 100% medication efficacy for transplant surgeries.',
 'Strategy 2 (speeding without cooling) risks total cargo spoilage; Strategy 3 (charter courier) exceeds emergency operational expenditure authorization.'),

('INC-1005', 'SHP-1092', 'STRAT-01',
 'Direct Dock Inbound Priority Slot at Hyderabad Hub clears customs expedite window and eliminates batch idle time.',
 'Strategy 1 investment of ₹ 7,500 prevents ₹ 41,200 in delayed formulation penalties.',
 'Immediate customs slot allocation with zero cargo transfer handling.',
 'Strategy 2 (rerouting via Vijayawada) introduces unnecessary miles; Strategy 3 regular processing breaches guaranteed batch formulation SLA.');

-- =========================================================================
-- 7. SIMULATION STRATEGIES (3 distinct strategies per incident)
-- =========================================================================
INSERT INTO `simulation_strategies`
(`incident_id`, `strategy_id`, `strategy_number`, `title`, `category_tag`, `description`, `is_recommended`, `recommendation_badge`, `predicted_delivery_time`, `delivery_margin`, `estimated_outlay`, `outlay_breakdown`, `expected_sla_outcome`, `sla_success_probability`, `liquidated_damages_impact`, `is_damages_avoided`, `operational_protocol`, `identified_constraints`, `operational_governance`, `required_approval_tier`, `governance_alert`)
VALUES
-- INC-1001 Strategies
('INC-1001', 'STRAT-01', 1, 'Alternate Route Bypass', 'Continuation', 'Expedited State Highway corridor bypassing KM 142 congestion node.', FALSE, NULL, '11:42 AM IST', '+12m Breach', 18500.00, 'Direct Fuel / Toll', 'Breach / Partial Penalty', 42.00, 45000.00, FALSE, 'Driver Murugan remains in vehicle TRK-882. Reroutes through SH-116. Zero cross-loading required.', 'Recurrence risk on alternator belt high. Exceeds permissible continuous driver duty limit by 38 mins.', NULL, 'Tier 1', NULL),
('INC-1001', 'STRAT-02', 2, 'Relief Cross-Dock Transfer', 'Optimized', 'Direct deployment of secondary pre-cooled reefer with highway mobile transfer.', TRUE, 'RECOMMENDED STRATEGY · Zero SLA Breach', '11:24 AM IST', '-6m Prior to Cut-off', 34500.00, 'Reefer + Transshipment', '100% On-Time Preserved', 97.80, 135000.00, TRUE, 'Dispatch dedicated reefer TRK-914 from Hosur Logistics Hub. Rendezvous at KM 145 Rest Area for 14-min mobile cryogenic cross-dock; bypass via NICE expressway.', NULL, 'Requires dual-driver Murugan/Kiran pallet sign-off. Fits inside Tier 2 authorization threshold (Alex Mercer).', 'Tier 2', NULL),
('INC-1001', 'STRAT-03', 3, 'Secondary Depot Swap', 'Stock Swap', 'Release identical batch from Nelamangala Regional Central Depot via priority 3PL courier.', FALSE, NULL, '11:28 AM IST', '-2m Margin', 58000.00, 'Expedited 3PL + Swap', 'SLA Preserved', 98.20, 135000.00, TRUE, 'Immobilize stranded truck TRK-882. Release secondary reserve batch #VAC-992 from depot. Deliver via partner BlueDart reefer van.', NULL, NULL, 'Tier 4', 'Depletes regional hospital buffer. Mandates Tier 4 VP Supply Chain authorization (> ₹ 50,000 threshold).'),

-- INC-1002 Strategies
('INC-1002', 'STRAT-01', 1, 'Old Highway Ghat Bypass', 'Optimized', 'Divert commercial transport through Khopoli Old Highway connector to bypass landslide blockage.', TRUE, 'RECOMMENDED STRATEGY', '12:55 PM IST', '-20m Margin', 14000.00, 'Toll + Escort Pilot', '100% On-Time Preserved', 95.50, 85000.00, TRUE, 'Divert vehicle at Khalapur toll onto Old Highway with local police pilot escort.', NULL, 'Authorized under Tier 1 Dispatch Controller authority.', 'Tier 1', NULL),
('INC-1002', 'STRAT-02', 2, 'Wait for Expressway Clearance', 'Continuation', 'Maintain position on expressway until NHAI heavy equipment clears debris.', FALSE, NULL, '02:30 PM IST', '+1h 15m Breach', 0.00, 'Zero Outlay', 'Factory Assembly Line Stoppage', 35.00, 85000.00, FALSE, 'Vehicle idles in queue. Driver rests in cabin.', 'High liquidated penalty risk of ₹ 85,000 per hour of assembly stoppage.', NULL, 'Tier 1', NULL),
('INC-1002', 'STRAT-03', 3, 'Emergency Courier Split Relay', 'Expedited', 'Cross-load critical tier-1 sub-assemblies into two light rapid commercial vehicles.', FALSE, NULL, '12:40 PM IST', '-35m Margin', 42000.00, 'Dual Courier Relay', 'SLA Preserved', 96.00, 85000.00, TRUE, 'Split pallets across two Tata Ace vans at toll plaza. Proceed via single-lane bypass.', 'Double freight handling risk.', 'Requires Operations Manager sign-off.', 'Tier 2', NULL),

-- INC-1003 Strategies
('INC-1003', 'STRAT-01', 1, 'Mobile Transmission Van Repair', 'Optimized', 'Dispatch authorized mobile service unit from Neemrana service cluster.', TRUE, 'RECOMMENDED STRATEGY', '03:55 PM IST', '-35m Margin', 8500.00, 'Mobile Service Charge', 'SLA Preserved', 92.00, 34000.00, TRUE, 'On-site transmission sensor replacement by certified technician at Plaza 03.', NULL, 'Fits within local fleet coordinator petty maintenance ceiling.', 'Tier 1', NULL),
('INC-1003', 'STRAT-02', 2, 'Tow to Jaipur Terminal Hub', 'Recovery', 'Engage commercial heavy flatbed tow truck to haul vehicle directly to terminal.', FALSE, NULL, '04:25 PM IST', '-5m Margin', 16000.00, 'Commercial Towing Outlay', 'Marginal SLA Compliance', 78.00, 34000.00, TRUE, 'Tow tractor TRK-881 to Jaipur Transport Nagar for joint delivery.', 'High risk of minor transit cosmetic damage during haulage.', NULL, 'Tier 1', NULL),
('INC-1003', 'STRAT-03', 3, 'Wait for Fleet Breakdown Assistance', 'Continuation', 'Wait for standard non-priority roadside fleet assistance.', FALSE, NULL, '05:10 PM IST', '+40m Breach', 0.00, 'Standard Maintenance', 'Late Delivery Breach', 48.00, 34000.00, FALSE, 'Truck awaits scheduled fleet assistance.', 'Breaches retail mall dock receiving hours (05:00 PM cutoff).', NULL, 'Tier 1', NULL),

-- INC-1004 Strategies (Matching INC-1004 Biologics context!)
('INC-1004', 'STRAT-01', 1, 'Salem Cold-Storage Transfer & Dry Ice Intervention', 'Optimized', 'Rendezvous at Salem Integrated Cold-Chain Facility for auxiliary dry-ice thermal recharge.', TRUE, 'RECOMMENDED STRATEGY · Zero Spoilage', '11:55 AM IST', '-15m Prior to Cut-off', 28000.00, 'Cryo-Recharge + Handling', '100% On-Time Preserved', 96.50, 95000.00, TRUE, 'Direct vehicle into Salem Cold Hub Bay 3. Inject cryogenic dry-ice buffer to stabilize reefer at +3.2°C. Expedite transit via NH-544 bypass.', NULL, 'Fits comfortably inside Tier 2 operations manager authority ceiling (< ₹ 50,000).', 'Tier 2', NULL),
('INC-1004', 'STRAT-02', 2, 'High-Speed Non-Stop Transit', 'Continuation', 'Instruct driver to bypass planned rest stops and drive direct without thermal intervention.', FALSE, NULL, '12:25 PM IST', '+15m Breach', 12000.00, 'Excess Fuel Outlay', 'Severe Thermal Degradation / Cargo Rejection', 51.00, 95000.00, FALSE, 'Driver operates at maximum permissible speed without stopping for compressor diagnosis.', 'Extreme probability of temperature crossing +6.5°C resulting in ₹ 55 Lakhs total cargo disposal loss.', NULL, 'Tier 1', 'Severe Regulatory Compliance Risk. FDA / GMP cold-chain audit violation.'),
('INC-1004', 'STRAT-03', 3, 'Emergency Cryo-Dedicated Express Courier', 'Expedited', 'Charter dedicated medical air-conditioned vehicle from Coimbatore regional branch.', FALSE, NULL, '11:45 AM IST', '-25m Margin', 65000.00, 'Dedicated Fleet Charter', 'SLA Preserved', 98.00, 95000.00, TRUE, 'Intercept vehicle at Bhavani junction. Transfer all biologics batches into dedicated backup sprinter van.', NULL, 'Exceeds Tier 2 ₹ 50,000 authority ceiling. Requires Tier 4 VP Supply Chain approval.', 'Tier 4', 'Expenditure exceeds standard contingency threshold by ₹ 15,000.'),

-- INC-1005 Strategies
('INC-1005', 'STRAT-01', 1, 'Priority Inbound Customs Slot', 'Optimized', 'Pay express clearance facilitation tariff for immediate dock spot allocation.', TRUE, 'RECOMMENDED STRATEGY', '05:15 PM IST', '-45m Margin', 7500.00, 'Express Clearance Fee', '100% On-Time Preserved', 94.00, 41200.00, TRUE, 'Present pre-cleared electronic manifests at Hyderabad Hub Express Gate 4.', NULL, 'Pre-approved under standard expedited trade compliance protocol.', 'Tier 1', NULL),
('INC-1005', 'STRAT-02', 2, 'Cross-Dock Swap at Vijayawada Hub', 'Reroute', 'Transfer shipment to scheduled outbound shuttle departing Vijayawada.', FALSE, NULL, '05:40 PM IST', '-20m Margin', 22000.00, 'Inter-Hub Shuttling', 'SLA Preserved', 89.00, 41200.00, TRUE, 'Rendezvous with outbound shuttle VT-104 at Vijayawada junction.', 'Additional handling adds 30 min variance.', NULL, 'Tier 1', NULL),
('INC-1005', 'STRAT-03', 3, 'Standard Batch Queue Processing', 'Continuation', 'Process truck in standard first-come first-served queue.', FALSE, NULL, '06:30 PM IST', '+30m Breach', 0.00, 'Zero Outlay', 'SLA Breach Penalty', 58.00, 41200.00, FALSE, 'Driver waits in standard unverified inbound line.', 'Breaches pharmaceutical manufacturing batch start SLA at 06:00 PM.', NULL, 'Tier 1', NULL);

-- =========================================================================
-- 8. SLA RISK ASSESSMENTS (1-to-1)
-- =========================================================================
INSERT INTO `sla_risk_assessments`
(`incident_id`, `shipment_id`, `route`, `severity`, `breach_probability`, `risk_tier`, `risk_baseline_delta`, `time_to_sla_breach_minutes`, `hard_breach_threshold_time`, `projected_delay`, `committed_sla_window`, `financial_exposure`, `financial_exposure_type`, `remaining_buffer_badge`)
VALUES
('INC-1001', 'SHP-1042', 'Chennai → Bengaluru · NH-48 KM 142.4', 'CRITICAL P1', 94.20, 'Critical Risk', '+18.4% vs baseline', 38, '11:30 AM IST', '+1h 48m', '11:30 AM IST', 135000.00, 'Contractual liquidated damages', 'Remaining Buffer: 38 min'),
('INC-1002', 'SHP-1058', 'Mumbai → Pune Expwy · KM 42 Ghat', 'CRITICAL P1', 88.50, 'Critical Risk', '+22.0% vs baseline', 50, '01:15 PM IST', '+2h 10m', '01:15 PM IST', 85000.00, 'Factory assembly line stoppage penalty', 'Remaining Buffer: 50 min'),
('INC-1003', 'SHP-1081', 'Delhi → Jaipur · NH-48 Plaza 03', 'HIGH P2', 64.00, 'High Risk', '+10.5% vs baseline', 100, '04:30 PM IST', '+1h 15m', '04:30 PM IST', 34000.00, 'Commercial retail SLA liquidated damages', 'Remaining Buffer: 1h 40m'),
('INC-1004', 'SHP-1103', 'BLR → Coimbatore · NH-544 Bypass', 'CRITICAL P1', 96.50, 'Critical Risk', '+28.2% vs baseline', 22, '12:10 PM IST', '+1h 35m', '12:10 PM IST', 95000.00, 'Medical cold-chain SLA breach penalty', 'Remaining Buffer: 22 min'),
('INC-1005', 'SHP-1092', 'Vizag → Hyderabad Hub · NH-65', 'HIGH P2', 55.00, 'Moderate Risk', '+8.0% vs baseline', 135, '06:00 PM IST', '+45m', '06:00 PM IST', 41200.00, 'Pharma formulation delay liquidated penalty', 'Remaining Buffer: 2h 15m');

-- =========================================================================
-- 9. SLA TREND POINTS (Progression curves for each incident)
-- =========================================================================
INSERT INTO `sla_trend_points` (`incident_id`, `sort_order`, `time_label`, `normal_transit_probability`, `breach_curve_probability`, `is_current_point`, `is_hard_breach_threshold`)
VALUES
-- INC-1001 Trend Points (Matching Slide 6!)
('INC-1001', 1, '09:30', 5.00, 5.00, FALSE, FALSE),
('INC-1001', 2, '10:00', 6.20, 14.80, FALSE, FALSE),
('INC-1001', 3, '10:15', 7.50, 46.50, FALSE, FALSE),
('INC-1001', 4, '10:30 (NOW)', 8.80, 94.20, TRUE, FALSE),
('INC-1001', 5, '11:00', 9.50, 99.10, FALSE, FALSE),
('INC-1001', 6, '11:30', 10.00, 100.00, FALSE, TRUE),

-- INC-1004 Trend Points (22 min buffer!)
('INC-1004', 1, '10:00', 4.00, 4.00, FALSE, FALSE),
('INC-1004', 2, '10:45', 5.50, 18.00, FALSE, FALSE),
('INC-1004', 3, '11:15', 7.00, 52.00, FALSE, FALSE),
('INC-1004', 4, '11:48 (NOW)', 8.50, 96.50, TRUE, FALSE),
('INC-1004', 5, '12:10', 10.00, 100.00, FALSE, TRUE);

-- =========================================================================
-- 10. SLA CONTRIBUTING FACTORS
-- =========================================================================
INSERT INTO `sla_contributing_factors` (`incident_id`, `sort_order`, `factor_name`, `severity_badge`, `contribution_percentage`, `description`, `key_metrics_json`)
VALUES
-- INC-1001 (58%, 24%, 18%)
('INC-1001', 1, 'Vehicle Immobilization', 'Critical · 58% Contributor', 58, 'Mechanical brake caliper issue on NH-48 KM 142.4. Awaiting mobile repair unit arrival.', '{"Sub-system":"Brake Caliper","Location":"NH-48 KM 142.4","ETA Mobile Van":"14 min","Delay Impact":"+52m delay"}'),
('INC-1001', 2, 'Route Congestion', 'High · 24% Contributor', 24, 'Peak expressway backlog entering Bengaluru South arterial corridor.', '{"Corridor":"BLR South Arterial","Traffic Flow":"18 / 55 km/h","Backlog Queue":"4.2 km queue","Delay Impact":"+28m delay"}'),
('INC-1001', 3, 'Remaining Delivery Buffer', 'High · 18% Contributor', 18, 'Turnaround ETA exceeds contractual grace window by 70 minutes without intervention.', '{"Grace Window":"Expired (-10 min)","Temp Hazard":"Safe (+4.1°C)","Contract Rule":"Grace 60m","Delay Impact":"+18m deficit"}'),

-- INC-1004 (62%, 23%, 15%)
('INC-1004', 1, 'Reefer Compressor Excursion', 'Critical · 62% Contributor', 62, 'Auxiliary cooling compressor thermal overload on NH-544 bypass.', '{"Current Temp":"+4.5°C","Threshold":"+5.0°C","Time to Excursion":"22 mins","Delay Impact":"+55m delay"}'),
('INC-1004', 2, 'Regional Highway Reroute', 'High · 23% Contributor', 23, 'Single-lane road construction slowing bypass transit speed.', '{"Speed":"28 km/h","Backlog":"3.1 km","Delay Impact":"+25m delay"}'),
('INC-1004', 3, 'Zero Buffer Margin', 'High · 15% Contributor', 15, 'Strict hospital pharmacy sterile prep cutoff window with zero tolerance.', '{"Grace Period":"0 mins","Penalty Hazard":"Severe","Delay Impact":"+15m deficit"}');

-- =========================================================================
-- 11. SLA SHIPMENT CONTEXT
-- =========================================================================
INSERT INTO `sla_shipment_contexts` (`incident_id`, `consignee`, `master_sla`, `criticality`, `temperature_status`, `assigned_vehicle`)
VALUES
('INC-1001', 'Apollo Hospitals & Research Centre', 'Tier 1 Cold-Chain Compliance', 'High-priority Medical Supplies (Vaccines)', 'Safe (+4.1°C)', 'TRK-882 (Reefer Unit)'),
('INC-1002', 'Tata Motors Commercial Vehicles', 'JIT Manufacturing Line Supply SLA', 'Production Critical Assembly', 'Ambient Normal', 'MH-12-TR-402 (Heavy Hauler)'),
('INC-1003', 'Croma Regional Distribution Hub', 'Tier 2 Retail Promotion SLA', 'High Commercial Value', 'Ambient Normal', 'HR-55-E-890 (Cargo Van)'),
('INC-1004', 'KMCH Hospital & Research Center', 'Tier 1 Critical Biologics Cold-Chain', 'Emergency Surgery Biologics', 'Excursion Alert (+4.5°C)', 'KA-01-MJ-1103 (Cryo Reefer)'),
('INC-1005', 'Dr. Reddy’s Laboratories FTO Unit', 'Pharmaceutical Bulk Synthesis SLA', 'Active Chemical Reagents', 'Safe Ambient (18°C-22°C)', 'AP-31-TX-901 (Dedicated Fleet)');
