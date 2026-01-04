import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import svgr from "vite-plugin-svgr";
import federation from "@originjs/vite-plugin-federation";


// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    svgr({
      svgrOptions: {
        icon: true,
        // This will transform your SVG to a React component
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
        shared: ["react", "react-dom"],
    })
    ],
    build: {
        target: "esnext",
    }
});
