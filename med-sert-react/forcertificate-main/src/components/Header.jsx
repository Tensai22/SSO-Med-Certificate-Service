// components/Header.js
import { useNavigate } from 'react-router-dom';
import { useState, useEffect } from 'react';
import '../css/Header.css'
import { isRegistrarRole } from '../constants/roles';
import { clearAuthStorage, getStoredUser } from '../utils/auth';

function Header() {
    const navigate = useNavigate();
    const [userName, setUserName] = useState('');
    const [userRole, setUserRole] = useState('');

    useEffect(() => {
        const user = getStoredUser();
        if (user) {
            setUserName(user.userName || user.email || 'Пользователь');
            setUserRole(isRegistrarRole(user.roleId, user.roleName) ? 'Регистратура' : 'Бакалавриат');
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
                    <button type="button" className="bvi-open" style={{ background: 'none', border: 'none', cursor: 'pointer' }}>
                        <i className="icon-eye"></i>&nbsp;&nbsp; Версия для слабовидящих
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

                    <div className="exit_block" onClick={handleLogout} style={{ cursor: 'pointer' }}>
                        <i className="icon-enter"></i>
                        <p>Выход</p>
                    </div>
                    
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
