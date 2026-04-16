// App.js
import './App.css';
import { BrowserRouter as Router, Routes, Route, useLocation } from 'react-router-dom';
import Header from './components/Header';
import MainPage from './pages/MainPage';
import RegistrarPage from './pages/RegistrarPage';
import LoginPage from "./pages/LoginPage";
import ProtectedRoute from './routes/ProtectedRoute';
import { Navigate } from 'react-router-dom';
import RoleBasedRedirect from './routes/RoleBasedRedirect';

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
                    <ProtectedRoute allowedRoles={[2]}>
                        <MainPage />
                    </ProtectedRoute>
                } />

                <Route path="/registrar" element={
                    <ProtectedRoute allowedRoles={[1]}>
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
