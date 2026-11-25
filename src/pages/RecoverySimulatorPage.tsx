// =============================================
// Module 4 — Recovery Simulator
// IBM Carbon Design System — 3-Column Strategy Grid
// =============================================
import React, { useEffect, useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { getRecoverySimulation } from '../services/api'
import type { RecoverySimulation, Strategy } from '../types/api'

// ── Mock (Ramya's exact contract)
const MOCK: RecoverySimulation = {
  incidentId: 'INC-1001',
  shipmentId: 'SHP-1042',
  optimizationEngineHeadline: 'System Optimization Engine · Recommended Strategy: Strategy 2 - Relief Vehicle / Cross-Dock Transfer',
  recommendedStrategyId: 'STRAT-02',
  recommendationSummary: 'Highest SLA recovery certainty with manageable operational outlay. Preserves 100% cold-chain compliance window (2°C-8°C) and completes hospital dock handover 6 minutes prior to contractual SLA breach limit.',
  strategies: [
    {
      strategyId: 'STRAT-01',
      strategyNumber: 1,
      title: 'Alternate Route Bypass',
      categoryTag: 'Continuation',
      description: 'Expedited State Highway corridor bypassing KM 142 congestion node.',
      isRecommended: false,
      predictedDeliveryTime: '11:42 AM IST',
      deliveryMargin: '+12m Breach',
      estimatedOutlayFormatted: '₹ 18,500',
      outlayBreakdown: 'Direct Fuel / Toll',
      expectedSlaOutcome: 'Breach / Partial Penalty',
      slaSuccessProbabilityFormatted: '42%',
      liquidatedDamagesImpactFormatted: '₹ 45,000 Penalty',
      isDamagesAvoided: false,
      operationalProtocol: 'Driver Murugan remains in vehicle TRK-882. Reroutes through SH-116. Zero cross-loading required.',
      identifiedConstraints: 'Recurrence risk on alternator belt high. Exceeds permissible continuous driver duty limit by 38 mins.',
      requiredApprovalTier: 'Tier 1',
    },
    {
      strategyId: 'STRAT-02',
      strategyNumber: 2,
      title: 'Relief Cross-Dock Transfer',
      categoryTag: 'Optimized',
      description: 'Direct deployment of secondary pre-cooled reefer with highway mobile transfer.',
      isRecommended: true,
      recommendationBadge: 'RECOMMENDED STRATEGY · Zero SLA Breach',
      predictedDeliveryTime: '11:24 AM IST',
      deliveryMargin: '-6m Prior to Cut-off',
      estimatedOutlayFormatted: '₹ 34,500',
      outlayBreakdown: 'Reefer + Transshipment',
      expectedSlaOutcome: '100% On-Time Preserved',
      slaSuccessProbabilityFormatted: '97.8%',
      liquidatedDamagesImpactFormatted: '₹ 1,35,000 Avoided',
      isDamagesAvoided: true,
      operationalProtocol: 'Dispatch dedicated reefer TRK-914 from Hosur Logistics Hub. Rendezvous at KM 145 Rest Area for 14-min mobile cryogenic cross-dock; bypass via NICE expressway.',
      operationalGovernance: 'Requires dual-driver Murugan/Kiran pallet sign-off. Fits inside Tier 2 authorization threshold (Alex Mercer).',
      requiredApprovalTier: 'Tier 2',
    },
    {
      strategyId: 'STRAT-03',
      strategyNumber: 3,
      title: 'Secondary Depot Swap',
      categoryTag: 'Stock Swap',
      description: 'Release identical batch from Nelamangala Regional Central Depot via priority 3PL courier.',
      isRecommended: false,
      predictedDeliveryTime: '11:28 AM IST',
      deliveryMargin: '-2m Margin',
      estimatedOutlayFormatted: '₹ 58,000',
      outlayBreakdown: 'Expedited 3PL + Swap',
      expectedSlaOutcome: 'SLA Preserved',
      slaSuccessProbabilityFormatted: '98.2%',
      liquidatedDamagesImpactFormatted: '₹ 1,35,000 Avoided',
      isDamagesAvoided: true,
      operationalProtocol: 'Immobilize stranded truck TRK-882. Release secondary reserve batch #VAC-992 from depot. Deliver via partner BlueDart reefer van.',
      governanceAlert: 'Depletes regional hospital buffer. Mandates Tier 4 VP Supply Chain authorization (> ₹ 50,000 threshold).',
      requiredApprovalTier: 'Tier 4',
    },
  ],
  tradeoffSummary: {
    keyTradeoffFinding: 'Strategy 2 preserves the hospital delivery SLA window while saving ₹ 1,00,500 in net penalties versus Strategy 1, and avoids the ₹ 58,000 cost + regional stockout risk of Strategy 3.',
    netBenefitFormatted: '₹ 1,00,500 Net Cost-Benefit Advantage',
    whyRecommended: 'High SLA certainty (97.8%), fits within Tier 2 budget ceiling (< ₹ 50k), and preserves cold-chain buffer without depleting central warehouse reserves.',
    whyRejectedAlternatives: 'Strategy 1 breaches delivery cut-off (+12m) triggering penalties; Strategy 3 exceeds Tier 2 spend threshold and risks buffer depletion at Nelamangala central depot.',
  },
}

// ── Category tag colors
const tagCfg = (tag: string) => {
  const m: Record<string, { bg: string; color: string; border: string }> = {
    Continuation: { bg: '#fff2e8', color: '#ba4e00', border: '#ff832b' },
    Optimized: { bg: '#d0e2ff', color: '#0043ce', border: '#0f62fe' },
    'Stock Swap': { bg: '#e8daff', color: '#6929c4', border: '#a56eff' },
  }
  return m[tag] || { bg: '#f4f4f4', color: '#525252', border: '#c6c6c6' }
}

// ── Individual strategy card
const StrategyCard: React.FC<{
  s: Strategy
  onApprove: (sid: string) => void
  approved: string | null
}> = ({ s, onApprove, approved }) => {
  const isRec = s.isRecommended
  const isApproved = approved === s.strategyId
  const tc = tagCfg(s.categoryTag)

  return (
    <div
      style={{
        background: '#ffffff',
        border: isRec ? '2px solid #0f62fe' : '1px solid #e0e0e0',
        borderRadius: '0',
        boxShadow: 'none',
        display: 'flex',
        flexDirection: 'column',
        position: 'relative',
      }}
    >
      {/* Recommended banner */}
      {isRec && (
        <div
          style={{
            background: '#0f62fe',
            color: '#ffffff',
            fontSize: '11px',
            fontWeight: 700,
            letterSpacing: '0.5px',
            padding: '6px 16px',
            textAlign: 'center',
          }}
        >
          ★ {s.recommendationBadge}
        </div>
      )}

      {/* Card header */}
      <div style={{ padding: '20px 20px 16px 20px', borderBottom: '1px solid #f4f4f4' }}>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: '10px' }}>
          <span
            style={{
              background: tc.bg, color: tc.color, border: `1px solid ${tc.border}`,
              fontSize: '11px', fontWeight: 600, padding: '3px 10px', letterSpacing: '0.3px',
            }}
          >
            {s.categoryTag}
          </span>
          <span style={{ fontSize: '11px', color: '#8d8d8d' }}>
            Approval: <strong>{s.requiredApprovalTier}</strong>
          </span>
        </div>
        <div style={{ fontSize: '11px', color: '#8d8d8d', letterSpacing: '0.32px', textTransform: 'uppercase', marginBottom: '4px' }}>
          Strategy {s.strategyNumber}
        </div>
        <div style={{ fontSize: '18px', fontWeight: 600, color: '#161616', marginBottom: '8px' }}>{s.title}</div>
        <div style={{ fontSize: '13px', color: '#525252' }}>{s.description}</div>
      </div>

      {/* Metrics */}
      <div style={{ padding: '16px 20px', borderBottom: '1px solid #f4f4f4' }}>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '14px' }}>
          {[
            { l: 'Delivery Time', v: s.predictedDeliveryTime, mono: true },
            {
              l: 'Margin',
              v: s.deliveryMargin,
              color: s.deliveryMargin.startsWith('+') ? '#da1e28' : '#198038',
              mono: true,
            },
            { l: 'Outlay', v: s.estimatedOutlayFormatted, mono: true },
            { l: 'Breakdown', v: s.outlayBreakdown, mono: false },
          ].map((f) => (
            <div key={f.l}>
              <div style={{ fontSize: '10px', color: '#8d8d8d', letterSpacing: '0.32px', textTransform: 'uppercase', marginBottom: '3px' }}>{f.l}</div>
              <div style={{
                fontFamily: f.mono ? 'IBM Plex Mono, monospace' : 'IBM Plex Sans, sans-serif',
                fontSize: '13px', fontWeight: f.mono ? 600 : 400,
                color: (f as { color?: string }).color || '#161616',
              }}>
                {f.v}
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* SLA outcome */}
      <div
        style={{
          padding: '14px 20px',
          borderBottom: '1px solid #f4f4f4',
          background: s.isDamagesAvoided ? '#defbe6' : '#fff1f1',
          display: 'flex',
          alignItems: 'center',
          gap: '12px',
        }}
      >
        <span style={{ fontSize: '16px' }}>{s.isDamagesAvoided ? '✅' : '❌'}</span>
        <div>
          <div style={{ fontSize: '12px', fontWeight: 600, color: s.isDamagesAvoided ? '#198038' : '#da1e28' }}>
            {s.expectedSlaOutcome}
          </div>
          <div style={{ fontSize: '12px', color: '#525252' }}>
            {s.liquidatedDamagesImpactFormatted} · SLA Success: <strong style={{ fontFamily: 'IBM Plex Mono, monospace' }}>{s.slaSuccessProbabilityFormatted}</strong>
          </div>
        </div>
      </div>

      {/* Protocol */}
      <div style={{ padding: '16px 20px', flex: 1, borderBottom: '1px solid #f4f4f4' }}>
        <div style={{ fontSize: '11px', color: '#8d8d8d', letterSpacing: '0.32px', textTransform: 'uppercase', marginBottom: '8px' }}>
          Operational Protocol
        </div>
        <div style={{ fontSize: '12px', color: '#525252', lineHeight: '18px' }}>{s.operationalProtocol}</div>
        {s.operationalGovernance && (
          <div style={{ marginTop: '8px', fontSize: '12px', color: '#525252', fontStyle: 'italic' }}>{s.operationalGovernance}</div>
        )}
        {s.identifiedConstraints && (
          <div
            style={{
              marginTop: '10px', fontSize: '12px', color: '#ba4e00',
              background: '#fff2e8', borderLeft: '3px solid #ff832b', padding: '8px 10px',
            }}
          >
            ⚠ {s.identifiedConstraints}
          </div>
        )}
        {s.governanceAlert && (
          <div
            style={{
              marginTop: '10px', fontSize: '12px', color: '#161616',
              background: '#fff1f1', borderLeft: '3px solid #da1e28', padding: '8px 10px',
            }}
          >
            🔒 {s.governanceAlert}
          </div>
        )}
      </div>

      {/* CTA */}
      <div style={{ padding: '16px 20px' }}>
        {isRec ? (
          <button
            className="carbon-btn-primary"
            style={{ width: '100%', justifyContent: 'center' }}
            onClick={() => onApprove(s.strategyId)}
            disabled={isApproved}
          >
            {isApproved ? '✓ Strategy Approved' : `Approve Strategy ${s.strategyNumber} (Recommended) →`}
          </button>
        ) : (
          <button
            className="carbon-btn-ghost"
            style={{ width: '100%', justifyContent: 'center' }}
            onClick={() => onApprove(s.strategyId)}
            disabled={isApproved}
          >
            {isApproved ? '✓ Selected' : `Select Strategy ${s.strategyNumber}`}
          </button>
        )}
      </div>
    </div>
  )
}

