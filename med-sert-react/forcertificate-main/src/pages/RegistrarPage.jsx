import React, { useEffect, useState } from 'react';
import axios from 'axios';
import '../css/RegistrarPage.css';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const RegistrarPage = () => {
    const [activeTab, setActiveTab] = useState('vkhodyashie'); // 'vkhodyashie' или 'istoriya'
    const [certificates, setCertificates] = useState([]);
    
    // Состояния для модального окна
    const [selectedCert, setSelectedCert] = useState(null);
    const [registrarComment, setRegistrarComment] = useState('');
    
    const registrarId = 1; // ID текущего регистратора

    useEffect(() => {
        fetchCertificates();
    }, [activeTab]);

    const fetchCertificates = async () => {
        try {
            const response = await axios.get('http://localhost:5280/api/Certificate');
            const allData = response.data;

            if (activeTab === 'vkhodyashie') {
                // Показываем только те, что ждут проверки (statusId = 1)
                setCertificates(allData.filter(cert => cert.statusId === 1));
            } else {
                // Показываем всё остальное (Принято/Отклонено)
                setCertificates(allData.filter(cert => cert.statusId !== 1));
            }
        } catch (error) {
            console.error("Ошибка при загрузке данных:", error);
        }
    };

    const handleAction = async (id, action) => {
        try {
            const url = `http://localhost:5280/api/Certificate/${id}/${action}`;
            
            // Подстраиваем payload под вашу модель (ReviewerComment вместо comment)
            const payload = action === 'approve' 
                ? { approvedByUserId: registrarId, reviewerComment: registrarComment }
                : { rejectedByUserId: registrarId, reviewerComment: registrarComment || "Не соответствует требованиям" };

            await axios.post(url, payload);
            toast.success(action === 'approve' ? "Справка подтверждена" : "Справка отклонена");
            
            closeModal(); // Закрываем модальное окно
            fetchCertificates(); // Обновляем список
        } catch (error) {
            console.error("Ошибка при обработке:", error);
            toast.error("Не удалось выполнить действие");
        }
    };

    const formatDate = (dateString) => {
        if (!dateString) return '';
        const date = new Date(dateString);
        
        // 'ru-RU' гарантирует формат ДД.ММ.ГГГГ
        return date.toLocaleDateString('ru-RU');
    };

    const openModal = (cert) => {
        setSelectedCert(cert);
        setRegistrarComment(cert.reviewerComment || ''); // Если есть старый комментарий, показываем его
    };

    const closeModal = () => {
        setSelectedCert(null);
    };

    // Функция для получения URL картинки по FilePathId
    const getImageUrl = (filePathId) => {
        if (!filePathId) return "https://via.placeholder.com/400x500?text=Нет+изображения";
        
        // Путь теперь соответствует новому методу в контроллере [HttpGet("{id}")]
        return `http://localhost:5280/api/File/${filePathId}`; 
    };

    return (
        <div className="page-container">
            <aside className="sidebar">
                <button 
                    className={activeTab === 'vkhodyashie' ? 'active' : ''} 
                    onClick={() => setActiveTab('vkhodyashie')}
                >
                    Входящие данные
                </button>
                <button 
                    className={activeTab === 'istoriya' ? 'active' : ''} 
                    onClick={() => setActiveTab('istoriya')}
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
                            {certificates.length > 0 ? certificates.map((cert) => (
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
                                    <td colSpan="4" style={{textAlign: 'center', padding: '20px'}}>Справок нет</td>
                                </tr>
                            )}
                        </tbody>
                    </table>
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
                                    {/* Берем картинку по FilePathId */}
                                    <img 
                                        src={getImageUrl(selectedCert.filePathId)} 
                                        alt="Медицинская справка" 
                                    />
                                </div>
                            </div>

                            <div className="modal-right">
                                <div className="info-block">
                                    <h4>Данные студента</h4>
                                    <div className="info-grid">
                                        {/* Обращаемся к вложенному объекту User */}
                                            <span>ФИО</span> 
                                        <span>{selectedCert.user?.userName || selectedCert.user?.name || 'Данные отсутствуют'}</span>
                                        
                                        {/* 2. Проверьте iin (все маленькие) */}
                                        <span>ИИН</span> 
                                        <span>{selectedCert.user?.iin || 'Данные отсутствуют'}</span>
                                        {/*<span>Институт</span> <span>{selectedCert.user?.institute || '****'}</span>*/}
                                        {/*<span>Группа</span> <span>{selectedCert.user?.group || '****'}</span>*/}
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
                                        {/* В модели это свойство Comment */}
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