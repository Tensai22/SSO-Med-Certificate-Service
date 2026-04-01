import { useState, useEffect } from 'react';
import axios from 'axios';
import '../css/Content.css';

function Content() {
    const [activeTab, setActiveTab] = useState('справка');
    const [certificates, setCertificates] = useState([]); 
    const [file, setFile] = useState(null);
    const [formData, setFormData] = useState({
        comment: '',
        dateFrom: '',
        dateTo: '',
        institution: ''
    });

    useEffect(() => {
        if (activeTab === 'статус' || activeTab === 'история') {
            fetchCertificates();
        }
    }, [activeTab]);

    const fetchCertificates = async () => {
        try {
            const response = await axios.get('http://localhost:5280/api/Certificate');
        
            console.log("DATA FROM BACKEND:", response.data);
            
            setCertificates(response.data);
        } catch (error) {
            console.error("Ошибка при загрузке статусов:", error);
        }
    };

    const handleFileChange = (e) => setFile(e.target.files[0]);
    const handleInputChange = (e) => setFormData({ ...formData, [e.target.name]: e.target.value });

    const handleSubmit = async () => {
        if (!file || !formData.dateFrom || !formData.dateTo) {
            alert("Заполните все поля и выберите файл");
            return;
        }
        try {
            const fileData = new FormData();
            fileData.append('File', file); 
            const fileResponse = await axios.post('http://localhost:5280/api/File/upload', fileData);
            const fileId = fileResponse.data.id;

            const certificateRequest = {
                UserId: 1,
                StartDate: formData.dateFrom, 
                EndDate: formData.dateTo,
                Clinic: formData.institution,
                Comment: formData.comment,
                FilePathId: fileId,
                StatusId: 1, 
                ReviewerComment: ""
            };

            await axios.post('http://localhost:5280/api/Certificate', certificateRequest);
            alert("Справка успешно отправлена!");
            setActiveTab('статус');
        } catch (error) {
            alert("Ошибка при отправке");
        }
    };

    const formatDate = (dateString) => {
        if (!dateString) return '';
        const date = new Date(dateString);
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const year = date.getFullYear();
    
        return `${day}-${month}-${year}`;
    };

    return (
        <div className="page-container">
            <aside className="sidebar">
                <button className={activeTab === 'справка' ? 'active' : ''} onClick={() => setActiveTab('справка')}>Справка</button>
                <button className={activeTab === 'статус' ? 'active' : ''} onClick={() => setActiveTab('статус')}>Статус</button>
            </aside>

            <main className="main-content">
                {/* ВКЛАДКА: ДОБАВИТЬ СПРАВКУ */}
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
                                📎 {file ? file.name : "Загрузить справку (PDF/JPG/PNG)"}
                                <input type="file" hidden onChange={handleFileChange} />
                            </label>
                            <button className="submit-outline-btn" onClick={handleSubmit}>Отправить справку</button>
                        </div>
                    </div>
                )}

                {/* ВКЛАДКА: СТАТУС */}
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

export default Content;