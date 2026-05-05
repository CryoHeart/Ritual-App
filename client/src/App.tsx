import { useState } from 'react';
import { AuthProvider, useAuth } from './context/AuthContext';
import { DashboardPage } from './pages/DashboardPage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';

function AppRoutes() {
  const { user } = useAuth();
  const [mode, setMode] = useState<'login' | 'register'>('login');

  if (user) {
    return <DashboardPage />;
  }

  return mode === 'login' ? (
    <LoginPage onSwitchToRegister={() => setMode('register')} />
  ) : (
    <RegisterPage onSwitchToLogin={() => setMode('login')} />
  );
}

function App() {
  return (
    <AuthProvider>
      <AppRoutes />
    </AuthProvider>
  );
}

export default App;
