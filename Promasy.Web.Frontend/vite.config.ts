import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import { resolve } from "path";
import vueI18n from "@intlify/vite-plugin-vue-i18n";
import mkcert from "vite-plugin-mkcert";
import svgLoader from "vite-svg-loader";

// https://vitejs.dev/config/
export default defineConfig({
  base: "./",
  resolve: {
    alias: {
      "@": resolve(__dirname, "./src"),
      "vue-i18n": "vue-i18n/dist/vue-i18n.runtime.esm-bundler.js",
    },
  },
  server: {
    https: true,
    host: process.env.NODE_ENV !== "production",
    proxy: {
      "/api": {
        target: "https://localhost:5001",
        changeOrigin: true,
        secure: false,
      },
    },
  },
  plugins: [
    vue(),
    vueI18n({
      include: resolve(__dirname, "./src/i18n/locales/**"),
    }),
    mkcert(),
    svgLoader(),
  ],
  css: {
    devSourcemap: process.env.NODE_ENV !== "production",
  },
  build: {
    sourcemap: process.env.NODE_ENV !== "production",
  },
});
