// =============================================
// App Shell Layout — IBM Carbon UI Shell
// =============================================
import React from 'react'
import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

const NAV_ITEMS = [
  {
    icon: '⬛',
    label: 'Control Tower',
    path: '/control-tower',
    desc: 'Module 8 · Live Dashboard',
  },
  {
    icon: '🔴',
    label: 'Impact Analysis',
    path: '/impact/INC-1001',
    desc: 'Module 2 · Blast Radius',
  },
  {
    icon: '📉',
    label: 'SLA Risk Engine',
    path: '/slarisk/INC-1001',
    desc: 'Module 3 · Breach Probability',
  },
  {
    icon: '🔄',
    label: 'Recovery Simulator',
    path: '/simulation/INC-1001',
    desc: 'Module 4 · Strategy Comparison',
  },
]

const AppLayout: React.FC = () => {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  return (
    <div style={{ minHeight: '100vh', background: '#f4f4f4' }}>
      {/* ── Top Navigation Bar ── */}
      <header
        style={{
          position: 'fixed', top: 0, left: 0, right: 0,
          height: '48px', background: '#161616',
          display: 'flex', alignItems: 'center',
          padding: '0 16px', zIndex: 100,
          borderBottom: '1px solid #393939',
        }}
      >
        {/* Logo */}
        <div
          style={{ display: 'flex', alignItems: 'center', gap: '10px', marginRight: '32px', cursor: 'pointer' }}
          onClick={() => navigate('/control-tower')}
        >
          <div
            style={{
              width: '32px', height: '32px', background: '#0f62fe',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
              flexShrink: 0,
            }}
          >
            <span style={{ color: '#fff', fontWeight: 700, fontSize: '12px' }}>OR</span>
          </div>
          <span style={{ color: '#f4f4f4', fontWeight: 600, fontSize: '14px', letterSpacing: '0.16px' }}>
            OpsRescue
          </span>
          <span style={{ color: '#525252', fontSize: '12px' }}>/ Supply Chain Intelligence</span>
        </div>

        {/* Live status pill */}
        <div
          style={{
            display: 'flex', alignItems: 'center', gap: '6px',
            background: '#2d1515', border: '1px solid #da1e28',
            color: '#ff8389', fontSize: '11px', fontWeight: 600,
            padding: '3px 10px', letterSpacing: '0.3px',
          }}
        >
          <span style={{ width: '6px', height: '6px', borderRadius: '50%', background: '#da1e28' }} />
          1 CRITICAL · INC-1001
        </div>

        {/* Right side — user info + logout */}
        <div style={{ marginLeft: 'auto', display: 'flex', alignItems: 'center', gap: '16px' }}>
          {user && (
            <div style={{ textAlign: 'right' }}>
              <div style={{ color: '#f4f4f4', fontSize: '12px', fontWeight: 500 }}>{user.name}</div>
              <div style={{ color: '#8d8d8d', fontSize: '11px' }}>{user.role}</div>
            </div>
          )}
          <button
            onClick={handleLogout}
            style={{
              background: 'transparent', color: '#8d8d8d', border: '1px solid #393939',
              padding: '6px 12px', fontSize: '12px', cursor: 'pointer', borderRadius: '0',
            }}
          >
            Sign out
          </button>
        </div>
      </header>

      {/* ── Side Navigation ── */}
      <nav
        style={{
          position: 'fixed', top: '48px', left: 0, bottom: 0,
          width: '256px', background: '#262626',
          display: 'flex', flexDirection: 'column',
          zIndex: 90, overflowY: 'auto',
        }}
      >
        {/* Nav section header */}
        <div
          style={{
            padding: '16px 16px 8px 16px',
            fontSize: '11px', color: '#8d8d8d',
            letterSpacing: '0.32px', textTransform: 'uppercase',
          }}
        >
          Modules
        </div>

        {NAV_ITEMS.map((item) => (
          <NavLink
            key={item.path}
            to={item.path}
            style={({ isActive }) => ({
              display: 'flex', alignItems: 'flex-start', gap: '12px',
              padding: '12px 16px', textDecoration: 'none',
              color: isActive ? '#ffffff' : '#c6c6c6',
              background: isActive ? '#393939' : 'transparent',
              borderLeft: `3px solid ${isActive ? '#0f62fe' : 'transparent'}`,
              transition: 'background 70ms, border-color 70ms',
            })}
          >
            <span style={{ fontSize: '16px', marginTop: '1px', flexShrink: 0 }}>{item.icon}</span>
            <div>
              <div style={{ fontSize: '14px', lineHeight: '20px' }}>{item.label}</div>
              <div style={{ fontSize: '11px', color: '#8d8d8d', marginTop: '1px' }}>{item.desc}</div>
            </div>
          </NavLink>
        ))}

        {/* Spacer */}
        <div style={{ flex: 1 }} />

        {/* Bottom: incident quick-access */}
        <div
          style={{
            margin: '16px', background: '#1a1a1a', border: '1px solid #da1e28',
            padding: '12px',
          }}
        >
          <div style={{ fontSize: '11px', color: '#ff8389', fontWeight: 600, letterSpacing: '0.3px', marginBottom: '6px' }}>
            🚨 ACTIVE P1 INCIDENT
          </div>
          <div style={{ fontFamily: 'IBM Plex Mono, monospace', fontSize: '12px', color: '#f4f4f4', marginBottom: '4px' }}>
            INC-1001
          </div>
          <div style={{ fontSize: '11px', color: '#8d8d8d', marginBottom: '10px' }}>
            SLA Buffer: 38 min remaining
          </div>
          <button
            className="carbon-btn-primary"
            style={{ width: '100%', fontSize: '12px', padding: '8px 12px', minHeight: '32px', justifyContent: 'center' }}
            onClick={() => navigate('/impact/INC-1001')}
          >
            Open Dossier
          </button>
        </div>
      </nav>

      {/* ── Main Content Area ── */}
      <main
        style={{
          marginTop: '48px',
          marginLeft: '256px',
          padding: '32px',
          minHeight: 'calc(100vh - 48px)',
          background: '#f4f4f4',
        }}
      >
        <Outlet />
      </main>
    </div>
  )
}

export default AppLayout
