<template>
  <Dropdown v-model.number="selectedYear" :options="years" :editable="true" :class="{ 'p-invalid': hasError }" :disabled="disabled"/>
</template>

<script lang="ts" setup>
import { useSessionStore } from "@/store/session";
import { ref, watch } from "vue";

const sessionStore = useSessionStore();
const selectedYear = ref(sessionStore.year);
const hasError = ref(false);
const years = ref(Array(11).fill(0).map((_, i) => {
  return sessionStore.year - 5 + i;
}));

defineProps<{
  disabled?: boolean
}>()

watch(() => sessionStore.year, (n) => { selectedYear.value = n });

watch(selectedYear, (s) => {
  if (s > 1990 && s < 2200) {
    sessionStore.setYear(s);
    hasError.value = false;
  } else {
    hasError.value = true;
  }
});
</script>