// =============================================
// Module 3 — SLA Risk Engine
// IBM Carbon Design System
// =============================================
import React, { useEffect, useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import {
  LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip,
  ReferenceLine, ResponsiveContainer, Legend,
} from 'recharts'
import { getSlaRisk } from '../services/api'
import type { SlaRisk } from '../types/api'

// ── Mock data (Ramya's exact contract)
const MOCK: SlaRisk = {
  incidentId: 'INC-1001',
  shipmentId: 'SHP-1042',
  route: 'Chennai → Bengaluru · NH-48 KM 142.4',
  severity: 'CRITICAL P1',
  breachProbability: 94.2,
  breachProbabilityFormatted: '94.2%',
  riskTier: 'Critical Risk',
  riskBaselineDelta: '+18.4% vs baseline',
  timeToSlaBreachMinutes: 38,
  timeToSlaBreachFormatted: '38 min',
  hardBreachThresholdTime: '11:30 AM IST',
  projectedDelay: '+1h 48m',
  committedSlaWindow: '11:30 AM IST',
  financialExposure: 135000,
  financialExposureFormatted: '₹ 1.35L',
  remainingBufferBadge: 'Remaining Buffer: 38 min',
  breachProbabilityTrend: [
    { timeLabel: '09:30', normalTransitProbability: 5.0, breachCurveProbability: 5.0 },
    { timeLabel: '10:00', normalTransitProbability: 6.2, breachCurveProbability: 14.8 },
    { timeLabel: '10:15', normalTransitProbability: 7.5, breachCurveProbability: 46.5 },
    { timeLabel: '10:30 (NOW)', normalTransitProbability: 8.8, breachCurveProbability: 94.2, isCurrentPoint: true },
    { timeLabel: '11:00', normalTransitProbability: 9.5, breachCurveProbability: 99.1 },
    { timeLabel: '11:30', normalTransitProbability: 10.0, breachCurveProbability: 100.0, isHardBreachThreshold: true },
  ],
  contributingFactors: [
    { factorName: 'Vehicle Immobilization', severityBadge: 'Critical · 58% Contributor', contributionPercentage: 58, description: 'Mechanical brake caliper issue on NH-48 KM 142.4. Awaiting mobile repair unit arrival.' },
    { factorName: 'Route Congestion', severityBadge: 'High · 24% Contributor', contributionPercentage: 24, description: 'Peak expressway backlog entering Bengaluru South arterial corridor.' },
    { factorName: 'Remaining Delivery Buffer', severityBadge: 'High · 18% Contributor', contributionPercentage: 18, description: 'Turnaround ETA exceeds contractual grace window by 70 minutes without intervention.' },
  ],
  shipmentContext: {
    consignee: 'Apollo Hospitals & Research Centre',
    masterSla: 'Tier 1 Cold-Chain Compliance',
    criticality: 'High-priority Medical Supplies (Vaccines)',
    temperatureStatus: 'Safe (+4.1°C)',
    assignedVehicle: 'TRK-882 (Reefer Unit)',
  },
}

// ── Custom recharts dot for "NOW" and breach threshold
const CustomDot = (props: { cx?: number; cy?: number; payload?: { isCurrentPoint?: boolean; isHardBreachThreshold?: boolean } }) => {
  const { cx = 0, cy = 0, payload } = props
  if (payload?.isCurrentPoint) {
    return <circle cx={cx} cy={cy} r={8} fill="#da1e28" stroke="#fff" strokeWidth={2} />
  }
  if (payload?.isHardBreachThreshold) {
    return <circle cx={cx} cy={cy} r={6} fill="#ff832b" stroke="#fff" strokeWidth={2} />
  }
  return null
}

// ── Factor bar
const FactorBar = ({ f }: { f: SlaRisk['contributingFactors'][0] }) => {
  const color = f.contributionPercentage >= 50 ? '#da1e28' : f.contributionPercentage >= 25 ? '#ff832b' : '#f1c21b'
  return (
    <div style={{ padding: '16px 20px', borderBottom: '1px solid #f4f4f4' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '8px' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
          <span style={{ fontWeight: 600, fontSize: '14px', color: '#161616' }}>{f.factorName}</span>
          <span style={{ fontSize: '11px', color, border: `1px solid ${color}`, padding: '2px 8px', background: `${color}18` }}>
            {f.severityBadge}
          </span>
        </div>
        <span style={{ fontFamily: 'IBM Plex Mono, monospace', fontWeight: 700, fontSize: '18px', color }}>{f.contributionPercentage}%</span>
      </div>
      <div style={{ background: '#e0e0e0', height: '8px', borderRadius: '0', marginBottom: '8px' }}>
        <div style={{ width: `${f.contributionPercentage}%`, height: '100%', background: color, transition: 'width 0.8s ease' }} />
      </div>
      <div style={{ fontSize: '12px', color: '#525252' }}>{f.description}</div>
    </div>
  )
}

// ── Main Page
const SlaRiskPage: React.FC = () => {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [data, setData] = useState<SlaRisk>(MOCK)

  useEffect(() => {
    if (!id) return
    getSlaRisk(id)
      .then((r) => setData(r.data))
      .catch(() => { /* use mock */ })
  }, [id])

  const topCards = [
    { label: 'Breach Probability', value: data.breachProbabilityFormatted, sub: data.riskBaselineDelta, accent: '#da1e28', big: true },
    { label: 'Time to SLA Breach', value: data.timeToSlaBreachFormatted, sub: `Hard limit: ${data.hardBreachThresholdTime}`, accent: '#ff832b', big: false },
    { label: 'Projected Delay', value: data.projectedDelay, sub: `SLA window: ${data.committedSlaWindow}`, accent: '#ff832b', big: false },
    { label: 'Financial Exposure', value: data.financialExposureFormatted, sub: 'Liquidated damages', accent: '#da1e28', big: false },
  ]

  return (
    <div>
      {/* Breadcrumb */}
      <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '20px', fontSize: '12px', color: '#525252' }}>
        <span style={{ color: '#0f62fe', cursor: 'pointer' }} onClick={() => navigate('/control-tower')}>Control Tower</span>
        <span>/</span>
        <span style={{ color: '#0f62fe', cursor: 'pointer' }} onClick={() => navigate(`/impact/${data.incidentId}`)}>Impact Analysis</span>
        <span>/</span>
        <span>Module 3 — SLA Risk Engine</span>
      </div>

      {/* Page header */}
      <div style={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between', marginBottom: '24px' }}>
        <div>
          <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '6px' }}>
            <span style={{ background: '#da1e28', color: '#fff', fontSize: '11px', fontWeight: 700, letterSpacing: '0.5px', padding: '3px 10px' }}>
              {data.severity}
            </span>
            <span style={{ fontFamily: 'IBM Plex Mono, monospace', fontSize: '13px', color: '#525252' }}>
              {data.incidentId} · {data.shipmentId}
            </span>
          </div>
          <h1 style={{ fontSize: '24px', fontWeight: 400, lineHeight: '32px', color: '#161616', margin: '0 0 4px 0' }}>
            SLA Risk Engine
          </h1>
          <p style={{ color: '#525252', fontSize: '13px', margin: 0 }}>{data.route}</p>
        </div>
        <button className="carbon-btn-primary" onClick={() => navigate(`/simulation/${data.incidentId}`)}>
          Recovery Simulator →
        </button>
      </div>

      {/* Buffer badge */}
      <div
        style={{
          display: 'inline-flex', alignItems: 'center', gap: '8px',
          background: '#fff2e8', border: '1px solid #ff832b', color: '#ba4e00',
          padding: '8px 16px', fontSize: '13px', fontWeight: 600, marginBottom: '24px',
        }}
      >
        ⏱ {data.remainingBufferBadge}
      </div>

      {/* ── 4 Top Cards ── */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '1px', background: '#e0e0e0', marginBottom: '32px' }}>
        {topCards.map((c) => (
          <div key={c.label} style={{ background: '#fff', padding: '20px 24px' }}>
            <div style={{ fontSize: '11px', color: '#525252', letterSpacing: '0.32px', textTransform: 'uppercase', marginBottom: '12px' }}>{c.label}</div>
            <div style={{ fontSize: c.big ? '40px' : '32px', fontWeight: 300, lineHeight: '44px', color: c.accent, fontFamily: 'IBM Plex Mono, monospace' }}>
              {c.value}
            </div>
            <div style={{ fontSize: '11px', color: '#8d8d8d', marginTop: '8px' }}>{c.sub}</div>
          </div>
        ))}
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '3fr 2fr', gap: '24px', marginBottom: '28px' }}>
        {/* ── Breach Probability Chart ── */}
        <div style={{ background: '#fff', border: '1px solid #e0e0e0' }}>
          <div style={{ padding: '16px 20px', borderBottom: '1px solid #e0e0e0' }}>
            <div style={{ fontWeight: 600, fontSize: '14px' }}>Breach Probability Trend</div>
            <div style={{ fontSize: '12px', color: '#8d8d8d', marginTop: '2px' }}>
              Blue = Normal transit baseline · Red = Live breach escalation curve
            </div>
          </div>
          <div style={{ padding: '24px' }}>
            <ResponsiveContainer width="100%" height={280}>
              <LineChart data={data.breachProbabilityTrend} margin={{ top: 5, right: 20, left: 0, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="#f4f4f4" vertical={false} />
                <XAxis
                  dataKey="timeLabel"
                  tick={{ fontSize: 11, fontFamily: 'IBM Plex Mono, monospace', fill: '#525252' }}
                  axisLine={{ stroke: '#e0e0e0' }}
                  tickLine={false}
                />
                <YAxis
                  domain={[0, 100]}
                  tick={{ fontSize: 11, fontFamily: 'IBM Plex Mono, monospace', fill: '#525252' }}
                  axisLine={false}
                  tickLine={false}
                  tickFormatter={(v) => `${v}%`}
                  width={40}
                />
                <Tooltip
                  formatter={(val: number, name: string) => [`${val}%`, name === 'breachCurveProbability' ? 'Breach Probability' : 'Normal Transit']}
                  contentStyle={{ background: '#262626', border: 'none', borderRadius: '0', color: '#f4f4f4', fontSize: '12px' }}
                  labelStyle={{ color: '#c6c6c6', fontFamily: 'IBM Plex Mono, monospace' }}
                />
                <Legend
                  formatter={(val) => val === 'breachCurveProbability' ? 'Breach Curve' : 'Normal Transit'}
                  wrapperStyle={{ fontSize: '12px', paddingTop: '8px' }}
                />
                {/* Hard breach vertical line at 11:30 */}
                <ReferenceLine x="11:30" stroke="#ff832b" strokeDasharray="6 3" strokeWidth={2}
                  label={{ value: 'SLA CUT-OFF', position: 'top', fontSize: 10, fill: '#ff832b', fontFamily: 'IBM Plex Mono, monospace' }} />
                {/* NOW reference line */}
                <ReferenceLine x="10:30 (NOW)" stroke="#da1e28" strokeDasharray="4 2" strokeWidth={1.5}
                  label={{ value: 'NOW', position: 'top', fontSize: 10, fill: '#da1e28', fontFamily: 'IBM Plex Mono, monospace' }} />

                <Line
                  type="monotone"
                  dataKey="normalTransitProbability"
                  stroke="#0f62fe"
                  strokeWidth={2}
                  dot={false}
                  strokeDasharray="4 2"
                />
                <Line
                  type="monotone"
                  dataKey="breachCurveProbability"
                  stroke="#da1e28"
                  strokeWidth={3}
                  dot={<CustomDot />}
                />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* ── Shipment Context ── */}
        <div style={{ background: '#fff', border: '1px solid #e0e0e0' }}>
          <div style={{ padding: '16px 20px', borderBottom: '1px solid #e0e0e0' }}>
            <div style={{ fontWeight: 600, fontSize: '14px' }}>Shipment Context</div>
          </div>
          <div style={{ padding: '20px' }}>
            {Object.entries(data.shipmentContext).map(([k, v]) => (
              <div key={k} style={{ display: 'flex', flexDirection: 'column', gap: '2px', paddingBottom: '14px', marginBottom: '14px', borderBottom: '1px solid #f4f4f4' }}>
                <span style={{ fontSize: '11px', color: '#8d8d8d', letterSpacing: '0.32px', textTransform: 'uppercase' }}>
                  {k.replace(/([A-Z])/g, ' $1').trim()}
                </span>
                <span style={{
                  fontSize: '13px', fontWeight: 500, color: '#161616',
                  fontFamily: k === 'assignedVehicle' || k === 'temperatureStatus' ? 'IBM Plex Mono, monospace' : 'IBM Plex Sans, sans-serif',
                }}>
                  {v}
                </span>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* ── Contributing Factors ── */}
      <div style={{ background: '#fff', border: '1px solid #e0e0e0', marginBottom: '28px' }}>
        <div style={{ padding: '16px 20px', borderBottom: '1px solid #e0e0e0' }}>
          <div style={{ fontWeight: 600, fontSize: '14px' }}>Contributing Risk Factors</div>
          <div style={{ fontSize: '12px', color: '#8d8d8d', marginTop: '2px' }}>
            Percentage contribution to breach probability
          </div>
        </div>
        {data.contributingFactors.map((f) => <FactorBar key={f.factorName} f={f} />)}
      </div>

      <div style={{ display: 'flex', gap: '12px', justifyContent: 'flex-end' }}>
        <button className="carbon-btn-ghost" onClick={() => navigate(`/impact/${data.incidentId}`)}>← Impact Analysis</button>
        <button className="carbon-btn-primary" onClick={() => navigate(`/simulation/${data.incidentId}`)}>
          Module 4: Recovery Simulator →
        </button>
      </div>
    </div>
  )
}

export default SlaRiskPage
