import React, { useCallback, useEffect, useMemo, useState } from 'react';
import '../css/RegistrarPage.css';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import {
    approveCertificate,
    fetchCertificateFilters,
    fetchAllCertificates,
    rejectCertificate,
} from '../services/certificateService';
import { buildFileUrl, downloadFileById } from '../services/fileService';

const PAGE_SIZE = 30;
const INITIAL_SEARCH_FILTERS = {
    fullName: '',
    iin: '',
    department: '',
    institute: '',
    clinic: '',
    studentComment: '',
    registrarComment: '',
};

const getTextValue = (source, ...keys) => {
    for (const key of keys) {
        const value = source?.[key];
        if (typeof value === 'string' && value.trim()) {
            return value.trim();
        }
    }

    return '';
};

const getOrgUnitTitle = (unit) => getTextValue(unit, 'title', 'Title');
const getOrgUnitTypeTitle = (unit) => getTextValue(unit, 'typeTitle', 'TypeTitle');
const getOrgUnitParentId = (unit) => unit?.parentId ?? unit?.ParentID ?? null;
const normalizeText = (value) => (typeof value === 'string' ? value.trim().toLowerCase() : '');

const stripLeadingLabel = (value, labels) => {
    const text = typeof value === 'string' ? value.trim() : '';
    if (!text) {
        return '';
    }

    const parts = text.split(/\s+/);
    if (parts.length <= 1) {
        return text;
    }

    const [firstWord, ...rest] = parts;
    const matchesLabel = labels.some((label) => firstWord.toLowerCase() === label.toLowerCase());

    return matchesLabel ? rest.join(' ') : text;
};

