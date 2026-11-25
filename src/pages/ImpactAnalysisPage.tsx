// =============================================
// Module 2 — Impact Analysis (Blast Radius)
// IBM Carbon Design System
// =============================================
import React, { useEffect, useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { getImpactAnalysis } from '../services/api'
import type { ImpactAnalysis, GraphNode, GraphEdge } from '../types/api'

// ── Mock payload (matches Ramya's exact contract)
const MOCK: ImpactAnalysis = {
  incidentId: 'INC-1001',
  incidentType: 'Cold-Chain Delay / Mechanical Immobilization',
  rootCause: 'Mechanical brake caliper lock on NH-48 KM 142.4 (EVT-9401 Sensor Triggered)',
  severity: 'CRITICAL P1',
  totalBlastRadius: 14,
  blastRadiusBreakdown: { rootIncidents: 1, shipments: 3, purchaseOrders: 4, consignees: 6 },
  contractualRevenueRisk: 18400000,
  revenueRiskFormatted: '₹ 1.84 Cr',
  cargoValue: 4850000,
  liquidatedPenalty: 13550000,
  affectedCustomerAccounts: 6,
  customerImpactSummary: '06 Accounts (2 Tier-1 Critical: Apollo Hospitals, Fortis)',
  remainingSlaBufferMinutes: 38,
  remainingSlaBufferFormatted: '38 mins',
  exposureDossier: [
    {
      entityId: 'CONS-APOLLO',
      entityName: 'Apollo Hospitals & Research Centre',
      tier: 'Tier-1 Strategic Consignee',
      riskLevel: 'CRITICAL',
      location: 'BLR North Oncology Campus, Bangalore',
      poReference: 'PO-99104',
      valueAtRiskFormatted: '₹ 72.0 Lakhs',
      bufferRemainingFormatted: '38 mins',
      slaPenaltyRate: '₹ 1.2 Lakh / hour',
      clinicalOrBusinessImpact: 'Outpatient surgery oncology ward stockout projected at 11:30 AM IST if cold-chain delivery is not intercepted.',
      cargoDetails: '2,400 Vaccine Vials (2°C - 8°C cold-chain verified)',
    },
  ],
  graphNodes: [
    { id: 'INC-1001', label: 'INC-1001', tierLevel: 0, nodeType: 'IncidentOrigin', status: 'CRITICAL DELAY' },
    { id: 'SHP-1042', label: 'SHP-1042 (Vaccines)', tierLevel: 1, nodeType: 'DirectDisruption', status: 'Critical Delay (+1h 48m)' },
    { id: 'PO-99104', label: 'PO-99104 (Apollo)', tierLevel: 2, nodeType: 'DownstreamPO', status: 'Breach at 11:30 AM IST' },
    { id: 'CONS-APOLLO', label: 'Apollo Hospitals Hub', tierLevel: 3, nodeType: 'EndConsignee', status: 'Immediate Stockout Risk' },
  ],
  graphEdges: [
    { from: 'INC-1001', to: 'SHP-1042', relationship: 'Triggers', label: 'STALL: NH-48', severity: 'Critical' },
    { from: 'SHP-1042', to: 'PO-99104', relationship: 'Carries', label: 'BUFFER 38m', severity: 'Critical' },
    { from: 'PO-99104', to: 'CONS-APOLLO', relationship: 'DeliversTo', label: 'VIP TIER 1', severity: 'Critical' },
  ],
}

// ── Node color by type
const nodeStyle = (type: GraphNode['nodeType']) => {
  const cfg = {
    IncidentOrigin: { bg: '#da1e28', border: '#a2191f', text: '#fff' },
    DirectDisruption: { bg: '#ff832b', border: '#ba4e00', text: '#fff' },
    DownstreamPO: { bg: '#f1c21b', border: '#b28600', text: '#161616' },
    EndConsignee: { bg: '#0f62fe', border: '#0043ce', text: '#fff' },
  }
  return cfg[type]
}

// ── Blast-radius SVG Flow Graph (vertical cascade)
const BlastRadiusGraph: React.FC<{ nodes: GraphNode[]; edges: GraphEdge[] }> = ({ nodes, edges: _edges }) => {
  const W = 600
  const NODE_W = 180
  const NODE_H = 60
  const GAP_Y = 80

  const totalH = nodes.length * (NODE_H + GAP_Y) + 40

  return (
    <svg width="100%" viewBox={`0 0 ${W} ${totalH}`} style={{ overflow: 'visible', display: 'block' }}>
      {nodes.map((n, i) => {
        const x = (W - NODE_W) / 2
        const y = 20 + i * (NODE_H + GAP_Y)
        const style = nodeStyle(n.nodeType)

        // Draw arrow to next node
        const hasNext = i < nodes.length - 1
        const nextY = y + NODE_H + GAP_Y
        const arrowMid = y + NODE_H

        return (
          <g key={n.id}>
            {/* Connector line */}
            {hasNext && (
              <g>
                <line
                  x1={W / 2} y1={arrowMid}
                  x2={W / 2} y2={nextY - 2}
                  stroke="#da1e28" strokeWidth="2" strokeDasharray="6 3"
                />
                {/* Arrow head */}
                <polygon
                  points={`${W / 2 - 6},${nextY - 10} ${W / 2 + 6},${nextY - 10} ${W / 2},${nextY}`}
                  fill="#da1e28"
                />
                {/* Edge label */}
                <text
                  x={W / 2 + 12} y={arrowMid + GAP_Y / 2}
                  fontSize="11" fill="#da1e28" fontFamily="IBM Plex Mono, monospace"
                  dominantBaseline="middle"
                >
                  {_edges[i]?.label}
                </text>
                <text
                  x={W / 2 + 12} y={arrowMid + GAP_Y / 2 + 14}
                  fontSize="10" fill="#8d8d8d" fontFamily="IBM Plex Sans, sans-serif"
                  dominantBaseline="middle"
                >
                  {_edges[i]?.relationship}
                </text>
              </g>
            )}

            {/* Node box */}
            <rect
              x={x} y={y} width={NODE_W} height={NODE_H}
              fill={style.bg} stroke={style.border} strokeWidth="2" rx="0"
            />
            {/* Tier badge */}
            <rect x={x} y={y} width="6" height={NODE_H} fill={style.border} />
            {/* Node label */}
            <text
              x={x + 16} y={y + 22}
              fontSize="13" fontWeight="600" fill={style.text}
              fontFamily="IBM Plex Mono, monospace"
            >
              {n.label}
            </text>
            <text
              x={x + 16} y={y + 40}
              fontSize="10" fill={style.text} opacity={0.85}
              fontFamily="IBM Plex Sans, sans-serif"
            >
              {n.status}
            </text>
            {/* Tier level pill */}
            <text
              x={x + NODE_W - 8} y={y + 16}
              fontSize="10" fill={style.text} opacity={0.7}
              textAnchor="end" fontFamily="IBM Plex Sans"
            >
              Tier {n.tierLevel}
            </text>
          </g>
        )
      })}
    </svg>
  )
}

// ── Blast breakdown donut-style row
const BreakdownRow = ({ label, value, total }: { label: string; value: number; total: number }) => (
  <div style={{ display: 'flex', alignItems: 'center', gap: '12px', padding: '10px 0', borderBottom: '1px solid #f4f4f4' }}>
    <div style={{ flex: 1, fontSize: '13px', color: '#525252' }}>{label}</div>
    <div style={{ width: '120px' }}>
      <div className="carbon-progress-bar">
        <div
          className="carbon-progress-fill"
          style={{ width: `${(value / total) * 100}%`, background: '#0f62fe' }}
        />
      </div>
    </div>
    <div style={{ width: '24px', textAlign: 'right', fontWeight: 700, fontSize: '14px', fontFamily: 'IBM Plex Mono, monospace' }}>
      {value}
    </div>
  </div>
)

// ── Main Page
const ImpactAnalysisPage: React.FC = () => {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [data, setData] = useState<ImpactAnalysis>(MOCK)
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    if (!id) return
    setLoading(true)
    getImpactAnalysis(id)
      .then((r) => setData(r.data))
      .catch(() => { /* use mock */ })
      .finally(() => setLoading(false))
  }, [id])

  const bd = data.blastRadiusBreakdown

  return (
    <div>
      {/* Breadcrumb */}
      <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '20px', fontSize: '12px', color: '#525252' }}>
        <span style={{ color: '#0f62fe', cursor: 'pointer' }} onClick={() => navigate('/control-tower')}>
          Control Tower
        </span>
        <span>/</span>
        <span>Module 2 — Impact Analysis</span>
        <span>/</span>
        <span style={{ color: '#161616', fontFamily: 'IBM Plex Mono, monospace' }}>{data.incidentId}</span>
      </div>

      {/* Page header */}
      <div style={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between', marginBottom: '24px' }}>
        <div>
          <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '6px' }}>
            <span
              style={{
                background: '#da1e28', color: '#fff', fontSize: '11px', fontWeight: 700,
                letterSpacing: '0.5px', padding: '3px 10px', borderRadius: '0',
              }}
            >
              {data.severity}
            </span>
            <span style={{ fontFamily: 'IBM Plex Mono, monospace', fontSize: '13px', color: '#525252' }}>{data.incidentId}</span>
          </div>
          <h1 style={{ fontSize: '24px', fontWeight: 400, lineHeight: '32px', color: '#161616', margin: '0 0 4px 0' }}>
            Impact Analysis — Blast Radius
          </h1>
          <p style={{ color: '#525252', fontSize: '13px', margin: 0 }}>{data.incidentType}</p>
        </div>
        <button className="carbon-btn-primary" onClick={() => navigate(`/slarisk/${data.incidentId}`)}>
          SLA Risk Engine →
        </button>
      </div>

      {/* Inline alert */}
      <div
        style={{
          background: '#fff1f1', borderLeft: '4px solid #da1e28',
          padding: '14px 18px', marginBottom: '28px', display: 'flex', gap: '12px', alignItems: 'flex-start',
        }}
      >
        <svg width="20" height="20" viewBox="0 0 20 20" fill="#da1e28" style={{ flexShrink: 0 }}>
          <path d="M10 2L2 16h16L10 2zm0 3l5.5 9.5h-11L10 5zm-.5 3v3h1V8h-1zm0 4v1.5h1V12h-1z" />
        </svg>
        <div>
          <div style={{ fontWeight: 600, fontSize: '14px', color: '#161616' }}>Root Cause Identified</div>
          <div style={{ fontSize: '13px', color: '#525252', marginTop: '2px' }}>{data.rootCause}</div>
        </div>
      </div>

      {/* ── 4 KPI Cards ── */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '1px', background: '#e0e0e0', marginBottom: '32px' }}>
        {[
          { label: 'Blast Radius', value: data.totalBlastRadius, sub: 'Affected entities', accent: '#da1e28' },
          { label: 'Revenue at Risk', value: data.revenueRiskFormatted, sub: 'Contractual exposure', accent: '#da1e28' },
          { label: 'Accounts Affected', value: data.affectedCustomerAccounts, sub: data.customerImpactSummary, accent: '#ff832b' },
          { label: 'SLA Buffer Remaining', value: data.remainingSlaBufferFormatted, sub: 'Time to breach', accent: '#ff832b' },
        ].map((c) => (
          <div key={c.label} style={{ background: '#fff', padding: '20px 24px' }}>
            <div style={{ fontSize: '11px', color: '#525252', letterSpacing: '0.32px', textTransform: 'uppercase', marginBottom: '12px' }}>{c.label}</div>
            <div style={{ fontSize: '32px', fontWeight: 300, lineHeight: '40px', color: c.accent }}>{c.value}</div>
            <div style={{ fontSize: '11px', color: '#8d8d8d', marginTop: '8px' }}>{c.sub}</div>
          </div>
        ))}
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '24px', marginBottom: '28px' }}>
        {/* Blast Radius Graph */}
        <div style={{ background: '#fff', border: '1px solid #e0e0e0' }}>
          <div style={{ padding: '16px 20px', borderBottom: '1px solid #e0e0e0' }}>
            <div style={{ fontWeight: 600, fontSize: '14px', color: '#161616' }}>Supply Chain Blast Radius</div>
            <div style={{ fontSize: '12px', color: '#8d8d8d', marginTop: '2px' }}>Propagation cascade: Incident → Shipment → PO → Consignee</div>
          </div>
          <div style={{ padding: '24px 32px' }}>
            <BlastRadiusGraph nodes={data.graphNodes} edges={data.graphEdges} />
          </div>
        </div>

        {/* Breakdown + Revenue */}
        <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
          <div style={{ background: '#fff', border: '1px solid #e0e0e0' }}>
            <div style={{ padding: '16px 20px', borderBottom: '1px solid #e0e0e0' }}>
              <div style={{ fontWeight: 600, fontSize: '14px' }}>Blast Breakdown ({data.totalBlastRadius} total)</div>
            </div>
            <div style={{ padding: '12px 20px' }}>
              <BreakdownRow label="Root Incidents" value={bd.rootIncidents} total={data.totalBlastRadius} />
              <BreakdownRow label="Shipments Affected" value={bd.shipments} total={data.totalBlastRadius} />
              <BreakdownRow label="Purchase Orders" value={bd.purchaseOrders} total={data.totalBlastRadius} />
              <BreakdownRow label="Consignees at Risk" value={bd.consignees} total={data.totalBlastRadius} />
            </div>
          </div>

          <div style={{ background: '#fff', border: '1px solid #e0e0e0', padding: '20px' }}>
            <div style={{ fontWeight: 600, fontSize: '14px', marginBottom: '16px' }}>Revenue Risk Breakdown</div>
            {[
              { label: 'Cargo Value', val: `₹ ${(data.cargoValue / 100000).toFixed(2)} L`, color: '#8d8d8d' },
              { label: 'Liquidated Damages', val: `₹ ${(data.liquidatedPenalty / 10000000).toFixed(2)} Cr`, color: '#da1e28' },
              { label: 'Total Exposure', val: data.revenueRiskFormatted, color: '#da1e28', bold: true },
            ].map((r) => (
              <div key={r.label} style={{ display: 'flex', justifyContent: 'space-between', padding: '10px 0', borderBottom: '1px solid #f4f4f4', alignItems: 'center' }}>
                <span style={{ fontSize: '13px', color: '#525252' }}>{r.label}</span>
                <span style={{ fontFamily: 'IBM Plex Mono, monospace', fontSize: '14px', fontWeight: r.bold ? 700 : 400, color: r.color }}>{r.val}</span>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Exposure Dossier */}
      <div style={{ background: '#fff', border: '1px solid #e0e0e0' }}>
        <div style={{ padding: '16px 20px', borderBottom: '1px solid #e0e0e0', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div style={{ fontWeight: 600, fontSize: '14px' }}>Exposure Dossier — Consignee Risk Profiles</div>
        </div>
        {data.exposureDossier.map((d) => (
          <div key={d.entityId} style={{ padding: '20px', borderBottom: '1px solid #f4f4f4' }}>
            <div style={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between', marginBottom: '12px' }}>
              <div>
                <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '4px' }}>
                  <span
                    style={{
                      background: '#da1e28', color: '#fff', fontSize: '11px', fontWeight: 700,
                      padding: '2px 8px', letterSpacing: '0.3px',
                    }}
                  >
                    {d.riskLevel}
                  </span>
                  <span style={{ fontSize: '11px', color: '#525252', border: '1px solid #c6c6c6', padding: '2px 8px' }}>{d.tier}</span>
                </div>
                <div style={{ fontWeight: 600, fontSize: '16px', color: '#161616' }}>{d.entityName}</div>
                <div style={{ fontSize: '12px', color: '#525252', marginTop: '2px' }}>{d.location}</div>
              </div>
              <div style={{ textAlign: 'right' }}>
                <div style={{ fontFamily: 'IBM Plex Mono, monospace', fontSize: '20px', fontWeight: 600, color: '#da1e28' }}>{d.valueAtRiskFormatted}</div>
                <div style={{ fontSize: '11px', color: '#8d8d8d' }}>Value at Risk</div>
              </div>
            </div>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '16px', marginBottom: '12px' }}>
              {[
                { l: 'PO Reference', v: d.poReference },
                { l: 'Buffer Remaining', v: d.bufferRemainingFormatted },
                { l: 'SLA Penalty Rate', v: d.slaPenaltyRate },
              ].map((f) => (
                <div key={f.l}>
                  <div style={{ fontSize: '11px', color: '#8d8d8d', letterSpacing: '0.32px', textTransform: 'uppercase', marginBottom: '4px' }}>{f.l}</div>
                  <div style={{ fontFamily: 'IBM Plex Mono, monospace', fontSize: '13px', fontWeight: 600, color: '#161616' }}>{f.v}</div>
                </div>
              ))}
            </div>
            <div
              style={{
                background: '#fff1f1', borderLeft: '3px solid #da1e28',
                padding: '10px 14px', fontSize: '13px', color: '#525252', marginBottom: '8px',
              }}
            >
              ⚠ {d.clinicalOrBusinessImpact}
            </div>
            <div style={{ fontSize: '12px', color: '#525252' }}>
              <strong>Cargo:</strong> {d.cargoDetails}
            </div>
          </div>
        ))}
      </div>

      <div style={{ marginTop: '24px', display: 'flex', gap: '12px', justifyContent: 'flex-end' }}>
        <button className="carbon-btn-ghost" onClick={() => navigate('/control-tower')}>← Back to Control Tower</button>
        <button className="carbon-btn-primary" onClick={() => navigate(`/slarisk/${data.incidentId}`)}>
          Module 3: SLA Risk Engine →
        </button>
      </div>

      {loading && <div style={{ position: 'fixed', bottom: '20px', right: '20px', background: '#0f62fe', color: '#fff', padding: '8px 16px', fontSize: '13px' }}>Syncing…</div>}
    </div>
  )
}

export default ImpactAnalysisPage
