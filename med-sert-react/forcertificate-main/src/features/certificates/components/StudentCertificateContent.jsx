import { useCallback, useEffect, useState } from 'react';
import '../../../css/Content.css';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import {
    createCertificate,
    fetchUserCertificates,
} from '../../../services/certificateService';
import { uploadCertificateFile } from '../../../services/fileService';

const initialFormState = {
    comment: '',
    dateFrom: '',
    dateTo: '',
    institution: '',
};

function StudentCertificateContent() {
    const [activeTab, setActiveTab] = useState('справка');
    const [certificates, setCertificates] = useState([]);
    const [file, setFile] = useState(null);
    const [formData, setFormData] = useState(initialFormState);

    const fetchCertificates = useCallback(async () => {
        const userId = localStorage.getItem('userId');

        if (!userId) {
            console.error('Пользователь не авторизован');
            return;
        }

        try {
            const data = await fetchUserCertificates(userId);
            setCertificates(data);
        } catch (error) {
            console.error('Ошибка при загрузке ваших справок:', error);
        }
    }, []);

    useEffect(() => {
        if (activeTab === 'статус') {
            fetchCertificates();
        }
    }, [activeTab, fetchCertificates]);

    const handleFileChange = (event) => {
        const selectedFile = event.target.files?.[0] ?? null;
        if (!selectedFile) {
            return;
        }

        const isPdf = selectedFile.type === 'application/pdf'
            || selectedFile.name.toLowerCase().endsWith('.pdf');

        if (!isPdf) {
            toast.error('Можно загрузить только PDF файл');
            event.target.value = '';
            return;
        }

        setFile(selectedFile);
    };

    const handleInputChange = (event) => {
        setFormData((previous) => ({
            ...previous,
            [event.target.name]: event.target.value,
        }));
    };

    const handleSubmit = async () => {
        const currentUserId = localStorage.getItem('userId');

        if (!file || !formData.dateFrom || !formData.dateTo) {
            toast.error('Заполните все поля и выберите файл');
            return;
        }

        if (!currentUserId) {
            toast.error('Ошибка авторизации. Пожалуйста, войдите в систему снова.');
            return;
        }

        const isPdf = file.type === 'application/pdf' || file.name.toLowerCase().endsWith('.pdf');
        if (!isPdf) {
            toast.error('Можно загрузить только PDF файл');
            return;
        }

        try {
            const uploadResult = await uploadCertificateFile(file);
            const fileId = uploadResult.id;

            const certificateRequest = {
                UserId: parseInt(currentUserId, 10),
                StartDate: formData.dateFrom,
                EndDate: formData.dateTo,
                Clinic: formData.institution,
                Comment: formData.comment,
                FilePathId: fileId,
                StatusId: 1,
                ReviewerComment: '',
            };

            await createCertificate(certificateRequest);
            toast.success('Справка успешно отправлена!');
            setFormData(initialFormState);
            setFile(null);
            setActiveTab('статус');
        } catch (error) {
            const errorMsg = error.response?.data?.title || 'Ошибка при отправке';
            toast.error(errorMsg);
        }
    };

    const formatDate = (dateString) => {
        if (!dateString) return '';
        const date = new Date(dateString);

        return date.toLocaleDateString('ru-RU');
    };

    return (
        <div className="page-container">
            <aside className="sidebar">
                <button className={activeTab === 'справка' ? 'active' : ''} onClick={() => setActiveTab('справка')}>Справка</button>
                <button className={activeTab === 'статус' ? 'active' : ''} onClick={() => setActiveTab('статус')}>Статус</button>
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
                {activeTab === 'справка' && (
                    <div className="form-card">
                        <h2 className="form-title">Добавить справку</h2>
                        <div className="form-grid">
                            <div className="input-group">
                                <label>Период с</label>
                                <input type="date" name="dateFrom" onChange={handleInputChange} />
                            </div>
                            <div className="input-group">
                                <label>Период по</label>
                                <input type="date" name="dateTo" onChange={handleInputChange} />
                            </div>
                            <input type="text" name="institution" placeholder="Учреждение" className="full-width-input" onChange={handleInputChange} />
                            <input type="text" name="comment" placeholder="Комментарии" className="full-width-input" onChange={handleInputChange} />
                        </div>
                        <div className="actions">
                            <label className="file-upload-link">
                                📎 {file ? file.name : 'Загрузить справку (PDF)'}
                                <input
                                    type="file"
                                    accept=".pdf,application/pdf"
                                    hidden
                                    onChange={handleFileChange}
                                />
                            </label>
                            <button className="submit-outline-btn" onClick={handleSubmit}>Отправить справку</button>
                        </div>
                    </div>
                )}

                {activeTab === 'статус' && (
                    <div className="form-card">
                        <h2 className="form-title">Статус моих заявок</h2>
                        <table className="status-table">
                            <thead>
                                <tr>
                                    <th>Дата подачи</th>
                                    <th>Учреждение</th>
                                    <th>Период</th>
                                    <th>Статус</th>
                                </tr>
                            </thead>
                            <tbody>
                                {certificates.length > 0 ? certificates.map((cert) => (
                                    <tr key={cert.id}>
                                        <td>{cert.createdAt ? formatDate(cert.createdAt) : '.'}</td>
                                        <td>{cert.clinic}</td>
                                        <td>{formatDate(cert.startDate)} - {formatDate(cert.endDate)}</td>
                                        <td>
                                            <span className={`status-badge status-${cert.statusId}`}>
                                                {
                                                    {
                                                        1: 'В обработке',
                                                        2: 'Принято',
                                                        3: 'Отклонено'
                                                    }[cert.statusId] || 'Неизвестный статус'
                                                }
                                            </span>
                                        </td>
                                    </tr>
                                )) : (
                                    <tr>
                                        <td colSpan="4" style={{textAlign: 'center', padding: '20px'}}>Заявок пока нет</td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                )}
            </main>
        </div>
    );
}

export default StudentCertificateContent;
