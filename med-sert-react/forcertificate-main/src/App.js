import './App.css';
import { BrowserRouter as Router, Routes, Route, useLocation } from 'react-router-dom';

import Header from './components/Header';
import MainPage from './pages/MainPage';
import RegistrarPage from './pages/RegistrarPage';
import LoginPage from "./pages/LoginPage";

function AppContent() {
    const location = useLocation();
    const isLoginPage = location.pathname === "/login";

    return (
        <div className="container">
            {!isLoginPage && <Header />}
            <Routes>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/" element={<MainPage />} />
                <Route path="/registrar" element={<RegistrarPage />} />
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
