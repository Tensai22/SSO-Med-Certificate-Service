import { Navigate } from 'react-router-dom';

const ProtectedRoute = ({ children, allowedRoles }) => {
    const token = localStorage.getItem('token');
    const userData = localStorage.getItem('user');
    
    if (!token) {
        return <Navigate to="/login" replace />;
    }

    let user = null;
    if (userData) {
        try {
            user = JSON.parse(userData);
        } catch (error) {
            return <Navigate to="/login" replace />;
        }
    } else {
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            user = { roleId: payload.roleId };
        } catch (e) {
            return <Navigate to="/login" replace />;
        }
    }

    if (allowedRoles && !allowedRoles.includes(user.roleId)) {
        return <Navigate to="/" replace />;
    }

    return children;
};

export default ProtectedRoute;
