import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import federation from '@originjs/vite-plugin-federation';

export default defineConfig({
    resolve: {
        dedupe: ['react', 'react-dom'],
    },
    plugins: [
        react(),
        federation({
            name: 'hostApp',
            remotes: {
                // Рекомендуется использовать точный путь к remoteEntry.js
                remoteApp: 'http://localhost:4173/assets/remoteEntry.js'
            },
            shared: ['react', 'react-dom', 'react-router-dom']
        })
    ],
    build: {
        target: 'esnext',
        minify: false,
        cssCodeSplit: true
    },
    server: {
        port: 5174,
        cors: true
    }
});