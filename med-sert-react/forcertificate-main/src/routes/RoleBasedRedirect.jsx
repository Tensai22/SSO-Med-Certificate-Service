import { Navigate } from 'react-router-dom';

const RoleBasedRedirect = () => {
    const userData = localStorage.getItem('user');
    
    if (!userData) {
        return <Navigate to="/login" replace />;
    }

    let user = null;

    try {
        user = JSON.parse(userData);
    } catch (error) {
        return <Navigate to="/login" replace />;
    }

    if (user.roleId === 1) {
        return <Navigate to="/registrar" replace />;
    } else if (user.roleId === 2) {
        return <Navigate to="/sertificate" replace />;
    }

    return <Navigate to="/login" replace />;
};


export default RoleBasedRedirect;
