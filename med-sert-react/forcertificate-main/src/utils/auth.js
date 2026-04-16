const TOKEN_KEY = 'token';
const USER_KEY = 'user';
const USER_ID_KEY = 'userId';

const decodeBase64Url = (value) => {
    const normalized = value.replace(/-/g, '+').replace(/_/g, '/');
    const pad = normalized.length % 4;
    const padded = normalized + (pad ? '='.repeat(4 - pad) : '');
    return atob(padded);
};

export const parseTokenPayload = (token) => {
    if (!token) {
        return null;
    }

    try {
        const parts = token.split('.');
        if (parts.length < 2) {
            return null;
        }

        return JSON.parse(decodeBase64Url(parts[1]));
    } catch {
        return null;
    }
};

export const getToken = () => localStorage.getItem(TOKEN_KEY);

export const isTokenExpired = (token) => {
    const payload = parseTokenPayload(token);
    if (!payload?.exp) {
        return false;
    }

    const nowSeconds = Math.floor(Date.now() / 1000);
    return payload.exp <= nowSeconds;
};

export const getStoredUser = () => {
    const rawUser = localStorage.getItem(USER_KEY);
    if (!rawUser) {
        return null;
    }

    try {
        return JSON.parse(rawUser);
    } catch {
        return null;
    }
};

export const clearAuthStorage = () => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    localStorage.removeItem(USER_ID_KEY);
};

export const getCurrentUserId = () => {
    const fromStorage = localStorage.getItem(USER_ID_KEY);
    if (fromStorage) {
        const parsed = Number.parseInt(fromStorage, 10);
        if (!Number.isNaN(parsed)) {
            return parsed;
        }
    }

    const payload = parseTokenPayload(getToken());
    const rawId = payload?.sub
        ?? payload?.nameid
        ?? payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];

    const parsed = Number.parseInt(rawId, 10);
    return Number.isNaN(parsed) ? null : parsed;
};

export const getCurrentRoleId = () => {
    const user = getStoredUser();
    if (user?.roleId) {
        return user.roleId;
    }

    const payload = parseTokenPayload(getToken());
    const roleId = payload?.roleId;
    if (roleId) {
        const parsed = Number.parseInt(roleId, 10);
        return Number.isNaN(parsed) ? null : parsed;
    }

    return null;
};

export const saveAuthData = ({ token, userId, user }) => {
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(USER_ID_KEY, String(userId));
    localStorage.setItem(USER_KEY, JSON.stringify(user));
};
