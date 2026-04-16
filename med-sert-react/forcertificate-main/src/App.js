// App.js
import './App.css';
import { BrowserRouter as Router, Routes, Route, useLocation } from 'react-router-dom';
import Header from './components/Header';
import MainPage from './pages/MainPage';
import RegistrarPage from './pages/RegistrarPage';
import LoginPage from "./pages/LoginPage";
import ProtectedRoute from './components/ProtectedRoute';
import { Navigate } from 'react-router-dom';
import RoleBasedRedirect from './features/RoleBasedRedirect';

function AppContent() {
    const location = useLocation();
    const isLoginPage = location.pathname === "/login";

    return (
        <div className="container">
            {!isLoginPage && <Header />}
            <Routes>
                <Route path="/login" element={<LoginPage />} />
                
                {/* 1. Главная страница теперь перенаправляет по ролям */}
                <Route path="/" element={<RoleBasedRedirect />} />

                {/* 2. Страница для студента (Роль 2) */}
                <Route path="/sertificate" element={
                    <ProtectedRoute allowedRoles={[2]}>
                        <MainPage />
                    </ProtectedRoute>
                } />
                
                {/* 3. Страница для регистратора (Роль 1) */}
                <Route path="/registrar" element={
                    <ProtectedRoute allowedRoles={[1]}> 
                        <RegistrarPage />
                    </ProtectedRoute>
                } />

                {/* Обработка несуществующих страниц (404) */}
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