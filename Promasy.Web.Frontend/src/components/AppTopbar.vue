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
        <button class="p-link layout-topbar-button" @click="onSettingsMenuToggle" v-tooltip.bottom="t('settings')">
          <i class="pi pi-cog"></i>
        </button>
        <OverlayPanel ref="settingsMenuPanel">
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
        </OverlayPanel>
      </li>
      <li>
        <button class="p-link layout-topbar-button" @click="onProfileMenuToggle" v-tooltip.bottom="t('profile')">
          <i class="pi pi-user"></i>
        </button>
        <OverlayPanel ref="profileMenuPanel">
          <div class="text-900 font-medium text-xl mb-2">{{ t('welcomeUser', {firstName: user?.firstName}) }}</div>
          <div class="text-600">
            <RoleBadge v-for="role in user?.roles" :key="role" :role="role" v-tooltip.left="t('role')"></RoleBadge>
          </div>
          <hr class="my-3 mx-0 border-top-1 border-none surface-border"/>
          <ul class="list-none p-0 m-0 flex-grow-1">
            <li class="flex align-items-center mb-3">
              <i class="pi pi-building text-green-500 mr-2"></i>
              <span v-tooltip.left="t('organization')">{{ user?.organization }}</span>
            </li>
            <li class="flex align-items-center mb-3">
              <i class="pi pi-sitemap text-green-500 mr-2"></i>
              <span v-tooltip.left="t('department')">{{ user?.department }}</span>
            </li>
            <li class="flex align-items-center mb-3">
              <i class="pi pi-briefcase text-green-500 mr-2"></i>
              <span v-tooltip.left="t('subDepartment')">{{ user?.subDepartment }}</span>
            </li>
          </ul>
          <hr class="mb-3 mx-0 border-top-1 border-none surface-border mt-auto"/>
          <div class="flex justify-content-between">
            <router-link to="/me">
              <Button :label="t('userProfile')" icon="pi pi-user" class="p-button-info p-button-sm"
                      @click="() => profileMenuPanel.toggle(false)"></Button>
            </router-link>
            <router-link to="/logout">
              <Button :label="t('logout')" icon="pi pi-sign-out" class="p-button-danger p-button-sm"></Button>
            </router-link>
          </div>

        </OverlayPanel>
      </li>
    </ul>
  </div>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import { useI18n } from "vue-i18n";
import { useSessionStore } from "@/store/session";
import LanguageSelector from "./LanguageSelector.vue";
import RoleBadge from "./RoleBadge.vue";
import YearSelector from "./YearSelector.vue";

const { t } = useI18n({ useScope: "local" });
const profileMenuPanel = ref(null);
const settingsMenuPanel = ref(null);
const { user } = useSessionStore();

const emit = defineEmits([ "menu-toggle", "topbar-settings-menu-toggle" ]);

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
  "organization": "Organization",
  "department": "Department",
  "subDepartment": "Subdepartment",
  "logout": "Logout",
  "role": "Role",
  "profile": "Profile",
  "settings": "Settings",
  "userProfile": "My Profile"
}
</i18n>

<i18n locale="uk">
{
  "welcomeUser": "{firstName}, вітаємо",
  "organization": "Організація",
  "department": "Відділ",
  "subDepartment": "Підрозділ",
  "logout": "Вийти",
  "role": "Роль",
  "profile": "Профіль користувача",
  "settings": "Налаштування",
  "userProfile": "Мій профіль"
}
</i18n>