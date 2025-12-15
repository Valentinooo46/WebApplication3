import React, { useState } from "react";
import { saveAuthData } from "../services/auth";
import { useNavigate } from "react-router-dom";

export default function LoginLocal() {
  const [form, setForm] = useState({ email: "", password: "" });
  const navigate = useNavigate();

  const onChange = e => setForm({ ...form, [e.target.name]: e.target.value });

  const onSubmit = async e => {
    e.preventDefault();
    const backendUrl = import.meta.env.VITE_BACKEND_URL;

    const res = await fetch(`${backendUrl}/api/auth/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(form)
    });

    const data = await res.json();
    if (!res.ok) {
      alert("Помилка входу");
      return;
    }

    // Очікуємо { token, user }
    saveAuthData(data);
    navigate("/profile");
  };

  return (
    <div className="center">
      <h2>Логін (Email + Пароль)</h2>
      <form onSubmit={onSubmit} style={{ display: "flex", flexDirection: "column", gap: 12, maxWidth: 320 }}>
        <input name="email" placeholder="Email" value={form.email} onChange={onChange} />
        <input name="password" type="password" placeholder="Пароль" value={form.password} onChange={onChange} />
        <button className="button" type="submit">Увійти</button>
      </form>
    </div>
  );
}