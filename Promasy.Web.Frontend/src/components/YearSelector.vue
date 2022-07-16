<template>
  <Dropdown v-model.number="selectedYear" :options="years" :editable="true" :class="{ 'p-invalid': hasError }"/>
</template>

<script lang="ts" setup>
import { useSessionStore } from "@/store/session";
import { ref, watch } from "vue";

const { year, setYear } = useSessionStore();
const selectedYear = ref(year);
const hasError = ref(false);
const years = ref(Array(11).fill(0).map((_, i) => {
  return year - 5 + i;
}));

watch(selectedYear, (s) => {
  if (s > 1990 && s < 2200) {
    setYear(s);
    hasError.value = false;
  } else {
    hasError.value = true;
  }
});
</script>