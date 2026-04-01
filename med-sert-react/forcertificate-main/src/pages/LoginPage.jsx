import React, { useState } from 'react';
import '../css/LoginPage.css';

const LoginPage = () => {
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');

    const handleLogin = (e) => {
        e.preventDefault();
        alert(`Логин: ${login}@satbayev.university\nПароль: ${password}`);
        // здесь будет вызов API
    };

    return (
        <div className="login-wrapper">
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
                <div className="forgot">Изменить пароль</div>
            </div>
        </div>
    );
};

export default LoginPage;
