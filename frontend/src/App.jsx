import React, { Suspense } from "react";
import { Outlet, Link } from "react-router-dom";


export default function App() {
    return (
        <div className="app">
            <div className="header">
                <div className="logo">
                    <svg width="36" height="36" viewBox="0 0 24 24" fill="none"><path d="M12 2l3.09 6.26L22 9.27l-5 4.87L18.18 22 12 18.56 5.82 22 7 14.14 2 9.27l6.91-1.01L12 2z" fill="#7c3aed" /></svg>
                    <div>
                        <div className="brand">AuthDemo</div>
                        <div style={{ fontSize: 12, color: "var(--muted)" }}>Google PKCE + ASP.NET Core</div>
                    </div>
                </div>
                <div>
                    <Link to="/" style={{ marginRight: 12, color: "var(--muted)", textDecoration: "none" }}>Home</Link>
                    <Link to="/profile" style={{ color: "var(--muted)", textDecoration: "none" }}>Profile</Link>
                </div>
            </div>

            <div className="card">
                <Outlet />
            </div>
        </div>
    );
}