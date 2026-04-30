// components/Header.js
import { useNavigate } from 'react-router-dom';
import { useState, useEffect } from 'react';
import '../css/Header.css'
import { isRegistrarRole } from '../constants/roles';
import { clearAuthStorage, getStoredUser } from '../utils/auth';
import apiClient from '../services/apiClient';

const EyeIcon = () => (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
        <path d="M12 5C6.5 5 2.1 8.4 0.6 12c1.5 3.6 5.9 7 11.4 7s9.9-3.4 11.4-7C21.9 8.4 17.5 5 12 5zm0 11.2c-2.9 0-5.2-2.3-5.2-5.2S9.1 5.8 12 5.8s5.2 2.3 5.2 5.2-2.3 5.2-5.2 5.2zm0-8.3a3.1 3.1 0 1 0 0 6.2 3.1 3.1 0 0 0 0-6.2z" />
    </svg>
);

const LogoutIcon = () => (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
        <path d="M10.2 17.4H5.9V6.6h4.3V5H4.3v14h5.9v-1.6zm8.8-4.6-4.3-4.3-1.1 1.1 2.4 2.4H9v1.6h7l-2.4 2.4 1.1 1.1 4.3-4.3z" />
    </svg>
);

function Header() {
    const navigate = useNavigate();
    const [userName, setUserName] = useState('');
    const [userRole, setUserRole] = useState('');

    useEffect(() => {
        let isMounted = true;
        const user = getStoredUser();

        if (!user) {
            return () => {
                isMounted = false;
            };
        }

        setUserName(user.userName || user.UserName || 'Пользователь');
        setUserRole(isRegistrarRole(user.roleId, user.roleName) ? 'Регистратура' : 'Бакалавриат');

        const loadCurrentUser = async () => {
            if (user.userName || user.UserName) {
                return;
            }

            const response = await apiClient.get('/Auth/me');
            const profile = response?.data?.user ?? response?.data;
            const resolvedName = profile?.userName || profile?.UserName || '';

            if (!isMounted || !resolvedName) {
                return;
            }

            setUserName(resolvedName);

            const currentUser = getStoredUser() || {};
            localStorage.setItem('user', JSON.stringify({
                ...currentUser,
                ...profile,
                userName: resolvedName,
            }));
        };

        loadCurrentUser();

        return () => {
            isMounted = false;
        }
    }, []);

    const handleLogout = () => {
        clearAuthStorage();
        navigate('/login');
    };

    return (
        <div className="cab_header_center">
            <div className="title_text_block">
                <div ng-if="$ctrl.byArray">
                    <div className="top_text">
                        <div className="menu">
                            <a href="//sso.satbayev.university" className="menu_link">
                                <div className="menu_button">
                                    <img src="/assets/home.png" alt="Домой" width="18" height="18" />
                                </div>
                            </a>
                        </div>
                    </div>
                </div>
            </div>
            <div className="operator_block">
                <div className="visually_impaired">
                    <button type="button" className="bvi-open">
                        <EyeIcon />
                        <span>Версия для слабовидящих</span>
                    </button>
                </div>
                <div className="oper_name_block">
                    <p>{userName}</p>
                    <p className="stat">{userRole}</p>
                </div>
                <div className="oper_set_block">
                    <div className="img default-img" ng-if="$ctrl.imageSrc &amp;&amp; $ctrl.showImage"
                         ng-class="{'default-img': !$ctrl.hasImage}">
                        <img ng-src="/assets/default-avatar.png"
                             src="/assets/default-avatar.png"
                             alt="Аватар"
                             className="bvi-img"/>
                    </div>

                    <button type="button" className="exit_block" onClick={handleLogout}>
                        <LogoutIcon />
                        <p>Выход</p>
                    </button>
                    
                    <div className="lang_block">
                        <button onClick={() => console.log('Switch to Kazakh')} className="button oper_button">
                            Қаз
                        </button>
                        <button onClick={() => console.log('Switch to Russian')} className="button oper_button active">
                            Рус
                        </button>
                        <button onClick={() => console.log('Switch to English')} className="button oper_button">
                            Eng
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Header;
