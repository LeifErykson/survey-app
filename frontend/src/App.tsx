import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import Login from './pages/Login';
import Register from './pages/Register';
import TakeSurvey from './pages/TakeSurvey';
import SurveyTabs from './components/SurveyTabs';
import './App.css';

const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { user, loading } = useAuth();
  
  if (loading) return <div>Loading...</div>;
  if (!user) return <Navigate to="/login" />;
  return <>{children}</>;
};

function AppRoutes() {
  const { user, logout } = useAuth();
  
  return (
    <div>
      {user && (
        <div style={{ padding: '10px', background: '#f0f0f0', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <span>Welcome, {user.login}!</span>
          <button onClick={logout}>Logout</button>
        </div>
      )}
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/survey/:id" element={
          <ProtectedRoute>
            <TakeSurvey />
          </ProtectedRoute>
        } />
        <Route path="/" element={
          <ProtectedRoute>
            <SurveyTabs />
          </ProtectedRoute>
        } />
      </Routes>
    </div>
  );
}

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <AppRoutes />
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;