const RegistrarPage = () => {
    const [activeTab, setActiveTab] = useState('vkhodyashie');
    const [certificates, setCertificates] = useState([]);
    const [filterOptions, setFilterOptions] = useState({
        orgUnits: [],
        departments: [],
        institutes: [],
    });
    const [selectedCert, setSelectedCert] = useState(null);
    const [registrarComment, setRegistrarComment] = useState('');
    const [searchFilters, setSearchFilters] = useState(INITIAL_SEARCH_FILTERS);
    const [statusFilter, setStatusFilter] = useState('all');
    const [currentPage, setCurrentPage] = useState(1);
    const [activeDropdown, setActiveDropdown] = useState(null);
    const [previewUrl, setPreviewUrl] = useState('');
    const [previewType, setPreviewType] = useState('');
    const [previewError, setPreviewError] = useState('');
    const [isPreviewLoading, setIsPreviewLoading] = useState(false);

    const fetchCertificates = useCallback(async () => {
        try {
            const allData = await fetchAllCertificates();

            if (activeTab === 'vkhodyashie') {
                setCertificates(allData.filter((cert) => cert.statusId === 1));
                return;
            }

            setCertificates(allData.filter((cert) => cert.statusId !== 1));
        } catch (error) {
            console.error('Ошибка при загрузке данных:', error);
        }
    }, [activeTab]);

    const fetchFilterOptions = useCallback(async () => {
        try {
            const data = await fetchCertificateFilters();
            setFilterOptions({
                orgUnits: Array.isArray(data?.orgUnits) ? data.orgUnits : [],
                departments: Array.isArray(data?.departments) ? data.departments : [],
                institutes: Array.isArray(data?.institutes) ? data.institutes : [],
            });
        } catch (error) {
            console.error('Ошибка при загрузке фильтров:', error);
        }
    }, []);

    useEffect(() => {
        fetchCertificates();
    }, [fetchCertificates]);

    useEffect(() => {
        fetchFilterOptions();
    }, [fetchFilterOptions]);

    useEffect(() => {
        let objectUrl;

        const loadFilePreview = async () => {
            if (!selectedCert?.filePathId) {
                setPreviewUrl('');
                setPreviewType('');
                setPreviewError('Для этой справки файл не найден');
                return;
            }

            setIsPreviewLoading(true);
            setPreviewError('');
            setPreviewType('');
            setPreviewUrl('');

            try {
                const fileBlob = await downloadFileById(selectedCert.filePathId);
                objectUrl = URL.createObjectURL(fileBlob);
                setPreviewUrl(objectUrl);
                setPreviewType(fileBlob.type?.toLowerCase().includes('pdf') ? 'pdf' : 'image');
            } catch (error) {
                console.error('Ошибка при загрузке файла справки:', error);
                setPreviewError('Не удалось открыть файл справки');
            } finally {
                setIsPreviewLoading(false);
            }
        };

        if (selectedCert) {
            loadFilePreview();
        }

        return () => {
            if (objectUrl) {
                URL.revokeObjectURL(objectUrl);
            }
        };
    }, [selectedCert]);

    const handleAction = async (id, action) => {
        try {
            if (action === 'approve') {
                await approveCertificate(id);
                toast.success('Справка подтверждена');
            } else {
                await rejectCertificate(id, {
                    comment: registrarComment || 'Не соответствует требованиям',
                });
                toast.success('Справка отклонена');
            }

            closeModal();
            fetchCertificates();
        } catch (error) {
            console.error('Ошибка при обработке:', error);
            toast.error('Не удалось выполнить действие');
        }
    };

    const formatDate = (dateString) => {
        if (!dateString) return '';
        const date = new Date(dateString);

        return date.toLocaleDateString('ru-RU');
    };

    const openModal = (cert) => {
        setSelectedCert(cert);
        setRegistrarComment(cert.reviewerComment || '');
    };

    const closeModal = () => {
        setSelectedCert(null);
        setPreviewUrl('');
        setPreviewType('');
        setPreviewError('');
        setIsPreviewLoading(false);
    };

    const handleTabChange = (tab) => {
        setActiveTab(tab);
        setSearchFilters({ ...INITIAL_SEARCH_FILTERS });
        setStatusFilter('all');
        setCurrentPage(1);
    };

    const filteredCertificates = useMemo(() => {
        return certificates.filter((cert) => {
            const isIncomingTab = activeTab === 'vkhodyashie';
            const matchesStatus = isIncomingTab || statusFilter === 'all' || String(cert.statusId) === statusFilter;
            if (!matchesStatus) {
                return false;
            }

            const fullName = getTextValue(cert.user, 'fullName', 'FullName', 'userName', 'UserName', 'name', 'Name').toLowerCase();
            const iin = getTextValue(cert.user, 'iin', 'IIN').toLowerCase();
            const department = getTextValue(cert.user, 'department', 'Department').toLowerCase();
            const institute = getTextValue(cert.user, 'institute', 'Institute').toLowerCase();
            const clinic = (cert.clinic || '').toLowerCase();
            const studentComment = (cert.comment || '').toLowerCase();
            const reviewerComment = (cert.reviewerComment || '').toLowerCase();

            const normalizedFullName = searchFilters.fullName.trim().toLowerCase();
            const normalizedIin = searchFilters.iin.trim().toLowerCase();
            const normalizedDepartment = searchFilters.department.trim().toLowerCase();
            const normalizedInstitute = searchFilters.institute.trim().toLowerCase();
            const normalizedClinic = searchFilters.clinic.trim().toLowerCase();
            const normalizedStudentComment = searchFilters.studentComment.trim().toLowerCase();
            const normalizedRegistrarComment = searchFilters.registrarComment.trim().toLowerCase();

            const matchesFullName = !normalizedFullName || fullName.includes(normalizedFullName);
            const matchesIin = !normalizedIin || iin.includes(normalizedIin);
            const matchesDepartment = !normalizedDepartment || department.includes(normalizedDepartment);
            const matchesInstitute = !normalizedInstitute || institute.includes(normalizedInstitute);
            const matchesClinic = !normalizedClinic || clinic.includes(normalizedClinic);
            const matchesStudentComment = !normalizedStudentComment || studentComment.includes(normalizedStudentComment);
            const matchesRegistrarComment = !normalizedRegistrarComment || reviewerComment.includes(normalizedRegistrarComment);

            return matchesFullName
                && matchesIin
                && matchesDepartment
                && matchesInstitute
                && matchesClinic
                && matchesStudentComment
                && matchesRegistrarComment;
        });
    }, [certificates, searchFilters, statusFilter, activeTab]);

    const totalPages = Math.max(1, Math.ceil(filteredCertificates.length / PAGE_SIZE));

    const paginatedCertificates = useMemo(() => {
        const startIndex = (currentPage - 1) * PAGE_SIZE;
        return filteredCertificates.slice(startIndex, startIndex + PAGE_SIZE);
    }, [filteredCertificates, currentPage]);

    const eduOrgUnits = useMemo(() => {
        return filterOptions.orgUnits;
    }, [filterOptions.orgUnits]);

    const instituteUnits = useMemo(() => eduOrgUnits.filter((unit) => {
        const typeTitle = normalizeText(getOrgUnitTypeTitle(unit));
        return typeTitle.includes('институт') || typeTitle.includes('instit');
    }), [eduOrgUnits]);

    const departmentUnits = useMemo(() => eduOrgUnits.filter((unit) => {
        const typeTitle = normalizeText(getOrgUnitTypeTitle(unit));
        return typeTitle.includes('кафед') || typeTitle.includes('depart') || typeTitle.includes('chair');
    }), [eduOrgUnits]);

    const selectedInstituteUnit = useMemo(() => {
        const query = normalizeText(searchFilters.institute);
        if (!query) {
            return null;
        }

        return instituteUnits.find((unit) => normalizeText(getOrgUnitTitle(unit)) === query) ?? null;
    }, [instituteUnits, searchFilters.institute]);

    const instituteSuggestions = useMemo(() => {
        const query = normalizeText(searchFilters.institute);
        const source = query
            ? instituteUnits.filter((unit) => normalizeText(getOrgUnitTitle(unit)).includes(query))
            : instituteUnits;

        return source
            .map(getOrgUnitTitle)
            .filter(Boolean);
    }, [instituteUnits, searchFilters.institute]);

    const departmentSuggestions = useMemo(() => {
        const query = normalizeText(searchFilters.department);
        const source = selectedInstituteUnit
            ? departmentUnits.filter((unit) => String(getOrgUnitParentId(unit)) === String(selectedInstituteUnit.id ?? selectedInstituteUnit.ID))
            : departmentUnits;

        return source
            .filter((unit) => !query || normalizeText(getOrgUnitTitle(unit)).includes(query))
            .map(getOrgUnitTitle)
            .filter(Boolean);
    }, [departmentUnits, searchFilters.department, selectedInstituteUnit]);

    useEffect(() => {
        setCurrentPage(1);
    }, [searchFilters, statusFilter, activeTab]);

    const updateSearchFilter = (field, value) => {
        setSearchFilters((prev) => ({
            ...prev,
            [field]: value,
        }));
    };

    const selectSuggestion = (field, value) => {
        if (field === 'institute') {
            updateSearchFilter('department', '');
        }

        updateSearchFilter(field, value);
        setActiveDropdown(null);
    };

    useEffect(() => {
        if (currentPage > totalPages) {
            setCurrentPage(totalPages);
        }
    }, [currentPage, totalPages]);

    return (
        <div className="page-container">
            <aside className="sidebar">
                <button
                    className={activeTab === 'vkhodyashie' ? 'active' : ''}
                    onClick={() => handleTabChange('vkhodyashie')}
                >
                    Входящие данные
                </button>
                <button
                    className={activeTab === 'istoriya' ? 'active' : ''}
                    onClick={() => handleTabChange('istoriya')}
                >
                    История
                </button>
            </aside>
            <ToastContainer 
                position="top-right"
                autoClose={4000}
                hideProgressBar={false}
                newestOnTop={false}
                closeOnClick
                rtl={false}
                pauseOnFocusLoss
                draggable
                pauseOnHover
                theme="light"
            />

            <main className="main-content">
                <div className="form-card">
                    <h2 className="form-title">
                        {activeTab === 'vkhodyashie' ? 'Новые справки на проверку' : 'История обработанных справок'}
                    </h2>

                    <div className="registrar-filters">
                        <input
                            className="registrar-filter-input"
                            type="text"
                            placeholder="ФИО"
                            value={searchFilters.fullName}
                            onChange={(e) => updateSearchFilter('fullName', e.target.value)}
                        />
                        <input
                            className="registrar-filter-input"
                            type="text"
                            placeholder="ИИН"
                            value={searchFilters.iin}
                            onChange={(e) => updateSearchFilter('iin', e.target.value)}
                        />
                        <div className="registrar-filter-group">
                            <input
                                className="registrar-filter-input"
                                type="text"
                                placeholder="Институт"
                                value={searchFilters.institute}
                                onChange={(e) => {
                                    updateSearchFilter('institute', e.target.value);
                                    setActiveDropdown('institute');
                                }}
                                onFocus={() => setActiveDropdown('institute')}
                                onBlur={() => setActiveDropdown((current) => (current === 'institute' ? null : current))}
                                autoComplete="off"
                            />
                            {activeDropdown === 'institute' && instituteSuggestions.length > 0 && (
                                <div className="registrar-dropdown">
                                    {instituteSuggestions.map((value) => (
                                        <button
                                            key={value}
                                            type="button"
                                            className="registrar-dropdown-option"
                                            onMouseDown={(e) => {
                                                e.preventDefault();
                                                selectSuggestion('institute', value);
                                            }}
                                        >
                                            {value}
                                        </button>
                                    ))}
                                </div>
                            )}
                        </div>
                        <div className="registrar-filter-group">
                            <input
                                className="registrar-filter-input"
                                type="text"
                                placeholder="Кафедра"
                                value={searchFilters.department}
                                onChange={(e) => {
                                    updateSearchFilter('department', e.target.value);
                                    setActiveDropdown('department');
                                }}
                                onFocus={() => setActiveDropdown('department')}
                                onBlur={() => setActiveDropdown((current) => (current === 'department' ? null : current))}
                                autoComplete="off"
                            />
                            {activeDropdown === 'department' && departmentSuggestions.length > 0 && (
                                <div className="registrar-dropdown">
                                    {departmentSuggestions.map((value) => (
                                        <button
                                            key={value}
                                            type="button"
                                            className="registrar-dropdown-option"
                                            onMouseDown={(e) => {
                                                e.preventDefault();
                                                selectSuggestion('department', value);
                                            }}
                                        >
                                            {value}
                                        </button>
                                    ))}
                                </div>
                            )}
                        </div>
                        <input
                            className="registrar-filter-input"
                            type="text"
                            placeholder="Учреждение"
                            value={searchFilters.clinic}
                            onChange={(e) => updateSearchFilter('clinic', e.target.value)}
                        />
                        <input
                            className="registrar-filter-input"
                            type="text"
                            placeholder="Комментарий студента"
                            value={searchFilters.studentComment}
                            onChange={(e) => updateSearchFilter('studentComment', e.target.value)}
                        />
                        {activeTab !== 'vkhodyashie' && (
                            <input
                                className="registrar-filter-input"
                                type="text"
                                placeholder="Комментарий регистратора"
                                value={searchFilters.registrarComment}
                                onChange={(e) => updateSearchFilter('registrarComment', e.target.value)}
                            />
                        )}
                        {activeTab !== 'vkhodyashie' && (
                            <select
                                className="registrar-status-select"
                                value={statusFilter}
                                onChange={(e) => setStatusFilter(e.target.value)}
                            >
                                <option value="all">Все статусы</option>
                                <option value="2">Принято</option>
                                <option value="3">Отклонено</option>
                            </select>
                        )}
                    </div>
                    
                    <table className="status-table">
                        <thead>
                            <tr>
                                <th>Дата подачи</th>
                                <th>Учреждение</th>
                                <th>Период</th>
                                <th>{activeTab === 'vkhodyashie' ? 'Действия' : 'Статус'}</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredCertificates.length > 0 ? paginatedCertificates.map((cert) => (
                                <tr key={cert.id}>
                                    <td>{cert.createdAt ? formatDate(cert.createdAt) : '.'}</td>
                                    <td>{cert.clinic}</td>
                                    <td>{formatDate(cert.startDate)} - {formatDate(cert.endDate)}</td>
                                    <td>
                                        <button
                                            className="action-btn btn-view"
                                            onClick={() => openModal(cert)}
                                        >
                                            📄 Посмотреть
                                        </button>
                                        
                                        {activeTab !== 'vkhodyashie' && (
                                            <span className={`status-badge status-${cert.statusId}`} style={{marginLeft: '10px'}}>
                                                {cert.statusId === 2 ? 'Принято' : cert.statusId === 3 ? 'Отклонено' : 'Неизвестно'}
                                            </span>
                                        )}
                                    </td>
                                </tr>
                            )) : (
                                <tr>
                                    <td colSpan="4" style={{textAlign: 'center', padding: '20px'}}>Справки не найдены</td>
                                </tr>
                            )}
                        </tbody>
                    </table>

                    {filteredCertificates.length > 0 && (
                        <div className="pagination-container">
                            <button
                                type="button"
                                className="pagination-btn"
                                onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 1))}
                                disabled={currentPage === 1}
                            >
                                Назад
                            </button>
                            <span className="pagination-info">Страница {currentPage} из {totalPages}</span>
                            <button
                                type="button"
                                className="pagination-btn"
                                onClick={() => setCurrentPage((prev) => Math.min(prev + 1, totalPages))}
                                disabled={currentPage === totalPages}
                            >
                                Вперед
                            </button>
                        </div>
                    )}
                </div>
            </main>

            {/* Модальное окно */}
            {selectedCert && (
                <div className="modal-overlay" onClick={closeModal}>
                    <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                        <div className="modal-header">
                            <h3>Обработка заявки: №{String(selectedCert.id).padStart(6, '0')}</h3>
                            <button className="modal-close-btn" onClick={closeModal}>&times;</button>
                        </div>
                        
                        <div className="modal-body">
                            <div className="modal-left">
                                    <div className="modal-image-container">
                                        {isPreviewLoading && (
                                            <p className="preview-state">Загрузка файла...</p>
                                        )}

                                        {!isPreviewLoading && previewError && (
                                            <div className="preview-state">
                                                <p>{previewError}</p>
                                                {selectedCert.filePathId && (
                                                    <a
                                                        href={buildFileUrl(selectedCert.filePathId)}
                                                        target="_blank"
                                                        rel="noreferrer"
                                                    >
                                                        Открыть файл в новой вкладке
                                                    </a>
                                                )}
                                            </div>
                                        )}

                                        {!isPreviewLoading && !previewError && previewType === 'pdf' && (
                                            <iframe
                                                src={previewUrl}
                                                title="Медицинская справка PDF"
                                                className="modal-file-preview pdf-preview"
                                            />
                                        )}

                                        {!isPreviewLoading && !previewError && previewType !== 'pdf' && previewUrl && (
                                            <img
                                                src={previewUrl}
                                                alt="Медицинская справка"
                                                className="modal-file-preview"
                                            />
                                        )}
                                    </div>
                                </div>

                            <div className="modal-right">
                                    <div className="info-block">
                                    <h4>Данные студента</h4>
                                    <div className="info-grid">
                                        <span>ФИО</span>
                                        <span>{getTextValue(selectedCert.user, 'fullName', 'FullName', 'userName', 'UserName', 'name', 'Name') || 'Данные отсутствуют'}</span>

                                        <span>ИИН</span>
                                        <span>{getTextValue(selectedCert.user, 'iin', 'IIN') || 'Данные отсутствуют'}</span>

                                        <span>Кафедра</span>
                                        <span>{stripLeadingLabel(getTextValue(selectedCert.user, 'department', 'Department'), ['Кафедра', 'Department', 'Chair']) || 'Данные отсутствуют'}</span>

                                        <span>Институт</span>
                                        <span>{stripLeadingLabel(getTextValue(selectedCert.user, 'institute', 'Institute'), ['Институт', 'Institute']) || 'Данные отсутствуют'}</span>
                                    </div>
                                </div>

                                <div className="info-block">
                                    <h4>Детали справки</h4>
                                    <div className="info-text">
                                        <p className="info-label">Период болезни</p>
                                        <p>{formatDate(selectedCert.startDate)} - {formatDate(selectedCert.endDate)}</p>
                                        
                                        <p className="info-label">Учреждение:</p>
                                        <p>{selectedCert.clinic || '****'}</p>
                                        
                                        <p className="info-label">Комментарий студента</p>
                                        <p>{selectedCert.comment || '****'}</p>
                                    </div>
                                </div>

                                {activeTab === 'vkhodyashie' ? (
                                    <div className="info-block action-block">
                                        <h4>Действие</h4>
                                        <textarea
                                            placeholder="Комментарий к отклонению"
                                            value={registrarComment}
                                            onChange={(e) => setRegistrarComment(e.target.value)}
                                        ></textarea>
                                        <div className="modal-actions">
                                            <button
                                                className="btn-reject"
                                                onClick={() => handleAction(selectedCert.id, 'reject')}
                                            >
                                                Отклонить
                                            </button>
                                            <button
                                                className="btn-approve"
                                                onClick={() => handleAction(selectedCert.id, 'approve')}
                                            >
                                                Подтвердить
                                            </button>
                                        </div>
                                    </div>
                                ) : (
                                    <div className="info-block">
                                        <h4>Комментарий регистратора</h4>
                                        <div className="info-text">
                                            <p>{selectedCert.reviewerComment || 'Нет комментария'}</p>
                                        </div>
                                    </div>
                                )}
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default RegistrarPage;
