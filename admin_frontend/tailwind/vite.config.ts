import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import svgr from "vite-plugin-svgr";
import federation from "@originjs/vite-plugin-federation";

export default defineConfig({
  resolve: {
    dedupe: ["react", "react-dom"],
  },
  plugins: [
    react(),
    svgr({
      svgrOptions: {
        icon: true,
        exportType: "named",
        namedExport: "ReactComponent",
      },
    }),
    federation({
      name: "admin",
      filename: "remoteEntry.js",
      exposes: {
        "./Admin": "./src/Admin.tsx",
      },
      shared: ["react", "react-dom", "react-router-dom"]
    })
  ],
  build: {
    target: "esnext",
  },
  server: {
    port: 4173,
    cors: true
  }
});
