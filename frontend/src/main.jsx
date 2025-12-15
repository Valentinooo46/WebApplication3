import React from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import App from "./App";
import Callback from "./pages/Callback";
import Profile from "./pages/Profile";
import Login from "./pages/Login";
import Register from "./pages/Register";
import LoginLocal from "./pages/LoginLocal";

import "./styles.css";

createRoot(document.getElementById("root")).render(
    <React.StrictMode>
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<App />}>
                    <Route index element={<Login />} /> {/* Google login */}
                    <Route path="login-local" element={<LoginLocal />} /> {/* Email+Password login */}
                    <Route path="register" element={<Register />} /> {/* Registration */}
                    <Route path="auth/callback" element={<Callback />} />
                    <Route path="profile" element={<Profile />} />
                </Route>
            </Routes>

        </BrowserRouter>
    </React.StrictMode>
);