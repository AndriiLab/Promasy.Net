<template>
  <div :class="containerClass" @click="onWrapperClick">
    <AppTopBar @menu-toggle="onMenuToggle($event)"/>
    <div class="layout-sidebar" @click="onSidebarClick">
      <AppMenu :model="menu" @menuitem-click="onMenuItemClick"/>
    </div>

    <div class="layout-main-container">
      <div class="layout-main">
        <router-view/>
      </div>
      <AppFooter/>
    </div>

    <transition name="layout-mask">
      <div class="layout-mask p-component-overlay" v-if="mobileMenuActive"></div>
    </transition>
  </div>
</template>

<script lang="ts" setup>
import AppTopBar from "@/components/AppTopbar.vue";
import AppMenu from "@/components/AppMenu.vue";
import AppFooter from "@/components/AppFooter.vue";
import { MenuItem } from "@/components/interfaces/menu-item";
import { ref, computed } from "vue";
import { useI18n } from "vue-i18n";
import { useSessionStore } from "@/store/session";

const { t } = useI18n({ useScope: "local" });
const { user } = useSessionStore();
const layoutMode = ref("static");
const staticMenuInactive = ref(false);
const overlayMenuActive = ref(false);
const mobileMenuActive = ref(false);
const menuClick = ref(false);

const containerClass = computed(() => {
  return [ "layout-wrapper", {
    "layout-overlay": layoutMode.value === "overlay",
    "layout-static": layoutMode.value === "static",
    "layout-static-sidebar-inactive": staticMenuInactive.value && layoutMode.value === "static",
    "layout-overlay-sidebar-active": overlayMenuActive.value && layoutMode.value === "overlay",
    "layout-mobile-sidebar-active": mobileMenuActive.value,
  } ];
});

const menu: MenuItem[] = [
  {
    label: t("orders"),
    items: [
      { label: t("ordersList"), icon: "pi pi-fw pi-shopping-cart", to: { name: "Orders" } },
      { label: t("cpv"), icon: "pi pi-fw pi-book", to: { name: "CpvList" } },
      { label: t("units"), icon: "pi pi-fw pi-arrows-h", to: { name: "Units" } },
      { label: t("manufacturers"), icon: "pi pi-fw pi-box", to: { name: "Manufacturers" } },
      { label: t("suppliers"), icon: "pi pi-fw pi-car", to: { name: "Suppliers" }},
    ],
  },
  {
    label: t("finances"),
    items: [
      { label: t("financesList"), icon: "pi pi-fw pi-wallet", to: { name: "Finances" } },
      { label: t("cpvOrders"), icon: "pi pi-fw pi-chart-line", to: { name: "OrdersByCpv" } },
    ],
  },
  {
    label: t("organization"),
    items: [
      { label: t("thisOrganization"), icon: "pi pi-fw pi-building", to: { name: "Organization", params: { organizationId: user?.organizationId }}},
      { label: t("departments"), icon: "pi pi-fw pi-sitemap", to: { name: "Departments" } },
      { label: t("employees"), icon: "pi pi-fw pi-users", to: { name: "Employees" } },
    ],
  },
  {
    label: t("user"),
    visible: () => !isDesktop(),
    items: [
      { label: t("userProfile"), icon: "pi pi-fw pi-user", to: { name: "EmployeeMe" } },
      { label: t("logout"), icon: "pi pi-fw pi-sign-out", to: { name: "Logout" } },
    ],
  },
  {
    label: "(temp)",
    items: [

      { label: "Error", icon: "pi pi-fw pi-times-circle", to: "/error" },
      { label: "Not Found", icon: "pi pi-fw pi-exclamation-circle", to: "/404" },
      { label: "Access Denied", icon: "pi pi-fw pi-lock", to: "/403" },
    ],
  },
];

function onWrapperClick() {
  if (!menuClick) {
    overlayMenuActive.value = false;
    mobileMenuActive.value = false;
  }

  menuClick.value = false;
}

function isDesktop() {
  return window.innerWidth >= 992;
};

function onMenuToggle(event: Event) {
  menuClick.value = true;

  if (isDesktop()) {
    if (layoutMode.value === "overlay") {
      if (mobileMenuActive.value === true) {
        overlayMenuActive.value = true;
      }

      overlayMenuActive.value = !overlayMenuActive;
      mobileMenuActive.value = false;
    } else if (layoutMode.value === "static") {
      staticMenuInactive.value = !staticMenuInactive.value;
    }
  } else {
    mobileMenuActive.value = !mobileMenuActive.value;
  }

  event.preventDefault();
}

function onSidebarClick() {
  menuClick.value = true;
}

function onMenuItemClick(event: Event) {
  if (event.item && !event.item.items) {
    overlayMenuActive.value = false;
    mobileMenuActive.value = false;
  }
}

</script>

<i18n locale="en">
{
  "orders": "Orders",
  "ordersList": "Orders",
  "cpv": "CPV",
  "finances": "Finances",
  "financesList": "Finances",
  "cpvOrders": "Orders by CPV",
  "units": "Units",
  "manufacturers": "Manufacturers",
  "suppliers": "Suppliers",
  "organization": "Orgaization",
  "thisOrganization": "Orgaization",
  "departments": "Departments",
  "subdepartments": "Sub-departments",
  "employees": "Employees",
  "user": "User",
  "userProfile": "My Profile",
  "logout": "Logout"
}
</i18n>

<i18n locale="uk">
{
  "orders": "Замовлення",
  "ordersList": "Замовлення",
  "cpv": "Довідник CPV",
  "finances": "Фінансування",
  "financesList": "Список тем",
  "cpvOrders": "Замовлення за CPV кодами",
  "units": "Розмірності",
  "manufacturers": "Виробники",
  "suppliers": "Постачальники",
  "organization": "Організація",
  "thisOrganization": "Організація",
  "departments": "Відділи",
  "subdepartments": "Підрозділи",
  "employees": "Працівники",
  "user": "Користувач",
  "userProfile": "Мій профіль",
  "logout": "Вийти"
}
</i18n>