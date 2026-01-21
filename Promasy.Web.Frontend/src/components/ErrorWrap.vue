<template>
  <div :id="id">
    <slot></slot>
    <small v-for="err in errors" :key="err.$uid" class="p-error block">{{ err.$message }}</small>
    <small v-for="err in externalErrors" :key="getRandomId()" class="p-error block">{{ err }}</small>
  </div>
</template>

<script lang="ts" setup>
import { getRandomId } from "@/utils/randomization-utils";
import { ErrorObject } from "@vuelidate/core";
import { watch, computed } from "vue";

const inputErrorClass = "p-invalid";
const invalidProp = "invalid";
const props = defineProps<{
  errors: ErrorObject[],
  externalErrors?: string[]
}>();
const id = getRandomId();
const hasErrors = computed(() => !!props.errors.length || !!props.externalErrors?.length);
watch(hasErrors, () => {
  const slotInputs = document.getElementById(id)?.querySelectorAll("input, select, textarea, .p-inputtext, .p-select, .p-datepicker, .p-inputnumber");
  slotInputs?.forEach((si) => {
    if (hasErrors.value) {
      si.setAttribute(invalidProp, "");
      si.classList.add(inputErrorClass);
    } else {
      si.removeAttribute(invalidProp);
      si.classList.remove(inputErrorClass);
    }
  });
});
</script>