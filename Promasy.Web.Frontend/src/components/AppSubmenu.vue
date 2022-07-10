<template>
  <ul v-if="items">
    <template v-for="(item, i) of items">
      <li v-if="visible(item) && !item.separator" :key="item.label || i"
          :class="[{ 'layout-menuitem-category': root, 'active-menuitem': activeIndex === i && !item.to && !item.disabled }]"
          role="none">
        <template v-if="root">
          <div class="layout-menuitem-root-text" :aria-label="item.label">{{ item.label }}</div>
          <AppSubmenu :items="getSubItems(item)" @menuitem-click="$emit('menuitem-click', $event)"
                      :root="false"></AppSubmenu>
        </template>
        <template v-else>
          <router-link v-if="item.to" :to="item.to"
                       :class="[item.class, 'p-ripple', { 'p-disabled': item.disabled }]" :style="item.style"
                       @click="onMenuItemClick($event, item, i)" :target="item.target" :aria-label="item.label" exact
                       role="menuitem" v-ripple>
            <i :class="item.icon"></i>
            <span>{{ item.label }}</span>
            <i v-if="item.items" class="pi pi-fw pi-angle-down menuitem-toggle-icon"></i>
            <Badge v-if="item.badge" :value="item.badge"></Badge>
          </router-link>
          <a v-if="!item.to" :href="item.url || '#'" :style="item.style"
             :class="[item.class, 'p-ripple', { 'p-disabled': item.disabled }]"
             @click="onMenuItemClick($event, item, i)" :target="item.target" :aria-label="item.label"
             role="menuitem" v-ripple>
            <i :class="item.icon"></i>
            <span>{{ item.label }}</span>
            <i v-if="item.items" class="pi pi-fw pi-angle-down menuitem-toggle-icon"></i>
            <Badge v-if="item.badge" :value="item.badge"></Badge>
          </a>
          <transition name="layout-submenu-wrapper">
            <AppSubmenu v-show="activeIndex === i" :items="getSubItems(item)"
                        @menuitem-click="$emit('menuitem-click', $event)" :root="false"></AppSubmenu>
          </transition>
        </template>
      </li>
      <li class="p-menu-separator" :style="item.style" v-if="visible(item) && item.separator"
          :key="'separator' + i" role="separator"></li>
    </template>
  </ul>
</template>
<script lang="ts" setup>
import { MenuItem, MenuItemCommandEvent } from "@/components/interfaces/menu-item";
import { ref } from "vue";

defineProps<{
  items: MenuItem[],
  root: boolean
}>();
const emit = defineEmits([ "menuitem-click" ]);
const activeIndex = ref(-1);

function onMenuItemClick(event: MenuItemCommandEvent, item: MenuItem, index: number) {
  if (item.disabled) {
    event.preventDefault();
    return;
  }

  if (!item.to && !item.url) {
    event.preventDefault();
  }

  if (item.command) {
    item.command(event);
  }

  activeIndex.value = index === activeIndex.value ? -1 : index;

  emit("menuitem-click", {
    originalEvent: event,
    item: item,
  });
}

function visible(item: MenuItem) {
  return typeof item.visible === "function" ? item.visible() : item.visible !== false;
}

function getSubItems(item: MenuItem) {
  return visible(item) && item.items?.length ? item.items : [];

}
</script>
