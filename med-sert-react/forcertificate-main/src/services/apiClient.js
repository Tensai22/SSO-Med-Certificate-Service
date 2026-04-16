import axios from 'axios';
import { clearAuthStorage, getToken, isTokenExpired } from '../utils/auth';

export const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:5280/api';

const apiClient = axios.create({
    baseURL: API_BASE_URL,
    withCredentials: true,
});

apiClient.interceptors.request.use((config) => {
    const token = getToken();
    if (token && isTokenExpired(token)) {
        clearAuthStorage();
        if (window.location.pathname !== '/login') {
            window.location.replace('/login');
        }
        return config;
    }

    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
});

apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        const status = error.response?.status;
        if (status === 401 || status === 403) {
            clearAuthStorage();
            if (window.location.pathname !== '/login') {
                window.location.replace('/login');
            }
        }

        return Promise.reject(error);
    }
);

export default apiClient;
