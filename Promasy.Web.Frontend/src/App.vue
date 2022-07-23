<template>
  <Toast/>
  <router-view></router-view>
</template>

<script lang="ts" setup>
import { setPrimeVueLocale } from "@/i18n";
import { onMounted } from "vue";
import Toast from "primevue/toast";
import { initStores } from "./store";
import { useSessionStore } from "./store/session";
import { useRolesStore } from "./store/roles";
import LocalStore, { keys } from "./services/local-store";
import { useI18n } from "vue-i18n";

initStores();

const sessionStore = useSessionStore();
const rolesStore = useRolesStore();

initLocale();
onMounted(async () => {
  initUser();
  await rolesStore.setRolesAsync();
});

function initLocale() {
  const { locale } = useI18n();
  locale.value = sessionStore.locale;
  setPrimeVueLocale();
  sessionStore.$subscribe(async (mutation, state) => {
    if (state.locale !== locale.value) {
      locale.value = state.locale;
      setPrimeVueLocale();
      await rolesStore.setRolesAsync();
    }
  });
}

function initUser() {
  const token = LocalStore.get(keys.token);
  if (token) {
    sessionStore.loginWithToken(token);
  }
}
</script>