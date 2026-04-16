import { Navigate } from 'react-router-dom';
import { clearAuthStorage, getCurrentRoleId, getToken, isTokenExpired } from '../utils/auth';

const ProtectedRoute = ({ children, allowedRoles }) => {
    const token = getToken();
    if (!token) {
        return <Navigate to="/login" replace />;
    }

    if (isTokenExpired(token)) {
        clearAuthStorage();
        return <Navigate to="/login" replace />;
    }

    const roleId = getCurrentRoleId();
    if (!roleId) {
        clearAuthStorage();
        return <Navigate to="/login" replace />;
    }

    if (allowedRoles && !allowedRoles.includes(roleId)) {
        return <Navigate to="/" replace />;
    }

    return children;
};

export default ProtectedRoute;
