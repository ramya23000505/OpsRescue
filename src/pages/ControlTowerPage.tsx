// =============================================
// Module 8 — Real-Time Control Tower
// IBM Carbon Design System
// =============================================
import React, { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { getControlTowerMetrics, getIncidents } from '../services/api'
import type { IncidentRow } from '../types/api'

// ── Mock data (used until backend is live)
const MOCK_METRICS = {
  activeIncidents: 3,
  criticalIncidents: 1,
  revenueAtRisk: '₹ 1.84 Cr',
  avgSlaBufferMinutes: 38,
  shipmentsMonitored: 142,
}

const MOCK_INCIDENTS: IncidentRow[] = [
  {
    id: 'INC-1001',
    type: 'Cold-Chain Delay / Mechanical Immobilization',
    severity: 'P1',
    route: 'Chennai → Bengaluru · NH-48',
    blastRadius: 14,
    revenueRisk: '₹ 1.84 Cr',
    slaBuffer: '38 min',
    status: 'ACTIVE',
    timestamp: '10:30 AM IST',
  },
  {
    id: 'INC-1002',
    type: 'Route Deviation — Customs Hold',
    severity: 'P2',
    route: 'Mumbai → Pune · NH-48 E',
    blastRadius: 6,
    revenueRisk: '₹ 42.5 L',
    slaBuffer: '2h 14m',
    status: 'ESCALATED',
    timestamp: '09:15 AM IST',
  },
  {
    id: 'INC-1003',
    type: 'Temperature Excursion Alert',
    severity: 'P2',
    route: 'Delhi → Lucknow · NH-9',
    blastRadius: 4,
    revenueRisk: '₹ 18.0 L',
    slaBuffer: '4h 02m',
    status: 'ACTIVE',
    timestamp: '08:45 AM IST',
  },
  {
    id: 'INC-1004',
    type: 'Vehicle Breakdown — Tyre Burst',
    severity: 'P3',
    route: 'Hyderabad → Chennai · NH-44',
    blastRadius: 2,
    revenueRisk: '₹ 6.2 L',
    slaBuffer: '5h 30m',
    status: 'RESOLVED',
    timestamp: '07:30 AM IST',
  },
]

// ── Severity badge
const SeverityBadge = ({ s }: { s: 'P1' | 'P2' | 'P3' }) => {
  const cfg = {
    P1: { bg: '#da1e28', label: 'P1 · CRITICAL' },
    P2: { bg: '#ff832b', label: 'P2 · HIGH' },
    P3: { bg: '#f1c21b', label: 'P3 · MEDIUM' },
  }[s]
  return (
    <span
      style={{
        display: 'inline-block',
        background: cfg.bg,
        color: s === 'P3' ? '#161616' : '#fff',
        fontSize: '11px',
        fontWeight: 600,
        letterSpacing: '0.32px',
        padding: '2px 8px',
        borderRadius: '0',
      }}
    >
      {cfg.label}
    </span>
  )
}

// ── Status badge
const StatusBadge = ({ s }: { s: 'ACTIVE' | 'ESCALATED' | 'RESOLVED' }) => {
  const cfg = {
    ACTIVE: { bg: '#fff1f1', color: '#da1e28', border: '#da1e28', dot: '#da1e28' },
    ESCALATED: { bg: '#fff2e8', color: '#ba4e00', border: '#ba4e00', dot: '#ff832b' },
    RESOLVED: { bg: '#defbe6', color: '#198038', border: '#198038', dot: '#24a148' },
  }[s]
  return (
    <span
      style={{
        display: 'inline-flex',
        alignItems: 'center',
        gap: '6px',
        background: cfg.bg,
        color: cfg.color,
        border: `1px solid ${cfg.border}`,
        fontSize: '12px',
        fontWeight: 400,
        letterSpacing: '0.32px',
        padding: '2px 8px',
        borderRadius: '0',
      }}
    >
      <span style={{ width: '6px', height: '6px', borderRadius: '50%', background: cfg.dot, flexShrink: 0 }} />
      {s}
    </span>
  )
}

// ── Top metric card
const MetricCard = ({
  label,
  value,
  subtext,
  accent,
  icon,
}: {
  label: string
  value: string | number
  subtext?: string
  accent?: string
  icon?: React.ReactNode
}) => (
  <div
    style={{
      background: '#ffffff',
      border: '1px solid #e0e0e0',
      borderRadius: '0',
      boxShadow: 'none',
      padding: '20px 24px',
      display: 'flex',
      flexDirection: 'column',
      gap: '12px',
    }}
  >
    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
      <span style={{ fontSize: '12px', color: '#525252', letterSpacing: '0.32px', textTransform: 'uppercase', fontWeight: 400 }}>
        {label}
      </span>
      {icon && <span style={{ color: accent || '#525252' }}>{icon}</span>}
    </div>
    <div style={{ fontSize: '36px', fontWeight: 300, lineHeight: '44px', color: accent || '#161616' }}>
      {value}
    </div>
    {subtext && (
      <div style={{ fontSize: '12px', color: '#8d8d8d', letterSpacing: '0.32px' }}>{subtext}</div>
    )}
  </div>
)

// ── Live clock
const LiveClock = () => {
  const [t, setT] = useState(() => new Date())
  useEffect(() => {
    const id = setInterval(() => setT(new Date()), 1000)
    return () => clearInterval(id)
  }, [])
  return (
    <span style={{ fontFamily: 'IBM Plex Mono, monospace', fontSize: '13px', color: '#8d8d8d' }}>
      {t.toLocaleTimeString('en-IN', { hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: true })} IST
    </span>
  )
}

// ── Main page
const ControlTowerPage: React.FC = () => {
  const navigate = useNavigate()
  const [metrics, setMetrics] = useState(MOCK_METRICS)
  const [incidents, setIncidents] = useState<IncidentRow[]>(MOCK_INCIDENTS)
  const [loading, setLoading] = useState(false)
  const [filter, setFilter] = useState<'ALL' | 'P1' | 'P2' | 'P3' | 'ACTIVE' | 'ESCALATED' | 'RESOLVED'>('ALL')

  useEffect(() => {
    // Attempt live API; fall back gracefully to mock
    setLoading(true)
    Promise.allSettled([
      getControlTowerMetrics(),
      getIncidents(),
    ]).then(([m, i]) => {
      if (m.status === 'fulfilled') setMetrics(m.value.data)
      if (i.status === 'fulfilled') setIncidents(i.value.data)
    }).finally(() => setLoading(false))
  }, [])

  const filtered = incidents.filter((inc) => {
    if (filter === 'ALL') return true
    if (['P1', 'P2', 'P3'].includes(filter)) return inc.severity === filter
    return inc.status === filter
  })

  const filters: Array<typeof filter> = ['ALL', 'P1', 'P2', 'P3', 'ACTIVE', 'ESCALATED', 'RESOLVED']

  return (
    <div>
      {/* Page header */}
      <div
        style={{
          display: 'flex',
          alignItems: 'flex-start',
          justifyContent: 'space-between',
          marginBottom: '32px',
        }}
      >
        <div>
          <div style={{ display: 'flex', alignItems: 'center', gap: '12px', marginBottom: '4px' }}>
            <span
              style={{
                display: 'inline-flex',
                alignItems: 'center',
                gap: '6px',
                background: '#fff1f1',
                border: '1px solid #da1e28',
                color: '#da1e28',
                fontSize: '11px',
                fontWeight: 600,
                letterSpacing: '0.5px',
                padding: '3px 10px',
              }}
            >
              <span style={{ width: '6px', height: '6px', borderRadius: '50%', background: '#da1e28', animation: 'pulse 1.5s infinite' }} />
              LIVE
            </span>
            <LiveClock />
          </div>
          <h1 style={{ fontSize: '28px', fontWeight: 300, lineHeight: '36px', color: '#161616', margin: 0 }}>
            Real-Time Control Tower
          </h1>
          <p style={{ color: '#525252', fontSize: '14px', marginTop: '4px' }}>
            Module 8 · Active incident monitoring &amp; supply chain command
          </p>
        </div>

        <button
          className="carbon-btn-primary"
          style={{ padding: '11px 20px', fontSize: '13px', gap: '8px' }}
          onClick={() => navigate('/impact/INC-1001')}
        >
          <span>Open INC-1001 Dossier</span>
          <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
            <path d="M11 8l-5 5-1.4-1.4L8.2 8 4.6 4.4 6 3z" />
          </svg>
        </button>
      </div>

      {/* ── 4 Metric Cards ── */}
      <div
        style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(4, 1fr)',
          gap: '1px',
          background: '#e0e0e0',
          border: '1px solid #e0e0e0',
          marginBottom: '32px',
        }}
      >
        <MetricCard
          label="Active Incidents"
          value={metrics.activeIncidents}
          subtext={`${metrics.criticalIncidents} Critical P1`}
          accent="#da1e28"
          icon={
            <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
              <path d="M10 2L2 16h16L10 2zm0 3l5.5 9.5h-11L10 5zm-.5 3v3h1V8h-1zm0 4v1.5h1V12h-1z" />
            </svg>
          }
        />
        <MetricCard
          label="Revenue at Risk"
          value={metrics.revenueAtRisk}
          subtext="Contractual exposure today"
          accent="#da1e28"
          icon={
            <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
              <path d="M10 1a9 9 0 1 0 0 18A9 9 0 0 0 10 1zm.5 13.5h-1v-5h1v5zm0-7h-1V6h1v1.5z" />
            </svg>
          }
        />
        <MetricCard
          label="Avg SLA Buffer"
          value={`${metrics.avgSlaBufferMinutes} min`}
          subtext="Remaining across active incidents"
          accent="#ff832b"
        />
        <MetricCard
          label="Shipments Monitored"
          value={metrics.shipmentsMonitored}
          subtext="Live tracking active"
          accent="#0f62fe"
        />
      </div>

      {/* ── Incidents Table ── */}
      <div style={{ background: '#ffffff', border: '1px solid #e0e0e0' }}>
        {/* Table toolbar */}
        <div
          style={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            padding: '16px 20px',
            borderBottom: '1px solid #e0e0e0',
          }}
        >
          <div>
            <span style={{ fontWeight: 600, fontSize: '14px', color: '#161616' }}>
              Active Incidents
            </span>
            <span style={{ color: '#525252', fontSize: '12px', marginLeft: '8px' }}>
              ({filtered.length} incidents)
            </span>
          </div>

          {/* Filter pills */}
          <div style={{ display: 'flex', gap: '4px', flexWrap: 'wrap' }}>
            {filters.map((f) => (
              <button
                key={f}
                onClick={() => setFilter(f)}
                style={{
                  padding: '4px 12px',
                  fontSize: '12px',
                  border: '1px solid',
                  borderColor: filter === f ? '#0f62fe' : '#e0e0e0',
                  background: filter === f ? '#d0e2ff' : '#f4f4f4',
                  color: filter === f ? '#0043ce' : '#525252',
                  cursor: 'pointer',
                  borderRadius: '0',
                  fontWeight: filter === f ? 600 : 400,
                }}
              >
                {f}
              </button>
            ))}
          </div>
        </div>

        <div style={{ overflowX: 'auto' }}>
          <table className="carbon-table">
            <thead>
              <tr>
                <th>Incident ID</th>
                <th>Type</th>
                <th>Severity</th>
                <th>Route</th>
                <th>Blast Radius</th>
                <th>Revenue Risk</th>
                <th>SLA Buffer</th>
                <th>Status</th>
                <th>Timestamp</th>
                <th style={{ textAlign: 'center' }}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {filtered.length === 0 ? (
                <tr>
                  <td colSpan={10} style={{ textAlign: 'center', color: '#8d8d8d', padding: '40px' }}>
                    No incidents match the selected filter.
                  </td>
                </tr>
              ) : (
                filtered.map((inc) => (
                  <tr key={inc.id}>
                    <td>
                      <span
                        style={{ fontFamily: 'IBM Plex Mono, monospace', fontSize: '13px', fontWeight: 600, color: '#0f62fe', cursor: 'pointer' }}
                        onClick={() => navigate(`/impact/${inc.id}`)}
                      >
                        {inc.id}
                      </span>
                    </td>
                    <td style={{ maxWidth: '220px', fontSize: '13px' }}>{inc.type}</td>
                    <td><SeverityBadge s={inc.severity} /></td>
                    <td style={{ fontSize: '13px', color: '#525252', fontFamily: 'IBM Plex Mono, monospace' }}>{inc.route}</td>
                    <td>
                      <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                        <div style={{
                          width: '32px', height: '32px', borderRadius: '50%',
                          background: inc.blastRadius >= 10 ? '#da1e28' : inc.blastRadius >= 5 ? '#ff832b' : '#8d8d8d',
                          color: '#fff', display: 'flex', alignItems: 'center', justifyContent: 'center',
                          fontSize: '12px', fontWeight: 700,
                        }}>
                          {inc.blastRadius}
                        </div>
                        <span style={{ fontSize: '11px', color: '#8d8d8d' }}>entities</span>
                      </div>
                    </td>
                    <td style={{ fontWeight: 600, color: inc.status !== 'RESOLVED' ? '#da1e28' : '#198038', fontFamily: 'IBM Plex Mono, monospace', fontSize: '13px' }}>
                      {inc.revenueRisk}
                    </td>
                    <td style={{ fontFamily: 'IBM Plex Mono, monospace', fontSize: '13px' }}>{inc.slaBuffer}</td>
                    <td><StatusBadge s={inc.status} /></td>
                    <td style={{ fontSize: '12px', color: '#8d8d8d', fontFamily: 'IBM Plex Mono, monospace' }}>{inc.timestamp}</td>
                    <td style={{ textAlign: 'center' }}>
                      <div style={{ display: 'flex', gap: '4px', justifyContent: 'center' }}>
                        <button
                          onClick={() => navigate(`/impact/${inc.id}`)}
                          className="carbon-btn-ghost"
                          style={{ padding: '6px 12px', fontSize: '12px', minHeight: '32px' }}
                        >
                          Impact
                        </button>
                        <button
                          onClick={() => navigate(`/slarisk/${inc.id}`)}
                          style={{
                            padding: '6px 12px', fontSize: '12px', border: '1px solid #e0e0e0',
                            background: '#f4f4f4', color: '#525252', cursor: 'pointer', borderRadius: '0',
                          }}
                        >
                          SLA
                        </button>
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {loading && (
          <div style={{ padding: '12px 20px', color: '#525252', fontSize: '12px', borderTop: '1px solid #e0e0e0' }}>
            Syncing with backend…
          </div>
        )}
      </div>
    </div>
  )
}

export default ControlTowerPage
