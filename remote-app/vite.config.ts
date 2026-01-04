import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import federation from '@originjs/vite-plugin-federation';

export default defineConfig({
    plugins: [
        react(),
        federation({
            name: 'remoteApp',
            filename: 'remoteEntry.js',
            exposes: {
                './FlightsTable': './src/components/FlightsTable',
                './CitiesList': './src/components/CitiesList',
                './CountriesList': './src/components/CountriesList',
            },
            shared: ['react', 'react-dom', 'axios']
        })
    ],
    build: {
        target: 'esnext',
        minify: false,
        cssCodeSplit: true
    },
    server: {
        port: 5173
    }
});