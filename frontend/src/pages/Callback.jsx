import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import confetti from "canvas-confetti";
import { getVerifier, saveAuthData } from "../services/auth";

export default function Callback() {
    const [status, setStatus] = useState("processing");
    const [message, setMessage] = useState("Обробляємо відповідь від Google...");
    const navigate = useNavigate();

    useEffect(() => {
        async function handle() {
            try {
                const qs = new URLSearchParams(window.location.search);
                const code = qs.get("code");
                const returnedState = qs.get("state");
                const savedState = sessionStorage.getItem("oauth_state");

                if (!code) {
                    const error = qs.get("error") || "no_code";
                    setStatus("error");
                    setMessage(`Помилка авторизації: ${error}`);
                    return;
                }
                if (returnedState !== savedState) {
                    setStatus("error");
                    setMessage("State не співпадає — можливий CSRF.");
                    return;
                }

                setMessage("Надсилаємо code на бекенд...");

                const backendUrl = import.meta.env.VITE_BACKEND_URL || "";
                const codeVerifier = getVerifier();
                const redirectUri = `${window.location.origin}/auth/callback`;

                const response = await fetch(`${backendUrl.replace(/\/+$/, "")}/api/auth/google`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify({ code, codeVerifier, redirectUri })
                });

                if (!response.ok) {
                    const txt = await response.text().catch(() => null);
                    setStatus("error");
                    setMessage(`Помилка від сервера: ${response.status} ${txt || ""}`);
                    return;
                }

                const data = await response.json();

                // Очікуємо, що бекенд повертає { token: "...", user: { email, name, picture, sub } }
                if (!data || !data.user) {
                    setStatus("error");
                    setMessage("Невірний формат відповіді від сервера.");
                    return;
                }

                // Зберігаємо дані для сторінки профілю
                saveAuthData(data);

                setStatus("success");
                setMessage("Успішно — входимо!");

                // запуск confetti
                confetti({
                    particleCount: 160,
                    spread: 70,
                    origin: { y: 0.6 }
                });

                // невелика пауза, потім переходимо на профіль
                setTimeout(() => {
                    navigate("/profile");
                }, 1600);

            } catch (err) {
                console.error(err);
                setStatus("error");
                setMessage("Внутрішня помилка обробки.");
            }
        }

        handle();
    }, [navigate]);

    return (
        <div className="center">
            <div className="anim" style={{ flexDirection: "column" }}>
                {status === "processing" && (
                    <>
                        <div className="loader" />
                        <div style={{ marginTop: 12, color: "var(--muted)" }}>{message}</div>
                    </>
                )}
                {status === "success" && (
                    <>
                        <div className="sparkle">Ласкаво просимо — щойно ввійшли 🎉</div>
                        <div style={{ marginTop: 10, color: "var(--muted)" }}>Переходимо до профілю...</div>
                    </>
                )}
                {status === "error" && (
                    <>
                        <div style={{ fontSize: 32, color: "#ff6b6b" }}>⚠️</div>
                        <div style={{ marginTop: 10 }}>{message}</div>
                    </>
                )}
            </div>
        </div>
    );
}