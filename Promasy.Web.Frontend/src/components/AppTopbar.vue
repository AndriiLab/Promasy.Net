<template>
  <div class="layout-topbar">
    <router-link to="/" class="layout-topbar-logo">
      <img alt="Promasy Logo" :src="'/src/assets/logo.png'"/>
    </router-link>
    <button class="p-link layout-menu-button layout-topbar-button" @click="onMenuToggle($event)">
      <i class="pi pi-bars"></i>
    </button>

    <ul class="layout-topbar-menu hidden lg:flex origin-top">
      <li>
        <button class="p-link layout-topbar-button" @click="onDarkModeToggle" v-tooltip.bottom="t('darkMode')">
          <i :class="['pi', sessionStore.darkMode ? 'pi-moon' : 'pi-sun']"></i>
        </button>
      </li>
      <li>
        <button class="p-link layout-topbar-button" @click="onSettingsMenuToggle" v-tooltip.bottom="t('settings')">
          <i class="pi pi-cog"></i>
        </button>
        <Popover ref="settingsMenuPanel">
          <div class="p-fluid formgrid grid w-15rem">
            <div class="field col-12 text-900 font-medium text-xl mb-2">{{ t('settings') }}</div>
            <hr class="field col-12 mb-3 mx-0 border-top-1 border-none surface-border mt-auto"/>
            <div class="field col-12">
              <label for="language">{{ t('language') }}</label>
              <LanguageSelector id="language"></LanguageSelector>
            </div>
            <div class="field col-12">
              <label for="currentYear">{{ t('queryYear') }}</label>
              <YearSelector id="currentYear"></YearSelector>
            </div>
          </div>
        </Popover>
      </li>
      <li>
        <button class="p-link layout-topbar-button" @click="onProfileMenuToggle" v-tooltip.bottom="t('profile')">
          <i class="pi pi-user"></i>
        </button>
        <Popover ref="profileMenuPanel">
          <div class="text-900 font-medium text-xl mb-2">{{ t('welcomeUser', {firstName: user?.firstName}) }}</div>
          <UserInfoSection :user="user" :config="{ role: true, organization: true, department: true, subDepartment: true } as UserInfoSectionConfig"/>
          <hr class="mb-3 mx-0 border-top-1 border-none surface-border mt-auto"/>
          <div class="flex justify-content-between">
            <router-link to="/me">
              <Button :label="t('userProfile')" icon="pi pi-user"  severity="info" size="small"
                      @click="() => profileMenuPanel.toggle(false)"></Button>
            </router-link>
            <router-link to="/logout">
              <Button :label="t('logout')" icon="pi pi-sign-out"  severity="danger" size="small"></Button>
            </router-link>
          </div>

        </Popover>
      </li>
    </ul>
  </div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useSessionStore } from "@/store/session";
import LanguageSelector from "./LanguageSelector.vue";
import YearSelector from "./YearSelector.vue";
import UserInfoSection from "./UserInfoSection.vue";
import {UserInfoSectionConfig} from "@/components/UserInfoSection.vue";

const { t } = useI18n({ useScope: "local" });
const profileMenuPanel = ref(null);
const settingsMenuPanel = ref(null);
const sessionStore = useSessionStore();
const { user } = sessionStore;

const emit = defineEmits([ "menu-toggle", "topbar-settings-menu-toggle" ]);

function onDarkModeToggle() {
  sessionStore.setDarkMode(!sessionStore.darkMode);
}

function onMenuToggle(event: Event) {
  emit("menu-toggle", event);
}

function onProfileMenuToggle(event: Event) {
  profileMenuPanel.value?.toggle(event);
}

function onSettingsMenuToggle(event: Event) {
  settingsMenuPanel.value?.toggle(event);
}
</script>

<i18n locale="en">
{
  "welcomeUser": "Welcome, {firstName}",
  "logout": "Logout",
  "profile": "Profile",
  "settings": "Settings",
  "userProfile": "My Profile",
  "darkMode": "Dark Mode"
}
</i18n>

<i18n locale="uk">
{
  "welcomeUser": "{firstName}, вітаємо",
  "logout": "Вийти",
  "profile": "Профіль користувача",
  "settings": "Налаштування",
  "userProfile": "Мій профіль",
  "darkMode": "Темна тема"
}
</i18n>






