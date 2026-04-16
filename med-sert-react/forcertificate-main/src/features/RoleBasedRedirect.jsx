import { Navigate } from 'react-router-dom';

const RoleBasedRedirect = () => {
    const userData = localStorage.getItem('user');
    
    if (!userData) {
        return <Navigate to="/login" replace />;
    }

    const user = JSON.parse(userData);

    if (user.roleId === 1) {
        return <Navigate to="/registrar" replace />;
    } else if (user.roleId === 2) {
        return <Navigate to="/sertificate" replace />;
    }

    return <Navigate to="/login" replace />;
};


export default RoleBasedRedirect;