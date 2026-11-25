// =============================================
// Login Page — IBM Carbon Design
// =============================================
import React, { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

const LoginPage: React.FC = () => {
  const { login } = useAuth()
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError(null)
    setLoading(true)
    try {
      await login(email, password)
      navigate('/control-tower')
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })
        ?.response?.data?.message
      setError(msg || 'Invalid credentials. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div
      className="min-h-screen flex"
      style={{ background: '#161616' }}
    >
      {/* Left panel — branding */}
      <div
        className="hidden lg:flex flex-col justify-between w-1/2 p-12"
        style={{ background: '#262626', borderRight: '1px solid #393939' }}
      >
        <div>
          {/* Logo */}
          <div className="flex items-center gap-3 mb-16">
            <div
              className="w-8 h-8 flex items-center justify-center"
              style={{ background: '#0f62fe' }}
            >
              <span className="text-white font-bold text-sm">OR</span>
            </div>
            <span className="text-white font-semibold text-lg tracking-wide">OpsRescue</span>
          </div>

          <h1 className="text-white font-light mb-4" style={{ fontSize: '42px', lineHeight: '50px' }}>
            Supply Chain<br />Intelligence Platform
          </h1>
          <p style={{ color: '#8d8d8d', fontSize: '16px', lineHeight: '24px', maxWidth: '400px' }}>
            Real-time incident detection, blast-radius analysis, and AI-powered recovery simulation for mission-critical logistics.
          </p>
        </div>

        {/* Stats */}
        <div className="grid grid-cols-3 gap-6">
          {[
            { label: 'Active Incidents', value: '3' },
            { label: 'Revenue Protected', value: '₹1.84Cr' },
            { label: 'SLA Compliance', value: '98.2%' },
          ].map((s) => (
            <div key={s.label} style={{ borderTop: '1px solid #393939', paddingTop: '16px' }}>
              <div className="text-white font-semibold" style={{ fontSize: '28px' }}>{s.value}</div>
              <div style={{ color: '#8d8d8d', fontSize: '12px', letterSpacing: '0.32px' }}>{s.label}</div>
            </div>
          ))}
        </div>
      </div>

      {/* Right panel — form */}
      <div className="flex-1 flex items-center justify-center p-8" style={{ background: '#161616' }}>
        <div className="w-full" style={{ maxWidth: '400px' }}>
          {/* Mobile logo */}
          <div className="flex items-center gap-2 mb-10 lg:hidden">
            <div className="w-8 h-8 flex items-center justify-center" style={{ background: '#0f62fe' }}>
              <span className="text-white font-bold text-sm">OR</span>
            </div>
            <span className="text-white font-semibold text-lg">OpsRescue</span>
          </div>

          <div className="mb-8">
            <h2 className="text-white font-light mb-1" style={{ fontSize: '28px', lineHeight: '36px' }}>Sign in</h2>
            <p style={{ color: '#8d8d8d', fontSize: '14px' }}>Use your OpsRescue credentials</p>
          </div>

          {error && (
            <div className="carbon-notification-error mb-6" style={{ background: '#2d1515', borderLeftColor: '#da1e28', color: '#ffb3b8' }}>
              <svg width="20" height="20" viewBox="0 0 20 20" fill="#da1e28" style={{ flexShrink: 0 }}>
                <path d="M10 1a9 9 0 1 0 0 18A9 9 0 0 0 10 1zm-.75 4h1.5v6h-1.5V5zm0 7h1.5v1.5h-1.5V12z" />
              </svg>
              <span style={{ fontSize: '14px' }}>{error}</span>
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-6">
            <div>
              <label className="carbon-label" style={{ color: '#c6c6c6' }}>Email address</label>
              <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="user@company.com"
                required
                autoComplete="email"
                className="carbon-input"
                style={{ background: '#262626', color: '#f4f4f4', borderBottomColor: '#6f6f6f' }}
              />
            </div>

            <div>
              <label className="carbon-label" style={{ color: '#c6c6c6' }}>Password</label>
              <input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="········"
                required
                autoComplete="current-password"
                className="carbon-input"
                style={{ background: '#262626', color: '#f4f4f4', borderBottomColor: '#6f6f6f' }}
              />
            </div>

            <button
              type="submit"
              disabled={loading}
              className="carbon-btn-primary w-full"
              style={{ width: '100%', justifyContent: 'space-between' }}
            >
              <span>{loading ? 'Signing in...' : 'Sign in'}</span>
              {!loading && (
                <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
                  <path d="M12 10l-6 6 1.4 1.4L14.8 10 7.4 2.6 6 4z" />
                </svg>
              )}
            </button>
          </form>

          <div className="mt-8 pt-8" style={{ borderTop: '1px solid #393939' }}>
            <p style={{ color: '#8d8d8d', fontSize: '14px' }}>
              Don't have an account?{' '}
              <Link to="/register" style={{ color: '#78a9ff', textDecoration: 'none' }}>
                Create account
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  )
}

export default LoginPage
