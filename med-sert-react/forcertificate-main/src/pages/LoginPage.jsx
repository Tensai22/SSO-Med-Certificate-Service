import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom'; // Импортируем хук для редиректа
import '../css/LoginPage.css';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const LoginPage = () => {
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');
    const navigate = useNavigate();

    // LoginPage.js (обновленная часть)
const handleLogin = async (e) => {
    e.preventDefault();

    const fullEmail = login.includes('@') ? login : `${login}@satbayev.university`;

    try {
        const response = await fetch('http://localhost:5280/api/Auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email: fullEmail, password: password })
        });

        if (response.ok) {
            const data = await response.json();
            console.log("Полный ответ сервера:", data); // Посмотри, как там называется ID (может быть Id, UserId или sub)

            // Проверяем все частые варианты названия ID
            const actualId = data.userId || data.UserId || data.id || data.Id;

            if (actualId) {
                localStorage.setItem('userId', actualId.toString());
                
                localStorage.setItem('token', data.token);
                localStorage.setItem('user', JSON.stringify({ 
                    roleId: data.roleId || data.RoleId,
                    email: fullEmail 
                }));

                navigate(data.roleId === 1 || data.RoleId === 1 ? '/registrar' : '/sertificate');
            } else {
                console.error("Ошибка: сервер не прислал ID пользователя в поле userId или Id");
                toast.error('Ошибка данных сервера. ID пользователя не найден.');
            }
        }
            } catch (error) {
                console.error('Ошибка при запросе:', error);
                toast.error('Сервер не отвечает');
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