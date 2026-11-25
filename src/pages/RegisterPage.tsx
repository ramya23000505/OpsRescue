// =============================================
// Register Page — IBM Carbon Design
// =============================================
import React, { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

const ROLES = [
  { value: 'operations_manager', label: 'Operations Manager' },
  { value: 'logistics_analyst', label: 'Logistics Analyst' },
  { value: 'supply_chain_vp', label: 'VP Supply Chain' },
  { value: 'admin', label: 'System Administrator' },
]

const RegisterPage: React.FC = () => {
  const { register } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState({ name: '', email: '', password: '', confirm: '', role: '' })
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const set = (k: string, v: string) => setForm((f) => ({ ...f, [k]: v }))

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError(null)
    if (form.password !== form.confirm) {
      setError('Passwords do not match.')
      return
    }
    if (form.password.length < 8) {
      setError('Password must be at least 8 characters.')
      return
    }
    setLoading(true)
    try {
      await register(form.name, form.email, form.password, form.role)
      navigate('/control-tower')
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })
        ?.response?.data?.message
      setError(msg || 'Registration failed. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  const Field = ({ label, type = 'text', k, placeholder, autoComplete }: {
    label: string; type?: string; k: string; placeholder?: string; autoComplete?: string
  }) => (
    <div>
      <label className="carbon-label" style={{ color: '#c6c6c6' }}>{label}</label>
      <input
        type={type}
        value={form[k as keyof typeof form]}
        onChange={(e) => set(k, e.target.value)}
        placeholder={placeholder}
        required
        autoComplete={autoComplete}
        className="carbon-input"
        style={{ background: '#262626', color: '#f4f4f4', borderBottomColor: '#6f6f6f' }}
      />
    </div>
  )

  return (
    <div className="min-h-screen flex items-center justify-center p-8" style={{ background: '#161616' }}>
      <div className="w-full" style={{ maxWidth: '480px' }}>
        {/* Logo */}
        <div className="flex items-center gap-2 mb-10">
          <div className="w-8 h-8 flex items-center justify-center" style={{ background: '#0f62fe' }}>
            <span className="text-white font-bold text-sm">OR</span>
          </div>
          <span className="text-white font-semibold text-lg">OpsRescue</span>
        </div>

        <div
          style={{ background: '#262626', border: '1px solid #393939', padding: '32px' }}
        >
          <h2 className="text-white font-light mb-1" style={{ fontSize: '28px', lineHeight: '36px' }}>Create account</h2>
          <p style={{ color: '#8d8d8d', fontSize: '14px', marginBottom: '24px' }}>
            Request access to the OpsRescue platform
          </p>

          {error && (
            <div className="carbon-notification-error mb-6" style={{ background: '#2d1515', borderLeftColor: '#da1e28', color: '#ffb3b8' }}>
              <svg width="20" height="20" viewBox="0 0 20 20" fill="#da1e28" style={{ flexShrink: 0 }}>
                <path d="M10 1a9 9 0 1 0 0 18A9 9 0 0 0 10 1zm-.75 4h1.5v6h-1.5V5zm0 7h1.5v1.5h-1.5V12z" />
              </svg>
              <span style={{ fontSize: '14px' }}>{error}</span>
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-5">
            <Field label="Full name" k="name" placeholder="Lavanya Krishnan" autoComplete="name" />
            <Field label="Email address" type="email" k="email" placeholder="user@company.com" autoComplete="email" />

            {/* Role select */}
            <div>
              <label className="carbon-label" style={{ color: '#c6c6c6' }}>Role</label>
              <select
                value={form.role}
                onChange={(e) => set('role', e.target.value)}
                required
                style={{
                  width: '100%',
                  background: '#262626',
                  color: form.role ? '#f4f4f4' : '#a8a8a8',
                  border: 'none',
                  borderBottom: '1px solid #6f6f6f',
                  borderRadius: '0',
                  padding: '11px 16px',
                  fontSize: '14px',
                  appearance: 'none',
                  cursor: 'pointer',
                }}
              >
                <option value="" disabled>Select your role</option>
                {ROLES.map((r) => (
                  <option key={r.value} value={r.value}>{r.label}</option>
                ))}
              </select>
            </div>

            <Field label="Password" type="password" k="password" placeholder="Min. 8 characters" autoComplete="new-password" />
            <Field label="Confirm password" type="password" k="confirm" placeholder="Repeat password" autoComplete="new-password" />

            <div className="pt-2">
              <button
                type="submit"
                disabled={loading}
                className="carbon-btn-primary"
                style={{ width: '100%', justifyContent: 'space-between' }}
              >
                <span>{loading ? 'Creating account...' : 'Create account'}</span>
                {!loading && (
                  <svg width="20" height="20" viewBox="0 0 20 20" fill="currentColor">
                    <path d="M12 10l-6 6 1.4 1.4L14.8 10 7.4 2.6 6 4z" />
                  </svg>
                )}
              </button>
            </div>
          </form>
        </div>

        <div className="mt-6">
          <p style={{ color: '#8d8d8d', fontSize: '14px' }}>
            Already have an account?{' '}
            <Link to="/login" style={{ color: '#78a9ff', textDecoration: 'none' }}>
              Sign in
            </Link>
          </p>
        </div>
      </div>
    </div>
  )
}

export default RegisterPage
