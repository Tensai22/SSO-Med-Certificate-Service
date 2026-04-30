import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '../css/LoginPage.css';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { login as loginUser } from '../services/authService';
import { normalizeRoleId } from '../constants/roles';
import { saveAuthData } from '../utils/auth';

const LoginPage = () => {
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');
    const navigate = useNavigate();

    const handleLogin = async (event) => {
        event.preventDefault();

        const fullEmail = login.includes('@') ? login : `${login}@satbayev.university`;

        try {
            const data = await loginUser({ email: fullEmail, password });
            const actualId = data.userId || data.UserId || data.id || data.Id;
            const roleId = normalizeRoleId(data.roleId ?? data.RoleId);
            const roleName = data.roleName || data.RoleName || '';
            const token = data.token || data.Token;

            if (!actualId) {
                toast.error('Ошибка данных сервера. ID пользователя не найден.');
                return;
            }

            if (!roleId && !roleName) {
                toast.error('Ошибка данных сервера. Роль пользователя не найдена.');
                return;
            }

            if (!token) {
                toast.error('Ошибка данных сервера. Токен авторизации не найден.');
                return;
            }

            saveAuthData({
                token,
                userId: actualId,
                user: {
                    roleId,
                    email: fullEmail,
                    roleName,
                    userName: data.userName || data.UserName || '',
                },
            });

            navigate('/');
        } catch (error) {
            console.error('Ошибка при запросе:', error);
            const errorMessage = error.response?.data?.title || 'Неверный логин или пароль';
            toast.error(errorMessage);
        }
    };

    return (
        <div className="login-wrapper">
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
            <div className="login-box">
                <img src="/assets/satbayev-logo.png" alt="Satbayev University" className="logo" />
                <h2 className="title">Учебная система</h2>
                <form className="login-form" onSubmit={handleLogin}>
                    <div className="input-group">
                        <input
                            type="text"
                            placeholder="Логин"
                            value={login}
                            onChange={e => setLogin(e.target.value)}
                            required
                        />
                    </div>
                    <div className="input-group">
                        <input
                            type="password"
                            placeholder="Пароль"
                            value={password}
                            onChange={e => setPassword(e.target.value)}
                            required
                        />
                    </div>
                    <button type="submit" className="login-btn">ВОЙТИ</button>
                </form>
            </div>
        </div>
    );
};

export default LoginPage;
