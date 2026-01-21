<template>
  <Toast/>
  <router-view></router-view>
</template>

<script lang="ts" setup>
import { setPrimeVueLocale } from "@/i18n";
import { usePrimeVue } from "primevue/config";
import { onMounted } from "vue";
import Toast from "primevue/toast";
import { initStores } from "./store";
import { useSessionStore } from "./store/session";
import LocalStore, { keys } from "./services/local-store";
import { useI18n } from "vue-i18n";

initStores();

const sessionStore = useSessionStore();
const { locale } = useI18n();
const primeVue = usePrimeVue();

initLocale();
initDarkMode();
onMounted(async () => {
  await initUserAsync();
});

function initDarkMode() {
  applyDarkMode(sessionStore.darkMode);
  sessionStore.$subscribe((mutation, state) => {
    applyDarkMode(state.darkMode);
  });
}

function applyDarkMode(isDark: boolean) {
  if (isDark) {
    document.documentElement.classList.add("app-dark");
  } else {
    document.documentElement.classList.remove("app-dark");
  }
}

function initLocale() {
  locale.value = sessionStore.locale;
  setPrimeVueLocale(primeVue, locale.value);
  sessionStore.$subscribe((mutation, state) => {
    if (state.locale !== locale.value) {
      locale.value = state.locale;
      setPrimeVueLocale(primeVue, locale.value);
    }
  });
}

async function initUserAsync() {
  const token = LocalStore.get(keys.token);
  if (token) {
    sessionStore.loginWithToken(token);
    await sessionStore.refreshTokenAsync();
  }
}
</script>