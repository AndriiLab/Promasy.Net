<template>
  <div class="layout-menu-container">
    <AppSubmenu :items="model" class="layout-menu" :root="true"/>
  </div>
</template>

<script lang="ts" setup>
import { MenuItem } from "primevue/menuitem";
import AppSubmenu from "./AppSubmenu.vue";

defineProps<{
  model: MenuItem[]
}>();
</script>