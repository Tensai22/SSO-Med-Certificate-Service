import { Navigate } from 'react-router-dom';
import { ROLES, isRegistrarRole, isStudentRole } from '../constants/roles';
import { getCurrentRoleId, getCurrentRoleName, getToken } from '../utils/auth';

const ProtectedRoute = ({ children, allowedRoles }) => {
    const token = getToken();
    if (!token) {
        return <Navigate to="/login" replace />;
    }

    const roleId = getCurrentRoleId();
    const roleName = getCurrentRoleName();

    if (!roleId && !roleName) {
        return <Navigate to="/login" replace />;
    }

    if (allowedRoles?.length) {
        const canAccessRegistrar = allowedRoles.includes(ROLES.REGISTRAR) && isRegistrarRole(roleId, roleName);
        const canAccessStudent = allowedRoles.includes(ROLES.STUDENT) && isStudentRole(roleId, roleName);

        if (!canAccessRegistrar && !canAccessStudent) {
            return <Navigate to="/" replace />;
        }
    }

    return children;
};

export default ProtectedRoute;
