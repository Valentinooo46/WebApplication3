import React from "react";
import { randomString, generateCodeChallenge, saveVerifier } from "../services/auth";

export default function Login() {
    const onLogin = async () => {
        const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID;
        if (!clientId) {
            alert("Please set VITE_GOOGLE_CLIENT_ID in .env");
            return;
        }

        const redirectUri = `${window.location.origin}/auth/callback`;
        const verifier = randomString(64);
        saveVerifier(verifier);
        const challenge = await generateCodeChallenge(verifier);
        const state = randomString(16);

        sessionStorage.setItem("oauth_state", state);

        const params = new URLSearchParams({
            client_id: clientId,
            redirect_uri: redirectUri,
            response_type: "code",
            scope: "openid email profile",
            include_granted_scopes: "true",
            state,
            code_challenge: challenge,
            code_challenge_method: "S256",
            prompt: "select_account"
        });

        const authUrl = `https://accounts.google.com/o/oauth2/v2/auth?${params.toString()}`;
        window.location.href = authUrl;
    };

    return (
        <div className="center">
            <div style={{ maxWidth: 640 }}>
                <h2>Увійти через Google</h2>
                <p style={{ color: "var(--muted)" }}>Натисніть кнопку, щоб перейти на сторінку Google. Після входу Google поверне authorization code до callback.</p>
                <div style={{ marginTop: 18 }}>
                    <button className="button" onClick={onLogin}>
                        <svg width="18" height="18" viewBox="0 0 48 48" style={{ marginRight: 8 }}><path fill="#EA4335" d="M24 9.5c3.9 0 6.6 1.7 8.1 3.1l6-5.8C33.8 4 29.2 2 24 2 14.9 2 7.5 7.6 4.3 15.1l7.7 6C13.1 16.3 18 9.5 24 9.5z" /><path fill="#34A853" d="M46.5 24.5c0-1.6-.1-2.8-.4-4H24v8h12.7c-.5 2.9-2.6 6.9-8.7 9.6l7.7 6.1C43.5 37.7 46.5 31.8 46.5 24.5z" /><path fill="#4A90E2" d="M12 29.3c-1.1-3.3-1.1-6.8 0-10.1l-7.7-6C1.1 17.5 0 20.7 0 24c0 3.3 1.1 6.5 4.3 10.7l7.7-5.4z" /><path fill="#FBBC05" d="M24 46c5.2 0 9.8-1.7 13.4-4.6l-7.7-6C29.3 36.8 26.4 38 24 38c-5.9 0-10.8-6.8-12-12.9l-7.7 5.4C7.5 40.4 14.9 46 24 46z" /></svg>
                        Sign in with Google
                    </button>
                </div>
            </div>
        </div>
    );
}