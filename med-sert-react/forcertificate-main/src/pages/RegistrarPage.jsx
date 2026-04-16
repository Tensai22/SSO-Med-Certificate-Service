import React, { useCallback, useEffect, useMemo, useState } from 'react';
import '../css/RegistrarPage.css';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import {
    approveCertificate,
    fetchAllCertificates,
    rejectCertificate,
} from '../services/certificateService';
import { buildFileUrl, downloadFileById } from '../services/fileService';

const PAGE_SIZE = 30;
const INITIAL_SEARCH_FILTERS = {
    fullName: '',
    iin: '',
    clinic: '',
    studentComment: '',
    registrarComment: '',
};

const RegistrarPage = () => {
    const [activeTab, setActiveTab] = useState('vkhodyashie');
    const [certificates, setCertificates] = useState([]);
    const [selectedCert, setSelectedCert] = useState(null);
    const [registrarComment, setRegistrarComment] = useState('');
    const [searchFilters, setSearchFilters] = useState(INITIAL_SEARCH_FILTERS);
    const [statusFilter, setStatusFilter] = useState('all');
    const [currentPage, setCurrentPage] = useState(1);
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

    useEffect(() => {
        fetchCertificates();
    }, [fetchCertificates]);

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

            const fullName = (cert.user?.userName || cert.user?.name || '').toLowerCase();
            const iin = (cert.user?.iin || '').toLowerCase();
            const clinic = (cert.clinic || '').toLowerCase();
            const studentComment = (cert.comment || '').toLowerCase();
            const reviewerComment = (cert.reviewerComment || '').toLowerCase();

            const normalizedFullName = searchFilters.fullName.trim().toLowerCase();
            const normalizedIin = searchFilters.iin.trim().toLowerCase();
            const normalizedClinic = searchFilters.clinic.trim().toLowerCase();
            const normalizedStudentComment = searchFilters.studentComment.trim().toLowerCase();
            const normalizedRegistrarComment = searchFilters.registrarComment.trim().toLowerCase();

            const matchesFullName = !normalizedFullName || fullName.includes(normalizedFullName);
            const matchesIin = !normalizedIin || iin.includes(normalizedIin);
            const matchesClinic = !normalizedClinic || clinic.includes(normalizedClinic);
            const matchesStudentComment = !normalizedStudentComment || studentComment.includes(normalizedStudentComment);
            const matchesRegistrarComment = !normalizedRegistrarComment || reviewerComment.includes(normalizedRegistrarComment);

            return matchesFullName
                && matchesIin
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

    useEffect(() => {
        setCurrentPage(1);
    }, [searchFilters, statusFilter, activeTab]);

    const updateSearchFilter = (field, value) => {
        setSearchFilters((prev) => ({
            ...prev,
            [field]: value,
        }));
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
                        <input
                            className="registrar-filter-input"
                            type="text"
                            placeholder="Комментарий регистратора"
                            value={searchFilters.registrarComment}
                            onChange={(e) => updateSearchFilter('registrarComment', e.target.value)}
                        />
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
                                        <span>{selectedCert.user?.userName || selectedCert.user?.name || 'Данные отсутствуют'}</span>

                                        <span>ИИН</span>
                                        <span>{selectedCert.user?.iin || 'Данные отсутствуют'}</span>
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
                                            placeholder="Комментарий регистратора"
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
