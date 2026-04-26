import { Navigate } from 'react-router-dom';
import { isRegistrarRole, isStudentRole } from '../constants/roles';
import { getCurrentRoleId, getCurrentRoleName, getToken } from '../utils/auth';
import StudentDashboardPage from '../pages/StudentDashboardPage';
import RegistrarDashboardPage from '../pages/RegistrarDashboardPage';

const RoleBasedRedirect = () => {
    const token = getToken();
    if (!token) {
        return <Navigate to="/login" replace />;
    }

    const roleId = getCurrentRoleId();
    const roleName = getCurrentRoleName();

    if (isRegistrarRole(roleId, roleName)) {
        return <RegistrarDashboardPage />;
    }

    if (isStudentRole(roleId, roleName)) {
        return <StudentDashboardPage />;
    }

    return <StudentDashboardPage />;
};

export default RoleBasedRedirect;
