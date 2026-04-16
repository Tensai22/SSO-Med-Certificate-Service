import apiClient from './apiClient';

export const login = async ({ email, password }) => {
    const response = await apiClient.post('/Auth/login', {
        email,
        password,
    });

    return response.data;
};
