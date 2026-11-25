// =============================================
// Auth Context — OpsRescue
// =============================================
import React, { createContext, useContext, useState } from 'react'
import { authLogin, authRegister } from '../services/api'

interface User {
  id: string
  name: string
  email: string
  role: string
}

interface AuthContextValue {
  user: User | null
  token: string | null
  login: (email: string, password: string) => Promise<void>
  register: (name: string, email: string, password: string, role: string) => Promise<void>
  logout: () => void
  isAuthenticated: boolean
}

const AuthContext = createContext<AuthContextValue | null>(null)

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(() => {
    try {
      const stored = localStorage.getItem('opsrescue_user')
      return stored ? JSON.parse(stored) : null
    } catch { return null }
  })
  const [token, setToken] = useState<string | null>(() =>
    localStorage.getItem('opsrescue_token')
  )

  const login = async (email: string, password: string) => {
    const res = await authLogin(email, password)
    const { token: t, user: u } = res.data
    setToken(t)
    setUser(u)
    localStorage.setItem('opsrescue_token', t)
    localStorage.setItem('opsrescue_user', JSON.stringify(u))
  }

  const register = async (name: string, email: string, password: string, role: string) => {
    const res = await authRegister(name, email, password, role)
    const { token: t, user: u } = res.data
    setToken(t)
    setUser(u)
    localStorage.setItem('opsrescue_token', t)
    localStorage.setItem('opsrescue_user', JSON.stringify(u))
  }

  const logout = () => {
    setToken(null)
    setUser(null)
    localStorage.removeItem('opsrescue_token')
    localStorage.removeItem('opsrescue_user')
  }

  return (
    <AuthContext.Provider value={{ user, token, login, register, logout, isAuthenticated: !!token }}>
      {children}
    </AuthContext.Provider>
  )
}

export const useAuth = () => {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used inside AuthProvider')
  return ctx
}
