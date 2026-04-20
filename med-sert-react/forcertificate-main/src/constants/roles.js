export const ROLES = {
    REGISTRAR: 1,
    STUDENT: 2,
};

const REGISTRAR_ROLE_NAMES = new Set([
    'office registrar',
    'registrar',
    'регистратор',
    'регистратура',
    'office of registrar',
    'офис регистратора',
]);

const STUDENT_ROLE_NAMES = new Set([
    'student',
    'студент',
    'бакалавриат',
    'бакалавр',
    'undergraduate',
]);

export const normalizeRoleId = (roleId) => {
    if (roleId === null || roleId === undefined) {
        return null;
    }

    const parsed = Number.parseInt(String(roleId), 10);
    return Number.isNaN(parsed) ? null : parsed;
};

export const normalizeRoleName = (roleName) => {
    if (!roleName || typeof roleName !== 'string') {
        return '';
    }

    return roleName.trim().toLowerCase();
};

export const isRegistrarRole = (roleId, roleName) => {
    const normalizedId = normalizeRoleId(roleId);
    if (normalizedId === ROLES.REGISTRAR) {
        return true;
    }

    return REGISTRAR_ROLE_NAMES.has(normalizeRoleName(roleName));
};

export const isStudentRole = (roleId, roleName) => {
    const normalizedId = normalizeRoleId(roleId);
    if (normalizedId === ROLES.STUDENT) {
        return true;
    }

    return STUDENT_ROLE_NAMES.has(normalizeRoleName(roleName));
};
