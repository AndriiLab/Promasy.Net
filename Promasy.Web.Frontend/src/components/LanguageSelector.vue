<template>
  <Dropdown v-model="language" :options="availableLanguages" optionLabel="name" optionValue="value">
    <template #value="slotProps">
      <div class="flex align-items-center">
        <span :class="'mr-2 flag flag-' + getFlag(slotProps.value)" style="width:18px; height: 12px"/>
        <div>{{ getNameByKey(slotProps.value, availableLanguages) }}</div>
      </div>
    </template>
    <template #option="slotProps">
      <div class="flex align-items-center">
        <span :class="'mr-2 flag flag-' + getFlag(slotProps.option.value)" style="width:18px; height: 12px"/>
        <div>{{ slotProps.option.name }}</div>
      </div>
    </template>
  </Dropdown>
</template>

<script lang="ts" setup>
import { ref, watch } from "vue";
import { availableLanguages } from "@/i18n";
import { useSessionStore } from "@/store/session";

const sessionStore = useSessionStore();
const language = ref(sessionStore.locale);

function getFlag(language: string) {
  switch (language) {
    case "en":
      return "uk";
    case "uk":
      return "ua";
    default:
      break;
  }
}

function getNameByKey(key: string, items: SelectObject<string>[]): string | undefined {
  return items.find(i => i.value === key)?.name;
}

watch(language, (l) => sessionStore.setLanguage(l));
</script>
