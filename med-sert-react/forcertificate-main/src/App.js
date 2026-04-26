// App.js
import './App.css';
import { BrowserRouter as Router, Routes, Route, useLocation } from 'react-router-dom';
import Header from './components/Header';
import MainPage from './pages/MainPage';
import RegistrarPage from './pages/RegistrarPage';
import StudentDashboardPage from './pages/StudentDashboardPage';
import RegistrarDashboardPage from './pages/RegistrarDashboardPage';
import LoginPage from "./pages/LoginPage";
import ProtectedRoute from './routes/ProtectedRoute';
import { Navigate } from 'react-router-dom';
import RoleBasedRedirect from './routes/RoleBasedRedirect';
import { ROLES } from './constants/roles';

function AppContent() {
    const location = useLocation();
    const isLoginPage = location.pathname === "/login";

    return (
        <div className="container">
            {!isLoginPage && <Header />}
            <Routes>
                <Route path="/login" element={<LoginPage />} />

                <Route path="/" element={<RoleBasedRedirect />} />

                <Route path="/sertificate" element={
                    <ProtectedRoute allowedRoles={[ROLES.STUDENT]}>
                        <StudentDashboardPage />
                    </ProtectedRoute>
                } />

                <Route path="/my-sertificates" element={
                    <ProtectedRoute allowedRoles={[ROLES.STUDENT]}>
                        <MainPage />
                    </ProtectedRoute>
                } />

                <Route path="/registrar" element={
                    <ProtectedRoute allowedRoles={[ROLES.REGISTRAR]}>
                        <RegistrarDashboardPage />
                    </ProtectedRoute>
                } />

                <Route path="/incoming-sertificates" element={
                    <ProtectedRoute allowedRoles={[ROLES.REGISTRAR]}>
                        <RegistrarPage />
                    </ProtectedRoute>
                } />

                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </div>
    );
}

function App() {
    return (
        <Router>
            <AppContent />
        </Router>
    );
}

export default App;