// ── Main Page
const RecoverySimulatorPage: React.FC = () => {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [data, setData] = useState<RecoverySimulation>(MOCK)
  const [approved, setApproved] = useState<string | null>(null)
  const [approvalMsg, setApprovalMsg] = useState<string | null>(null)

  useEffect(() => {
    if (!id) return
    getRecoverySimulation(id)
      .then((r) => setData(r.data))
      .catch(() => { /* use mock */ })
  }, [id])

  const handleApprove = (sid: string) => {
    const s = data.strategies.find((x) => x.strategyId === sid)
    setApproved(sid)
    setApprovalMsg(`Strategy "${s?.title}" approved for execution. Dispatching operational orders to field team.`)
  }

  return (
    <div>
      {/* Breadcrumb */}
      <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '20px', fontSize: '12px', color: '#525252' }}>
        <span style={{ color: '#0f62fe', cursor: 'pointer' }} onClick={() => navigate('/control-tower')}>Control Tower</span>
        <span>/</span>
        <span style={{ color: '#0f62fe', cursor: 'pointer' }} onClick={() => navigate(`/slarisk/${data.incidentId}`)}>SLA Risk Engine</span>
        <span>/</span>
        <span>Module 4 — Recovery Simulator</span>
      </div>

      {/* Page header */}
      <div style={{ marginBottom: '20px' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '8px' }}>
          <span style={{ background: '#0f62fe', color: '#fff', fontSize: '11px', fontWeight: 700, padding: '3px 10px', letterSpacing: '0.5px' }}>
            AI OPTIMIZER
          </span>
          <span style={{ fontFamily: 'IBM Plex Mono, monospace', fontSize: '13px', color: '#525252' }}>
            {data.incidentId} · {data.shipmentId}
          </span>
        </div>
        <h1 style={{ fontSize: '24px', fontWeight: 400, lineHeight: '32px', color: '#161616', margin: '0 0 4px 0' }}>
          Recovery Simulator
        </h1>
        <p style={{ color: '#525252', fontSize: '13px', margin: 0, maxWidth: '700px' }}>
          {data.optimizationEngineHeadline}
        </p>
      </div>

      {/* Recommendation summary */}
      <div
        style={{
          background: '#d0e2ff', borderLeft: '4px solid #0f62fe',
          padding: '16px 20px', marginBottom: '24px',
          display: 'flex', gap: '12px', alignItems: 'flex-start',
        }}
      >
        <svg width="20" height="20" viewBox="0 0 20 20" fill="#0f62fe" style={{ flexShrink: 0 }}>
          <path d="M10 1a9 9 0 1 0 0 18A9 9 0 0 0 10 1zm.5 13.5h-1v-6h1v6zm0-8h-1V5h1v1.5z" />
        </svg>
        <div>
          <div style={{ fontWeight: 600, fontSize: '14px', color: '#0043ce', marginBottom: '4px' }}>Optimization Engine Recommendation</div>
          <div style={{ fontSize: '13px', color: '#161616' }}>{data.recommendationSummary}</div>
        </div>
      </div>

      {/* Approval notification */}
      {approvalMsg && (
        <div
          style={{
            background: '#defbe6', borderLeft: '4px solid #198038',
            padding: '14px 18px', marginBottom: '24px',
            display: 'flex', gap: '12px', alignItems: 'center',
          }}
        >
          <span style={{ fontSize: '18px' }}>✅</span>
          <div style={{ fontSize: '13px', color: '#198038', fontWeight: 500 }}>{approvalMsg}</div>
        </div>
      )}

      {/* ── 3-Column Strategy Grid ── */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '0', border: '1px solid #e0e0e0', marginBottom: '32px' }}>
        {data.strategies.map((s, i) => (
          <div key={s.strategyId} style={{ borderRight: i < 2 ? '1px solid #e0e0e0' : 'none' }}>
            <StrategyCard s={s} onApprove={handleApprove} approved={approved} />
          </div>
        ))}
      </div>

      {/* ── Trade-off Summary Banner ── */}
      <div style={{ background: '#f4f4f4', border: '1px solid #e0e0e0', padding: '24px' }}>
        <div style={{ fontWeight: 600, fontSize: '16px', marginBottom: '16px', color: '#161616' }}>
          📊 Trade-Off Analysis Summary
        </div>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr', gap: '24px' }}>
          <div>
            <div style={{ fontSize: '11px', color: '#8d8d8d', letterSpacing: '0.32px', textTransform: 'uppercase', marginBottom: '8px' }}>
              Key Finding
            </div>
            <div style={{ fontSize: '13px', color: '#161616', lineHeight: '20px' }}>
              {data.tradeoffSummary.keyTradeoffFinding}
            </div>
            <div style={{ marginTop: '12px', fontFamily: 'IBM Plex Mono, monospace', fontSize: '16px', fontWeight: 700, color: '#198038' }}>
              {data.tradeoffSummary.netBenefitFormatted}
            </div>
          </div>
          <div>
            <div style={{ fontSize: '11px', color: '#8d8d8d', letterSpacing: '0.32px', textTransform: 'uppercase', marginBottom: '8px' }}>
              Why Strategy 2 is Recommended
            </div>
            <div style={{ fontSize: '13px', color: '#161616', lineHeight: '20px' }}>
              {data.tradeoffSummary.whyRecommended}
            </div>
          </div>
          <div>
            <div style={{ fontSize: '11px', color: '#8d8d8d', letterSpacing: '0.32px', textTransform: 'uppercase', marginBottom: '8px' }}>
              Why Alternatives Were Rejected
            </div>
            <div style={{ fontSize: '13px', color: '#525252', lineHeight: '20px' }}>
              {data.tradeoffSummary.whyRejectedAlternatives}
            </div>
          </div>
        </div>
      </div>

      <div style={{ marginTop: '24px', display: 'flex', gap: '12px', justifyContent: 'flex-end' }}>
        <button className="carbon-btn-ghost" onClick={() => navigate(`/slarisk/${data.incidentId}`)}>
          ← SLA Risk Engine
        </button>
        <button
          onClick={() => navigate('/control-tower')}
          style={{ padding: '12px 20px', background: '#393939', color: '#fff', border: 'none', cursor: 'pointer', fontSize: '14px', borderRadius: '0' }}
        >
          Back to Control Tower
        </button>
      </div>
    </div>
  )
}

export default RecoverySimulatorPage
