<template>
	<div class="layout-menu-container">
		<AppSubmenu :items="model" class="layout-menu" :root="true" />
	</div>
</template>

<script lang="ts" setup>
import AppSubmenu, { MenuItem } from './AppSubmenu.vue';
 defineProps<{
	model: MenuItem[]
 }>();
</script>