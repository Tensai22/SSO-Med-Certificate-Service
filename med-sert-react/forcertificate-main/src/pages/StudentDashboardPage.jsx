import { useNavigate } from 'react-router-dom';
import '../css/RoleDashboard.css';

const StudentDashboardPage = () => {
    const navigate = useNavigate();

    return (
        <div className="role-dashboard-page">
            <section className="role-cards-grid">
                <button
                    type="button"
                    className="role-card"
                    onClick={() => navigate('/my-sertificates')}
                >
                    <span className="role-card-title">Мои справки</span>
                    <span className="role-card-description">Подача новых справок и отслеживание статуса заявок.</span>
                </button>
            </section>

            <aside className="role-notifications-panel">
                <h3 className="role-notifications-title">Уведомления</h3>
                <div className="role-notification-item">
                    <p className="role-notification-headline">Модуль справок готов к работе</p>
                    <p className="role-notification-text">Новых уведомлений сейчас нет.</p>
                </div>
            </aside>
        </div>
    );
};

export default StudentDashboardPage;
