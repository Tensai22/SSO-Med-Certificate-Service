import { Navigate } from 'react-router-dom';
import { isRegistrarRole, isStudentRole } from '../constants/roles';
import { getCurrentRoleId, getCurrentRoleName, getToken } from '../utils/auth';

const RoleBasedRedirect = () => {
    const token = getToken();
    if (!token) {
        return <Navigate to="/login" replace />;
    }

    const roleId = getCurrentRoleId();
    const roleName = getCurrentRoleName();

    if (isRegistrarRole(roleId, roleName)) {
        return <Navigate to="/registrar" replace />;
    }

    if (isStudentRole(roleId, roleName)) {
        return <Navigate to="/sertificate" replace />;
    }

    return <Navigate to="/sertificate" replace />;
};

export default RoleBasedRedirect;
