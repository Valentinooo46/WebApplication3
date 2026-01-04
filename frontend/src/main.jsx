import React, { Suspense } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter, Routes, Route } from "react-router-dom";

import App from "./App";
import Callback from "./pages/Callback";
import Profile from "./pages/Profile";
import Login from "./pages/Login";
import Register from "./pages/Register";
import LoginLocal from "./pages/LoginLocal";

import "./styles.css";

// 👇 ВАЖЛИВО: викликаємо mod(), бо remoteEntry повертає функцію
const FlightsTable = React.lazy(() =>
    import("remoteApp/FlightsTable").then(mod => ({ default: mod.default }))
);

createRoot(document.getElementById("root")).render(
    <React.StrictMode>
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<App />}>
                    <Route index element={<Login />} />
                    <Route path="login-local" element={<LoginLocal />} />
                    <Route path="register" element={<Register />} />
                    <Route path="auth/callback" element={<Callback />} />
                    <Route path="profile" element={<Profile />} />

                    {/* Remote route */}
                    <Route
                        path="flights"
                        element={
                            <Suspense fallback={<div>Loading remote component...</div>}>
                                <FlightsTable />
                            </Suspense>
                        }
                    />
                </Route>
            </Routes>
        </BrowserRouter>
    </React.StrictMode>
);