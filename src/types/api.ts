// =============================================
// API Types — OpsRescue Frontend
// =============================================

// Auth
export interface LoginRequest {
  email: string
  password: string
}

export interface AuthResponse {
  token: string
  user: {
    id: string
    name: string
    email: string
    role: string
  }
}

// Module 2 — Impact Analysis
export interface GraphNode {
  id: string
  label: string
  tierLevel: number
  nodeType: 'IncidentOrigin' | 'DirectDisruption' | 'DownstreamPO' | 'EndConsignee'
  status: string
}

export interface GraphEdge {
  from: string
  to: string
  relationship: string
  label: string
  severity: string
}

export interface ExposureDossierItem {
  entityId: string
  entityName: string
  tier: string
  riskLevel: string
  location: string
  poReference: string
  valueAtRiskFormatted: string
  bufferRemainingFormatted: string
  slaPenaltyRate: string
  clinicalOrBusinessImpact: string
  cargoDetails: string
}

export interface ImpactAnalysis {
  incidentId: string
  incidentType: string
  rootCause: string
  severity: string
  totalBlastRadius: number
  blastRadiusBreakdown: {
    rootIncidents: number
    shipments: number
    purchaseOrders: number
    consignees: number
  }
  contractualRevenueRisk: number
  revenueRiskFormatted: string
  cargoValue: number
  liquidatedPenalty: number
  affectedCustomerAccounts: number
  customerImpactSummary: string
  remainingSlaBufferMinutes: number
  remainingSlaBufferFormatted: string
  exposureDossier: ExposureDossierItem[]
  graphNodes: GraphNode[]
  graphEdges: GraphEdge[]
}

// Module 3 — SLA Risk Engine
export interface BreachTrendPoint {
  timeLabel: string
  normalTransitProbability: number
  breachCurveProbability: number
  isCurrentPoint?: boolean
  isHardBreachThreshold?: boolean
}

export interface ContributingFactor {
  factorName: string
  severityBadge: string
  contributionPercentage: number
  description: string
}

export interface SlaRisk {
  incidentId: string
  shipmentId: string
  route: string
  severity: string
  breachProbability: number
  breachProbabilityFormatted: string
  riskTier: string
  riskBaselineDelta: string
  timeToSlaBreachMinutes: number
  timeToSlaBreachFormatted: string
  hardBreachThresholdTime: string
  projectedDelay: string
  committedSlaWindow: string
  financialExposure: number
  financialExposureFormatted: string
  remainingBufferBadge: string
  breachProbabilityTrend: BreachTrendPoint[]
  contributingFactors: ContributingFactor[]
  shipmentContext: {
    consignee: string
    masterSla: string
    criticality: string
    temperatureStatus: string
    assignedVehicle: string
  }
}

// Module 4 — Recovery Simulator
export interface Strategy {
  strategyId: string
  strategyNumber: number
  title: string
  categoryTag: string
  description: string
  isRecommended: boolean
  recommendationBadge?: string
  predictedDeliveryTime: string
  deliveryMargin: string
  estimatedOutlayFormatted: string
  outlayBreakdown: string
  expectedSlaOutcome: string
  slaSuccessProbabilityFormatted: string
  liquidatedDamagesImpactFormatted: string
  isDamagesAvoided: boolean
  operationalProtocol: string
  operationalGovernance?: string
  identifiedConstraints?: string
  governanceAlert?: string
  requiredApprovalTier: string
}

export interface RecoverySimulation {
  incidentId: string
  shipmentId: string
  optimizationEngineHeadline: string
  recommendedStrategyId: string
  recommendationSummary: string
  strategies: Strategy[]
  tradeoffSummary: {
    keyTradeoffFinding: string
    netBenefitFormatted: string
    whyRecommended: string
    whyRejectedAlternatives: string
  }
}

// Module 8 — Control Tower
export interface ControlTowerMetrics {
  activeIncidents: number
  criticalIncidents: number
  revenueAtRisk: string
  avgSlaBufferMinutes: number
  shipmentsMonitored: number
}

export interface IncidentRow {
  id: string
  type: string
  severity: 'P1' | 'P2' | 'P3'
  route: string
  blastRadius: number
  revenueRisk: string
  slaBuffer: string
  status: 'ACTIVE' | 'ESCALATED' | 'RESOLVED'
  timestamp: string
}
