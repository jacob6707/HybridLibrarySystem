import tailwindcss from "@tailwindcss/vite";
import react from "@vitejs/plugin-react";
import { fileURLToPath, URL } from "node:url";
import { defineConfig } from "vite";

const backend = "https://localhost:7238";

// https://vite.dev/config/
export default defineConfig({
	plugins: [react(), tailwindcss()],
	resolve: {
		alias: {
			"@": fileURLToPath(new URL("./src", import.meta.url)),
		},
	},
	server: {
		// Proxy API and Identity pages so the dev server is same-origin with the ASP.NET app.
		proxy: {
			"/api": { target: backend, changeOrigin: true, secure: false },
			"/Identity": { target: backend, changeOrigin: true, secure: false },
		},
	},
	build: {
		emptyOutDir: true,
		outDir: "../HybridLibrarySystem.Web/wwwroot/app",
	},
	base: "/app",
});
