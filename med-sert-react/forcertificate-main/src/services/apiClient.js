import axios from 'axios';
import { clearAuthStorage, getToken, isTokenExpired } from '../utils/auth';

export const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:5280/api';

const apiClient = axios.create({
    baseURL: API_BASE_URL,
    withCredentials: true,
});

const redirectToLogin = () => {
    if (typeof window === 'undefined') {
        return;
    }

    if (window.location.pathname !== '/login') {
        window.location.replace('/login');
    }
};

apiClient.interceptors.request.use((config) => {
    const token = getToken();
    if (token) {
        if (isTokenExpired(token)) {
            clearAuthStorage();
            redirectToLogin();
            return Promise.reject(new axios.Cancel('JWT token expired'));
        }

        config.headers = config.headers ?? {};
        config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
});

apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error?.response?.status === 401) {
            clearAuthStorage();
            redirectToLogin();
        }

        return Promise.reject(error);
    }
);

export default apiClient;
