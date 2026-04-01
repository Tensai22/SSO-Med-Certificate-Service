import React, { useEffect, useState } from 'react';
import '../css/RegistrarPage.css';

const RegistrarPage = () => {
    const [certificates, setCertificates] = useState([]);
    const [filterStatus, setFilterStatus] = useState('Все');
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        setCertificates([
            {
                id: 113454,
                fullName: "Айдос Нурлан",
                iin: "010101123456",
                specialty: "6806102 Computer Science",
                createdAt: "20.02.2025 20:50",
                type: "Справка по болезни",
                status: "Подтверждено",
                fileUrl: "/certificates/1.pdf"
            },
            {
                id: 113453,
                fullName: "Сауле Кайратова",
                iin: "020202654321",
                specialty: "6806102 Computer Science",
                createdAt: "20.02.2025 20:39",
                type: "Справка по болезни",
                status: "На рассмотрении",
                fileUrl: "/certificates/2.pdf"
            }
        ]);
    }, []);

    const handleApprove = (id) => {
        setCertificates(prev =>
            prev.map(c => c.id === id ? { ...c, status: "Подтверждено" } : c)
        );
    };

    const handleReject = (id) => {
        const reason = prompt("Причина отклонения:");
        if (reason) {
            setCertificates(prev =>
                prev.map(c => c.id === id ? { ...c, status: "Отклонено", rejectionReason: reason } : c)
            );
        }
    };

    const filtered = certificates.filter(c => {
        const matchesStatus = filterStatus === "Все" || c.status === filterStatus;
        const search = searchTerm.toLowerCase();
        const matchesSearch =
            c.fullName.toLowerCase().includes(search) ||
            c.iin.includes(search);
        return matchesStatus && matchesSearch;
    });

    return (
        <div className="registrar-container">
            <h1 className="registrar-title">Список справок</h1>

            <div className="filter-block">
                <select value={filterStatus} onChange={(e) => setFilterStatus(e.target.value)}>
                    <option value="Все">Все</option>
                    <option value="На рассмотрении">На рассмотрении</option>
                    <option value="Подтверждено">Подтверждено</option>
                    <option value="Отклонено">Отклонено</option>
                </select>
                <input
                    type="text"
                    className="search-input"
                    placeholder="Поиск по ФИО или ИИН"
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                />
            </div>

            <table className="certificate-table">
                <thead>
                <tr>
                    <th>Номер</th>
                    <th>ФИО</th>
                    <th>ИИН</th>
                    <th>Специальность</th>
                    <th>Дата подачи</th>
                    <th>Тип</th>
                    <th>Статус</th>
                    <th>Файл</th>
                    <th>Действия</th>
                </tr>
                </thead>
                <tbody>
                {filtered.map(c => (
                    <tr key={c.id}>
                        <td>{c.id}</td>
                        <td>{c.fullName}</td>
                        <td>{c.iin}</td>
                        <td>{c.specialty}</td>
                        <td>{c.createdAt}</td>
                        <td>{c.type}</td>
                        <td>
                <span className={`status ${c.status === 'Подтверждено' ? 'approved' :
                    c.status === 'Отклонено' ? 'rejected' : 'pending'}`}>
                  {c.status}
                </span>
                        </td>
                        <td>
                            <a href={c.fileUrl} target="_blank" rel="noreferrer">Скачать</a>
                        </td>
                        <td>
                            {c.status === "На рассмотрении" && (
                                <>
                                    <button onClick={() => handleApprove(c.id)}>✔</button>
                                    <button onClick={() => handleReject(c.id)}>✖</button>
                                </>
                            )}
                        </td>
                    </tr>
                ))}
                </tbody>
            </table>
        </div>
    );
};

export default RegistrarPage;