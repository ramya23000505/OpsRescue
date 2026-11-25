import axios from 'axios'

const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

const api = axios.create({
  baseURL: BASE_URL,
  headers: { 'Content-Type': 'application/json' },
})

// Attach JWT token automatically
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('opsrescue_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Auto-logout on 401
api.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err.response?.status === 401) {
      localStorage.removeItem('opsrescue_token')
      localStorage.removeItem('opsrescue_user')
      window.location.href = '/login'
    }
    return Promise.reject(err)
  }
)

// ── Auth
export const authLogin = (email: string, password: string) =>
  api.post('/api/auth/login', { email, password })

export const authRegister = (name: string, email: string, password: string, role: string) =>
  api.post('/api/auth/register', { name, email, password, role })

// ── Module 2: Impact Analysis
export const getImpactAnalysis = (incidentId: string) =>
  api.get(`/api/impact/${incidentId}`)

// ── Module 3: SLA Risk Engine
export const getSlaRisk = (incidentId: string) =>
  api.get(`/api/slarisk/${incidentId}`)

// ── Module 4: Recovery Simulator
export const getRecoverySimulation = (incidentId: string) =>
  api.get(`/api/simulation/${incidentId}`)

// ── Module 8: Control Tower
export const getControlTowerMetrics = () =>
  api.get('/api/controltower/metrics')

export const getIncidents = () =>
  api.get('/api/incidents')

export default api
