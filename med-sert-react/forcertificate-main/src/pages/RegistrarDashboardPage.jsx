import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { fetchAllCertificates } from '../services/certificateService';
import '../css/RoleDashboard.css';

const RegistrarDashboardPage = () => {
    const navigate = useNavigate();
    const [incomingCount, setIncomingCount] = useState(0);
    const [isLoading, setIsLoading] = useState(true);
    const [loadError, setLoadError] = useState('');

    useEffect(() => {
        let isMounted = true;

        const loadIncomingCount = async () => {
            try {
                const allCertificates = await fetchAllCertificates();
                const incomingCertificatesCount = Array.isArray(allCertificates)
                    ? allCertificates.filter((certificate) => certificate.statusId === 1).length
                    : 0;

                if (isMounted) {
                    setIncomingCount(incomingCertificatesCount);
                }
            } catch (error) {
                if (isMounted) {
                    setLoadError('Не удалось загрузить уведомления');
                }
            } finally {
                if (isMounted) {
                    setIsLoading(false);
                }
            }
        };

        loadIncomingCount();

        return () => {
            isMounted = false;
        };
    }, []);

    return (
        <div className="role-dashboard-page">
            <section className="role-cards-grid">
                <button
                    type="button"
                    className="role-card"
                    onClick={() => navigate('/incoming-sertificates')}
                >
                    <span className="role-card-title">Входящие справки</span>
                    <span className="role-card-description">Проверка, подтверждение и отклонение входящих заявок.</span>
                </button>
            </section>

            <aside className="role-notifications-panel">
                <h3 className="role-notifications-title">Уведомления</h3>
                <div className="role-notification-item role-notification-item-accent">
                    <p className="role-notification-headline">Новые входящие справки</p>
                    <p className="role-notification-text">
                        {isLoading && 'Подсчет входящих справок...'}
                        {!isLoading && loadError}
                        {!isLoading && !loadError && `Текущих входящих справок: ${incomingCount}`}
                    </p>
                    {!isLoading && !loadError && (
                        <span className="role-notification-count">{incomingCount}</span>
                    )}
                </div>
            </aside>
        </div>
    );
};

export default RegistrarDashboardPage;
