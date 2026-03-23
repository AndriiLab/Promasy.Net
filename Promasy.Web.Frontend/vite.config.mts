import { defineConfig, loadEnv } from "vite";
import vue from "@vitejs/plugin-vue";
import path from 'path';
import { fileURLToPath } from 'node:url';
import VueI18nPlugin from "@intlify/unplugin-vue-i18n/vite";
import viteNetCoreSslPlugin from "./.vite/vite-plugin-netcore-https";
import svgLoader from "vite-svg-loader";
import tailwindcss from "@tailwindcss/vite";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// https://vitejs.dev/config/
export default ({ mode }) => {
  console.log(`Running in ${mode} mode`);
  const env = loadEnv(mode, __dirname, "");
  const isDevelopment = mode === "development";
  const apiUrl = env.VITE_PROMASY_API_URL;
  if (!apiUrl) {
    throw new Error(`VITE_PROMASY_API_URL is not defined in environment variables or .env file (mode: ${mode}).`);
  }
  return defineConfig({
    base: "./",
    resolve: {
      alias: {
        "@": path.resolve(__dirname, "./src"),
        "vue-i18n": "vue-i18n/dist/vue-i18n.runtime.esm-bundler.js",
      },
    },
    server: {
      host: isDevelopment,
      proxy: {
        "/g": "https://google.com",
        "/api": {
          target: apiUrl,
          changeOrigin: true,
          secure: false,
          configure: (proxy, _options) => {
            proxy.on('error', (err, _req, _res) => {
              console.log('proxy error', err);
            });
            proxy.on('proxyReq', (proxyReq, req, _res) => {
              console.log('Sending Request to the Target:', req.method, req.url);
            });
            proxy.on('proxyRes', (proxyRes, req, _res) => {
              console.log('Received Response from the Target:', proxyRes.statusCode, req.url);
            });
          },
        },
      },
    },
    plugins: [
      vue(),
      tailwindcss(),
      VueI18nPlugin({ include: [path.resolve(__dirname, "./src/i18n/locales/**")] }),
      viteNetCoreSslPlugin(),
      svgLoader(),
    ],
    css: {
      devSourcemap: isDevelopment,
    },
    build: {
      sourcemap: isDevelopment,
    },
  });
};
