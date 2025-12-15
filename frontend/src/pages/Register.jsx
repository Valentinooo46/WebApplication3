import React, { useState } from "react";
import { saveAuthData } from "../services/auth";
import { useNavigate } from "react-router-dom";

export default function Register() {
    const [form, setForm] = useState({ email: "", password: "", firstName: "", lastName: "", iconUrl: "" });
    const navigate = useNavigate();

    const onChange = e => setForm({ ...form, [e.target.name]: e.target.value });

    const onSubmit = async e => {
        e.preventDefault();
        const backendUrl = import.meta.env.VITE_BACKEND_URL;

        const res = await fetch(`${backendUrl}/api/auth/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(form)
        });

        if (!res.ok) {
            alert("Помилка реєстрації");
            return;
        }

        alert("Успішна реєстрація! Тепер увійдіть.");
        navigate("/login-local");
    };

    return (
        <div className="center">
            <h2>Реєстрація</h2>
            <form onSubmit={onSubmit} style={{ display: "flex", flexDirection: "column", gap: 12, maxWidth: 320 }}>
                <input name="email" placeholder="Email" value={form.email} onChange={onChange} />
                <input name="password" type="password" placeholder="Пароль" value={form.password} onChange={onChange} />
                <input name="firstName" placeholder="Ім'я" value={form.firstName} onChange={onChange} />
                <input name="lastName" placeholder="Прізвище" value={form.lastName} onChange={onChange} />
                <input name="iconUrl" placeholder="URL аватарки" value={form.iconUrl} onChange={onChange} />
                <button className="button" type="submit">Зареєструватися</button>
            </form>
        </div>
    );
}