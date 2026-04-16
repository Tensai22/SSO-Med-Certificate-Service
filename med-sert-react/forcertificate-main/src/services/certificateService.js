import apiClient from './apiClient';

export const fetchAllCertificates = async () => {
    const response = await apiClient.get('/Certificate');
    return response.data;
};

export const fetchUserCertificates = async (userId) => {
    const response = await apiClient.get(`/Certificate/user/${userId}`);
    return response.data;
};

export const createCertificate = async (certificateRequest) => {
    const response = await apiClient.post('/Certificate', certificateRequest);
    return response.data;
};

export const approveCertificate = async (certificateId, payload) => {
    const response = await apiClient.post(`/Certificate/${certificateId}/approve`, payload);
    return response.data;
};

export const rejectCertificate = async (certificateId, payload) => {
    const response = await apiClient.post(`/Certificate/${certificateId}/reject`, payload);
    return response.data;
};
