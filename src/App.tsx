// =============================================
// App Router — OpsRescue Frontend
// =============================================
import React from 'react'
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { AuthProvider } from './context/AuthContext'
import AppLayout from './components/AppLayout'
import ProtectedRoute from './components/ProtectedRoute'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import ControlTowerPage from './pages/ControlTowerPage'
import ImpactAnalysisPage from './pages/ImpactAnalysisPage'
import SlaRiskPage from './pages/SlaRiskPage'
import RecoverySimulatorPage from './pages/RecoverySimulatorPage'

const App: React.FC = () => {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          {/* Public routes */}
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          {/* Protected routes — wrapped in AppLayout shell */}
          <Route
            element={
              <ProtectedRoute>
                <AppLayout />
              </ProtectedRoute>
            }
          >
            {/* Default redirect */}
            <Route index element={<Navigate to="/control-tower" replace />} />

            {/* Module 8 — Control Tower */}
            <Route path="control-tower" element={<ControlTowerPage />} />

            {/* Module 2 — Impact Analysis */}
            <Route path="impact/:id" element={<ImpactAnalysisPage />} />

            {/* Module 3 — SLA Risk Engine */}
            <Route path="slarisk/:id" element={<SlaRiskPage />} />

            {/* Module 4 — Recovery Simulator */}
            <Route path="simulation/:id" element={<RecoverySimulatorPage />} />
          </Route>

          {/* Catch-all */}
          <Route path="*" element={<Navigate to="/control-tower" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}

export default App
