// Утиліти PKCE та робота з localStorage
export function randomString(length = 43) {
    const array = new Uint8Array(length);
    crypto.getRandomValues(array);
    // base64url
    return btoa(String.fromCharCode.apply(null, [...array]))
        .replace(/\+/g, "-").replace(/\//g, "_").replace(/=+$/, "");
}

function base64urlEncode(buffer) {
    const bytes = new Uint8Array(buffer);
    let binary = "";
    for (let i = 0; i < bytes.byteLength; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return btoa(binary).replace(/\+/g, "-").replace(/\//g, "_").replace(/=+$/, "");
}

export async function generateCodeChallenge(verifier) {
    const encoder = new TextEncoder();
    const data = encoder.encode(verifier);
    const digest = await crypto.subtle.digest("SHA-256", data);
    return base64urlEncode(digest);
}

export function saveVerifier(verifier) {
    sessionStorage.setItem("pkce_verifier", verifier);
}

export function getVerifier() {
    return sessionStorage.getItem("pkce_verifier");
}

export function saveAuthData(data) {
    localStorage.setItem("auth_data", JSON.stringify(data));
}

export function getAuthData() {
    const s = localStorage.getItem("auth_data");
    return s ? JSON.parse(s) : null;
}

export function clearAuthData() {
    localStorage.removeItem("auth_data");
    sessionStorage.removeItem("pkce_verifier");
}