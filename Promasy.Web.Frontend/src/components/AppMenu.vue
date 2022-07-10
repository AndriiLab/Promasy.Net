<template>
  <div class="layout-menu-container">
    <AppSubmenu :items="model" class="layout-menu" :root="true"/>
  </div>
</template>

<script lang="ts" setup>
import { MenuItem } from "./interfaces/menu-item";
import AppSubmenu from "./AppSubmenu.vue";

defineProps<{
  model: MenuItem[]
}>();
</script>