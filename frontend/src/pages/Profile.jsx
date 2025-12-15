import React, { useEffect, useState } from "react";
import { getAuthData, clearAuthData } from "../services/auth";
import { useNavigate } from "react-router-dom";

export default function Profile() {
    const [auth, setAuth] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        const data = getAuthData();
        setAuth(data);
        console.log(data);
    }, []);

    const onLogout = () => {
        clearAuthData();
        navigate("/", { replace: true });
    };

    if (!auth) {
        return (
            <div style={{ textAlign: "center" }}>
                <h3>Немає даних аутентифікації</h3>
                <p style={{ color: "var(--muted)" }}>Будь ласка, увійдіть через Google</p>
            </div>
        );
    }

    const { user, token } = auth;

    return (
        <div style={{ display: "flex", gap: 18, alignItems: "flex-start" }}>
            <div style={{ flex: 1 }}>
                <h3>Профіль</h3>
                <div className="profile" style={{ marginTop: 12 }}>
                    <div className="avatar">
                        {user.iconUrl ? <img src={user.iconUrl} alt="avatar" /> : <div style={{ padding: 12 }}>{(user.name || "U").slice(0, 2)}</div>}
                    </div>
                    <div>
                        <div style={{ fontWeight: 700, fontSize: 18 }}>{user.name}</div>
                        <div style={{ color: "var(--muted)", fontSize: 14 }}>{user.email}</div>
                        <div style={{ marginTop: 8, color: "var(--muted)", fontSize: 13 }}>Google Sub: {user.sub}</div>
                    </div>
                </div>

                <div style={{ marginTop: 18 }}>
                    <button className="button ghost" onClick={onLogout}>Вийти</button>
                </div>
            </div>

            <div style={{ width: 360 }}>
                <div style={{ fontSize: 13, color: "var(--muted)", marginBottom: 8 }}>Внутрішній токен (коротко):</div>
                <div style={{ background: "rgba(255,255,255,0.02)", padding: 12, borderRadius: 8, wordBreak: "break-all" }}>{token}</div>
                <div style={{ fontSize: 13, color: "var(--muted)", marginTop: 12 }}>Відповідь сервера (повністю):</div>
                <pre style={{ background: "rgba(255,255,255,0.01)", padding: 12, borderRadius: 8, overflowX: "auto", color: "var(--muted)", fontSize: 12 }}>{JSON.stringify(auth, null, 2)}</pre>
            </div>
        </div>
    );
}