import { Navigate } from 'react-router-dom';
import { ROLES } from '../constants/roles';
import { clearAuthStorage, getCurrentRoleId, getToken, isTokenExpired } from '../utils/auth';

const RoleBasedRedirect = () => {
    const token = getToken();
    if (!token) {
        return <Navigate to="/login" replace />;
    }

    if (isTokenExpired(token)) {
        clearAuthStorage();
        return <Navigate to="/login" replace />;
    }

    const roleId = getCurrentRoleId();
    if (roleId === ROLES.REGISTRAR) {
        return <Navigate to="/registrar" replace />;
    }

    if (roleId === ROLES.STUDENT) {
        return <Navigate to="/sertificate" replace />;
    }

    clearAuthStorage();
    return <Navigate to="/login" replace />;
};

export default RoleBasedRedirect;
