import apiClient, { API_BASE_URL } from './apiClient';

export const uploadCertificateFile = async (file) => {
    const formData = new FormData();
    formData.append('File', file);

    const response = await apiClient.post('/File/upload', formData);
    return response.data;
};

export const downloadFileById = async (fileId) => {
    const response = await apiClient.get(`/File/${fileId}`, {
        responseType: 'blob',
    });

    return response.data;
};

export const buildFileUrl = (fileId) => `${API_BASE_URL}/File/${fileId}`;
